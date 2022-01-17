using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class RangedAttackTests
{
	const string PROJECTILE_NAME = "BasicProjectile(Clone)";
	const string NPC_NAME = "Enemy";

	// How much extra time to add when waiting for a specific length of time
	//		to elapse to avoid falling under on different powered machines.
	const float SAFETY_MARGIN = 0.1f;

	[SetUp]
	public void Setup()
	{
		SceneManager.LoadScene(TestUtilities.GetDefaultSceneName());
	}

	[UnityTest]
	public IEnumerator ProjectileFires()
	{
		// Gets the player character.
		TopdownCharacterController player =
			TestUtilities.GetDefaultCharacter();

		// Gets the ranged attack component and checks it's not null.
		TopdownRangedAttack rangedAttack =
			player.GetComponent<TopdownRangedAttack>();
		Assert.NotNull(rangedAttack);

		// Fires a projectile.
		GameObject fired = rangedAttack.Fire(player.Direction);

		// Ensures the projectile and player share the same position.
		Assert.AreEqual(player.transform.position, fired.transform.position);

		// Waits a frame.
		yield return null;

		// Ensures the projectile exists and matches the projectile returned.
		GameObject projectile = GameObject.Find(PROJECTILE_NAME);
		Assert.NotNull(projectile);
		Assert.AreSame(fired, projectile);
	}

	[UnityTest]
	public IEnumerator CannotFireDuringCooldown()
	{
		// Gets the player character.
		TopdownCharacterController player =
			TestUtilities.GetDefaultCharacter();

		// Gets the ranged attack component and checks it's not null.
		TopdownRangedAttack rangedAttack =
			player.GetComponent<TopdownRangedAttack>();
		Assert.NotNull(rangedAttack);

		// Fires a projectile and checks it's not null.
		GameObject fired = rangedAttack.Fire(player.Direction);
		Assert.NotNull(fired);

		// Tries to fire another projectile and ensures it's null.
		fired = rangedAttack.Fire(player.Direction);
		Assert.Null(fired);

		// Waits until the cooldown expires and fire again.
		yield return new WaitForSeconds(rangedAttack.Cooldown + SAFETY_MARGIN);
		fired = rangedAttack.Fire(player.Direction);
		Assert.NotNull(fired);
	}

	[UnityTest]
	public IEnumerator DefaultProjectileCausesDamages()
	{
		const float PROJECTILE_DISTANCE = 5.0f;

		// Gets the player character.
		TopdownCharacterController player =
			TestUtilities.GetDefaultCharacter();

		// Gets the enemy character.
		var enemy = TestUtilities.GetCharacterByName(NPC_NAME);

		// Gets the ranged attack component and checks it's not null.
		TopdownRangedAttack rangedAttack =
			player.GetComponent<TopdownRangedAttack>();
		Assert.NotNull(rangedAttack);

		// Positions the player to the left of the enemy.
		player.transform.position = 
			enemy.transform.position + Vector3.left * PROJECTILE_DISTANCE;

		// Gets the projectile component from the fired projectile.
		GameObject projectileObj = rangedAttack.Fire(Vector2.right);
		Projectile projectile = projectileObj.GetComponent<Projectile>();

		// Gets the enemies health and waits for the bullet to hit.
		float enemyHealth = enemy.Health;
		yield return new WaitForSeconds((PROJECTILE_DISTANCE / projectile.Speed) 
			+ SAFETY_MARGIN);

		// Checks that the enemy took damage and the projectile no longer exists.
		Assert.AreEqual(enemyHealth - projectile.Damage, enemy.Health);
		Assert.IsNull(GameObject.Find(PROJECTILE_NAME));
	}
}