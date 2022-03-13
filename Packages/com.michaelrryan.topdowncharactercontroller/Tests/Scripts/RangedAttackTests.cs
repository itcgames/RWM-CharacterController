using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using TopdownCharacterController;

public class RangedAttackTests
{
	const string NPC_NAME = "Enemy";

	// How much extra time to add when waiting for a specific length of time
	//		to elapse to avoid falling under on different powered machines.
	const float SAFETY_MARGIN = 0.1f;

	private Dictionary<string, string> _damageInfo;

	private void HealthChangedCallback(float newHealth, Dictionary<string, string> damageInfo)
	{
		_damageInfo = damageInfo;
	}

	private void DeathCallback(Dictionary<string, string> damageInfo)
	{
		_damageInfo = damageInfo;
	}

	[SetUp]
	public void Setup()
	{
		SceneManager.LoadScene(TestUtilities.GetDefaultSceneName());
	}

	[UnityTest]
	public IEnumerator ProjectileFires()
	{
		// Disables the enemy to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		// Gets the player's ranged attack component and checks it's not null.
		RangedAttack rangedAttack = GetDefaultRangedAttackComponent();
		Assert.NotNull(rangedAttack);

		// Fires a projectile.
		GameObject fired = rangedAttack.Fire(Vector2.right);

		// Ensures the projectile and player share the same position.
		Assert.AreEqual(rangedAttack.transform.position, fired.transform.position);

		// Waits a frame.
		yield return null;

		// Ensures the projectile exists and matches the projectile returned.
		GameObject projectile = GameObject.Find(TestUtilities.PROJECTILE_INSTANCE_NAME);
		Assert.NotNull(projectile);
		Assert.AreSame(fired, projectile);
	}

	[UnityTest]
	public IEnumerator CannotFireDuringCooldown()
	{
		// Disables the enemy to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		// Gets the player's ranged attack component and checks it's not null.
		RangedAttack rangedAttack = GetDefaultRangedAttackComponent();
		Assert.NotNull(rangedAttack);

		// Fires a projectile and checks it's not null.
		GameObject fired = rangedAttack.Fire(Vector2.right);
		Assert.NotNull(fired);

		// Tries to fire another projectile and ensures it's null.
		fired = rangedAttack.Fire(Vector2.right);
		Assert.Null(fired);

		// Waits until the cooldown expires and fire again.
		yield return new WaitForSeconds(rangedAttack.Cooldown + SAFETY_MARGIN);
		fired = rangedAttack.Fire(Vector2.right);
		Assert.NotNull(fired);
	}

	[UnityTest]
	public IEnumerator DefaultProjectileCausesDamages()
	{
		// Disables the enemy to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		const float PROJECTILE_DISTANCE = 2.0f;

		// Gets the enemy character's behaviour.
		CharacterBehaviour enemy = TestUtilities.GetBehaviourByCharacterName(NPC_NAME);
		Assert.NotNull(enemy.Health);

		// Gets the player's ranged attack component and checks it's not null.
		RangedAttack rangedAttack = GetDefaultRangedAttackComponent();
		Assert.NotNull(rangedAttack);

		// Positions the player to the left of the enemy.
		rangedAttack.transform.position =
			enemy.transform.position + Vector3.left * PROJECTILE_DISTANCE;

		// Gets the projectile component from the fired projectile.
		GameObject projectileObj = rangedAttack.Fire(Vector2.right);
		Projectile projectile = projectileObj.GetComponent<Projectile>();

		// Gets the enemies health and waits for the bullet to hit.
		float enemyHP = enemy.Health.HP;
		yield return new WaitForSeconds((PROJECTILE_DISTANCE / projectile.Speed)
			+ SAFETY_MARGIN);

		// Checks that the enemy took damage and the projectile no longer exists.
		Assert.AreEqual(enemyHP - projectile.Damage, enemy.Health.HP);
		Assert.IsNull(GameObject.Find(TestUtilities.PROJECTILE_INSTANCE_NAME));
	}

	[UnityTest]
	public IEnumerator ProjectileIsDestroyedAfterDelay()
	{
		// Disables the enemy to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		const float PROJECTILE_EXPIRE_TIME = 0.1f;

		// Gets the player's ranged attack component and checks it's not null.
		RangedAttack rangedAttack = GetDefaultRangedAttackComponent();
		Assert.NotNull(rangedAttack);

		// Gets the projectile component.
		GameObject projectileObj = rangedAttack.projectilePrefab;
		Assert.NotNull(projectileObj);
		Projectile projectile = projectileObj.GetComponent<Projectile>();
		Assert.NotNull(projectile);

		// Sets the projectile expire time.
		float expireTime = projectile.ExpireTime;
		projectile.ExpireTime = PROJECTILE_EXPIRE_TIME;

		// Fires a projectile and ensures it's not null.
		rangedAttack.Fire(Vector2.left);
		GameObject fired = GameObject.Find(TestUtilities.PROJECTILE_INSTANCE_NAME);
		Assert.NotNull(fired);

		// Waits for the projectile to expire and ensures it's now null.
		yield return new WaitForSeconds(PROJECTILE_EXPIRE_TIME + SAFETY_MARGIN);
		fired = GameObject.Find(TestUtilities.PROJECTILE_INSTANCE_NAME);
		Assert.Null(fired);

		// Sets the expire time back to what it was as Unity does not do this
		//		automatically.
		projectile.ExpireTime = expireTime;
	}

