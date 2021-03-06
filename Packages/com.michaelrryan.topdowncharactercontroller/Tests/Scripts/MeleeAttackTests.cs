using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using TopdownCharacterController;

public class MeleeAttackTests
{
	const string NPC_NAME = "Enemy";

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
	public IEnumerator AttackDamagesCharactersInARadius()
	{
		// Disables the enemy to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		const float ATTACK_RADIUS = 1.0f;
		const float ATTACK_DAMAGE = 1.0f;
		const float ENEMY_HEALTH = 2.0f;

		// Gets the character behaviours.
		CharacterBehaviour player = 
			TestUtilities.GetDefaultCharactersBehaviour();

		CharacterBehaviour enemy = 
			TestUtilities.GetBehaviourByCharacterName(NPC_NAME);

		Assert.NotNull(player.MeleeAttack);
		Assert.NotNull(player.Movement);
		Assert.NotNull(enemy.Health);

		// Sets the attack radius, attack damage, and enemy health.
		player.MeleeAttack.AttackRadius = ATTACK_RADIUS;
		player.MeleeAttack.AttackDamage = ATTACK_DAMAGE;
		enemy.Health.HP = ENEMY_HEALTH;

		// Makes the character face the right.
		player.Movement.MoveRight();
		yield return null;

		// Positions the enemy.
		enemy.transform.position = player.transform.position 
			+ Vector3.right * ATTACK_RADIUS;
		yield return null;

		// Attacks and waits a frame.
		player.MeleeAttack.Attack(player.Movement.Direction);
		yield return null;

		// Checks the enemy's health has decreased.
		Assert.AreEqual(ENEMY_HEALTH - ATTACK_DAMAGE, enemy.Health.HP);
	}

	[UnityTest]
	public IEnumerator CannotAttackDuringCooldown()
	{
		// Disables the enemy to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		// SETS UP THE CHARACTERS.
		const float ATTACK_RADIUS = 1.0f;
		const float ATTACK_DAMAGE = 1.0f;
		const float ATTACK_COOLDOWN = 0.5f;
		const float ENEMY_HEALTH = 3.0f;
		const float ENEMY_DAMAGE_GRACE_PERIOD = 0.0f;

		// Gets the character behaviours.
		CharacterBehaviour player =
			TestUtilities.GetDefaultCharactersBehaviour();

		CharacterBehaviour enemy =
			TestUtilities.GetBehaviourByCharacterName(NPC_NAME);

		Assert.NotNull(player.MeleeAttack);
		Assert.NotNull(player.Movement);
		Assert.NotNull(enemy.Health);

		// Sets the player and enemy properties.
		player.MeleeAttack.AttackRadius = ATTACK_RADIUS;
		player.MeleeAttack.AttackDamage = ATTACK_DAMAGE;
		player.MeleeAttack.AttackCooldown = ATTACK_COOLDOWN;
		enemy.Health.HP = ENEMY_HEALTH;
		enemy.Health.DamageGracePeriod = ENEMY_DAMAGE_GRACE_PERIOD;

		// Makes the character face the right.
		player.Movement.MoveRight();
		yield return null;

		// Positions the enemy within attack range.
		enemy.transform.position = player.transform.position
			+ Vector3.right * ATTACK_RADIUS;
		yield return null;


		// CHECKS THE ATTACK WORKS.
		// Attacks, waits a frame and checks the enemy's health has decreased.
		player.MeleeAttack.Attack(player.Movement.Direction);
		yield return null;
		Assert.AreEqual(ENEMY_HEALTH - ATTACK_DAMAGE, enemy.Health.HP);


		// CHECK THE COOLDOWN STOPS THE ATTACK.
		// Attacks, waits a frame and checks the enemy's health has not decreased further.
		player.MeleeAttack.Attack(player.Movement.Direction);
		yield return null;
		Assert.AreEqual(ENEMY_HEALTH - ATTACK_DAMAGE, enemy.Health.HP);


		// CHECKS THE COOLDOWN HAS EXPIRED.
		// Waits for the attack cooldown to elapse before attacking again.
		yield return new WaitForSeconds(ATTACK_COOLDOWN + 0.1f);

		// Attacks and waits a frame.
		player.MeleeAttack.Attack(player.Movement.Direction);
		yield return null;

		// Checks the enemy's health has not decreased further.
		Assert.AreEqual(ENEMY_HEALTH - ATTACK_DAMAGE * 2.0f, enemy.Health.HP);
	}

