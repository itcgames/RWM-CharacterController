using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class RangedAttackTests
{
	const string PROJECTILE_NAME = "BasicProjectile(Clone)";

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
		const float SAFETY_MARGIN = 0.1f;

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
}