	[UnityTest]
	public IEnumerator LimitedAmmoWorksAsExpected()
	{
		// Disables the enemy to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		const int AMMO = 3;
		const float COOLDOWN = 0.1f;

		// Gets the player's ranged attack component and checks it's not null.
		RangedAttack rangedAttack = GetDefaultRangedAttackComponent();
		Assert.NotNull(rangedAttack);

		// Setup the player's ranged attack component.
		rangedAttack.LimitedAmmo = true;
		rangedAttack.Ammo = AMMO;
		rangedAttack.Cooldown = COOLDOWN;

		// Loops for each ammo and fires.
		for (int i = 0; i < AMMO; ++i)
		{
			// Fires a projectile, checks the bullet is not null and waits for
			//		the cooldown to expire.
			GameObject fired = rangedAttack.Fire(Vector2.right);
			Assert.NotNull(fired);
			yield return new WaitForSeconds(COOLDOWN + SAFETY_MARGIN);
		}

		// Checks the ammo is now 0 and Fire() doesn't fire a bullet.
		Assert.AreEqual(0, rangedAttack.Ammo);
		GameObject failed = rangedAttack.Fire(Vector2.right);
		Assert.Null(failed);
	}

	[UnityTest]
	public IEnumerator AttackInfoIsPassedThroughHealthChangedCallbacks()
	{
		// Disables the enemy behaviour to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		const float PROJECTILE_DISTANCE = 2.0f;

		// Gets the enemy character's behaviour.
		CharacterBehaviour enemy = TestUtilities.GetBehaviourByCharacterName(NPC_NAME);
		Assert.NotNull(enemy.Health);

		// Gets the player's ranged attack component and checks it's not null.
		RangedAttack rangedAttack = GetDefaultRangedAttackComponent();
		Assert.NotNull(rangedAttack);

		// Initialises the checker variables.
		_damageInfo = null;

		// Assigns the attack info to the melee attack.
		const string KEY_NAME = "test_key";
		const string VALUE_NAME = "test_value";

		Dictionary<string, string> attackInfo = new Dictionary<string, string>();
		attackInfo.Add(KEY_NAME, VALUE_NAME);

		rangedAttack.AttackInfo = attackInfo;

		// Assigns the callback.
		enemy.Health.HealthChangedCallbacks.Add(HealthChangedCallback);

		// Positions the player to the left of the enemy.
		rangedAttack.transform.position =
			enemy.transform.position + Vector3.left * PROJECTILE_DISTANCE;

		// Gets the projectile component from the fired projectile.
		GameObject projectileObj = rangedAttack.Fire(Vector2.right);
		Projectile projectile = projectileObj.GetComponent<Projectile>();

		// Gets the enemies health and waits for the bullet to hit.
		float enemyHP = enemy.Health.HP;
		yield return new WaitForSeconds((PROJECTILE_DISTANCE / projectile.Speed)
			+ SAFETY_MARGIN);

		// Checks the damage info was passed through correctly.
		Assert.NotNull(_damageInfo);
		Assert.AreEqual(VALUE_NAME, _damageInfo[KEY_NAME]);
	}

	[UnityTest]
	public IEnumerator AttackInfoIsPassedThroughDeathCallbacks()
	{
		// Disables the enemy behaviour to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		const float PROJECTILE_DISTANCE = 2.0f;

		// Gets the player's ranged attack component and checks it's not null.
		RangedAttack rangedAttack = GetDefaultRangedAttackComponent();
		Assert.NotNull(rangedAttack);

		// Gets the enemy character's behaviour and sets its health to the projectile damage.
		CharacterBehaviour enemy = TestUtilities.GetBehaviourByCharacterName(NPC_NAME);
		Assert.NotNull(enemy.Health);

		Projectile projectile = rangedAttack.projectilePrefab.GetComponent<Projectile>();
		Assert.NotNull(projectile);
		enemy.Health.HP = projectile.Damage;

		// Initialises the checker variables.
		_damageInfo = null;

		// Assigns the attack info to the melee attack.
		const string KEY_NAME = "test_key";
		const string VALUE_NAME = "test_value";

		Dictionary<string, string> attackInfo = new Dictionary<string, string>();
		attackInfo.Add(KEY_NAME, VALUE_NAME);

		rangedAttack.AttackInfo = attackInfo;

		// Assigns the callback.
		enemy.Health.DeathCallbacks.Add(DeathCallback);

		// Positions the player to the left of the enemy.
		rangedAttack.transform.position =
			enemy.transform.position + Vector3.left * PROJECTILE_DISTANCE;

		// Fires the projectile and waits for it to hit.
		rangedAttack.Fire(Vector2.right);
		yield return new WaitForSeconds((PROJECTILE_DISTANCE / projectile.Speed)
			+ SAFETY_MARGIN);

		// Checks the damage info was passed through correctly.
		Assert.NotNull(_damageInfo);
		Assert.AreEqual(VALUE_NAME, _damageInfo[KEY_NAME]);
	}

	private RangedAttack GetDefaultRangedAttackComponent()
	{
		// Gets the player character.
		CharacterBehaviour player =
			TestUtilities.GetDefaultCharactersBehaviour();

		// Gets and returns the ranged attack component.
		return player.RangedAttack;
	}
}