	[UnityTest]
	public IEnumerator CollidingCharactersTakeThornsDamage()
	{
		// Disables the enemy to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		// SETS UP THE CHARACTERS.
		const float HEALTH = 2.0f;
		const float DAMAGE_GRACE_PERIOD = 0.2f;
		const float ENEMY_THORNS_DAMAGE = 0.5f;

		// Gets the character behaviours.
		CharacterBehaviour player =
			TestUtilities.GetDefaultCharactersBehaviour();

		CharacterBehaviour enemy =
			TestUtilities.GetBehaviourByCharacterName(NPC_NAME);

		Assert.NotNull(player.Health);
		Assert.NotNull(enemy.MeleeAttack);

		// Sets the player and enemy properties.
		player.Health.HP = HEALTH;
		player.Health.DamageGracePeriod = DAMAGE_GRACE_PERIOD;
		enemy.MeleeAttack.ThornsDamage = ENEMY_THORNS_DAMAGE;

		// CHECKS THE PLAYER TAKES THORNS DAMAGE.
		// Positions the player on the enemy, waits half the damage grace period..
		player.transform.position = enemy.transform.position;
		yield return new WaitForSeconds(DAMAGE_GRACE_PERIOD * 0.5f);
		Assert.AreEqual(HEALTH - ENEMY_THORNS_DAMAGE, player.Health.HP);

		// Waits for the damage grace period to expire and check for additional damage.
		yield return new WaitForSeconds(DAMAGE_GRACE_PERIOD + 0.05f);
		Assert.AreEqual(HEALTH - ENEMY_THORNS_DAMAGE * 2.0f, player.Health.HP);
	}

	[UnityTest]
	public IEnumerator CharacterFreezesOnAttack()
	{
		// Disables the enemy to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		// Gets and sets up the character.
		const float ATTACK_COOLDOWN = 0.25f;

		// Gets the character behaviours.
		CharacterBehaviour player =
			TestUtilities.GetDefaultCharactersBehaviour();

		Assert.NotNull(player.MeleeAttack);
		Assert.NotNull(player.Movement);

		player.MeleeAttack.FreezeOnAttack = true;
		player.MeleeAttack.AttackCooldown = ATTACK_COOLDOWN;

		// Gets the player's position, attacks, and moves down.
		Vector3 position = player.transform.position;
		player.MeleeAttack.Attack(player.Movement.Direction);
		player.Movement.MoveDown(true);

		// Waits 0.1 seconds and checks the position is still the same.
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(position, player.transform.position);

		// Waits the length of the attack cooldown and checks the postion has
		//		now changed.
		yield return new WaitForSeconds(ATTACK_COOLDOWN);
		Assert.AreNotEqual(position, player.transform.position);
	}

	[UnityTest]
	public IEnumerator AttackInfoIsPassedThroughHealthChangedCallbacks()
	{
		// Disables the enemy behaviour to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		const float ATTACK_RADIUS = 1.0f;
		const float ATTACK_DAMAGE = 1.0f;
		const float ENEMY_HEALTH = 2.0f;

		// Gets the character behaviours.
		CharacterBehaviour player =
			TestUtilities.GetDefaultCharactersBehaviour();

		CharacterBehaviour enemy =
			TestUtilities.GetBehaviourByCharacterName(NPC_NAME);

		Assert.NotNull(player.MeleeAttack);
		Assert.NotNull(enemy.Health);

		// Sets the attack radius, attack damage, and enemy health.
		player.MeleeAttack.AttackRadius = ATTACK_RADIUS;
		player.MeleeAttack.AttackDamage = ATTACK_DAMAGE;
		enemy.Health.HP = ENEMY_HEALTH;

		// Initialises the checker variables.
		_damageInfo = null;

		// Assigns the attack info to the melee attack.
		const string KEY_NAME = "test_key";
		const string VALUE_NAME = "test_value";

		Dictionary<string, string> attackInfo = new Dictionary<string, string>();
		attackInfo.Add(KEY_NAME, VALUE_NAME);

		player.MeleeAttack.AttackInfo = attackInfo;

		// Assigns the callback.
		enemy.Health.HealthChangedCallbacks.Add(HealthChangedCallback);

		// Positions the enemy.
		enemy.transform.position = player.transform.position
			+ Vector3.right * ATTACK_RADIUS;
		yield return null;

		// Attacks and waits a frame.
		player.MeleeAttack.Attack(Vector2.right);
		yield return null;

		// Checks the damage info was passed through correctly.
		Assert.NotNull(_damageInfo);
		Assert.AreEqual(VALUE_NAME, _damageInfo[KEY_NAME]);
	}

	[UnityTest]
	public IEnumerator AttackInfoIsPassedThroughDeathCallbacks()
	{
		// Disables the enemy behaviour to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		const float ATTACK_RADIUS = 1.0f;
		const float ENEMY_HEALTH = 2.0f;

		// Gets the character behaviours.
		CharacterBehaviour player =
			TestUtilities.GetDefaultCharactersBehaviour();

		CharacterBehaviour enemy =
			TestUtilities.GetBehaviourByCharacterName(NPC_NAME);

		Assert.NotNull(player.MeleeAttack);
		Assert.NotNull(enemy.Health);

		// Sets the attack radius, attack damage, and enemy health.
		player.MeleeAttack.AttackRadius = ATTACK_RADIUS;
		player.MeleeAttack.AttackDamage = ENEMY_HEALTH;
		enemy.Health.HP = ENEMY_HEALTH;

		// Initialises the checker variables.
		_damageInfo = null;

		// Assigns the attack info to the melee attack.
		const string KEY_NAME = "test_key";
		const string VALUE_NAME = "test_value";

		Dictionary<string, string> attackInfo = new Dictionary<string, string>();
		attackInfo.Add(KEY_NAME, VALUE_NAME);

		player.MeleeAttack.AttackInfo = attackInfo;

		// Assigns the callback.
		enemy.Health.DeathCallbacks.Add(DeathCallback);

		// Positions the enemy.
		enemy.transform.position = player.transform.position
			+ Vector3.right * ATTACK_RADIUS;
		yield return null;

		// Attacks and waits a frame.
		player.MeleeAttack.Attack(Vector2.right);
		yield return null;

		// Checks the damage info was passed through correctly.
		Assert.NotNull(_damageInfo);
		Assert.AreEqual(VALUE_NAME, _damageInfo[KEY_NAME]);
	}

	[UnityTest]
	public IEnumerator AttackInfoIsPassedThroughWithThornsDamage()
	{
		// Disables the enemy to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		// SETS UP THE CHARACTERS.
		const float HEALTH = 2.0f;
		const float DAMAGE_GRACE_PERIOD = 0.2f;
		const float ENEMY_THORNS_DAMAGE = 0.5f;

		// Gets the character behaviours.
		CharacterBehaviour player =
			TestUtilities.GetDefaultCharactersBehaviour();

		CharacterBehaviour enemy =
			TestUtilities.GetBehaviourByCharacterName(NPC_NAME);

		Assert.NotNull(player.Health);
		Assert.NotNull(enemy.MeleeAttack);

		// Sets the player and enemy properties.
		player.Health.HP = HEALTH;
		player.Health.DamageGracePeriod = DAMAGE_GRACE_PERIOD;
		enemy.MeleeAttack.ThornsDamage = ENEMY_THORNS_DAMAGE;

		// Initialises the checker variables.
		_damageInfo = null;

		// Assigns the attack info to the melee attack.
		const string KEY_NAME = "test_key";
		const string VALUE_NAME = "test_value";

		Dictionary<string, string> attackInfo = new Dictionary<string, string>();
		attackInfo.Add(KEY_NAME, VALUE_NAME);

		enemy.MeleeAttack.AttackInfo = attackInfo;

		// Assigns the callback.
		player.Health.HealthChangedCallbacks.Add(HealthChangedCallback);

		// CHECKS THE PLAYER TAKES THORNS DAMAGE.
		// Positions the player on the enemy, waits half the damage grace period..
		player.transform.position = enemy.transform.position;
		yield return new WaitForSeconds(DAMAGE_GRACE_PERIOD * 0.5f);
		Assert.AreEqual(HEALTH - ENEMY_THORNS_DAMAGE, player.Health.HP);

		// Checks the damage info was passed through correctly.
		Assert.NotNull(_damageInfo);
		Assert.AreEqual(VALUE_NAME, _damageInfo[KEY_NAME]);
	}
}