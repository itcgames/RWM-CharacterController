using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using TopdownCharacterController;

public class CollisionTests
{
	private const string ENEMY_NAME = "Enemy";
	private const string PROJECTILE_NAME = "BasicProjectile(Clone)";

	// How much extra time to add when waiting for a specific length of time
	//		to elapse to avoid falling under on different powered machines.
	const float SAFETY_MARGIN = 0.1f;

	[UnityTest]
	public IEnumerator CharacterIsBlockedByWalls()
	{
		SceneManager.LoadScene(TestUtilities.GetDefaultSceneName());
		yield return null;

		// Disables the enemy to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
		Assert.NotNull(player.TopdownMovement);

		// Positions the player by the bottom wall and sets its properties.
		player.transform.position = new Vector3(0.0f, -3.0f);
		player.TopdownMovement.MaxSpeed = 20.0f; // 20 Unity units per second.
		player.TopdownMovement.TimeToMaxSpeed = 0.0f;

		// Moves down and checks the player has been blocked.
		player.Movement.MoveDown(true);
		yield return new WaitForSeconds(0.1f);
		Assert.Greater(player.transform.position.y, -3.5f);
	}

	[UnityTest]
	public IEnumerator CharacterIsBlockedByCharacters()
	{
		SceneManager.LoadScene(TestUtilities.GetDefaultSceneName());
		yield return null;

		// Disables the enemy to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
		CharacterBehaviour enemy = TestUtilities.GetBehaviourByCharacterName(ENEMY_NAME);
		Assert.NotNull(player.TopdownMovement);

		// Positions the player by the enemy and sets its properties.
		player.transform.position = enemy.transform.position + Vector3.right;
		player.TopdownMovement.MaxSpeed = 20.0f; // 20 Unity units per second.
		player.TopdownMovement.TimeToMaxSpeed = 0.0f;

		// Moves left into the enemy and checks the player has been blocked.
		player.Movement.MoveLeft(true);
		yield return new WaitForSeconds(0.1f);
		Assert.Greater(player.transform.position.x, enemy.transform.position.x);
	}

	[UnityTest]
	public IEnumerator CollidingEnemyIsUnmoved()
	{
		SceneManager.LoadScene(TestUtilities.GetDefaultSceneName());
		yield return null;

		// Disables the enemy to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
		CharacterBehaviour enemy = TestUtilities.GetBehaviourByCharacterName(ENEMY_NAME);
		Assert.NotNull(player.TopdownMovement);

		// Positions the player by the enemy and sets its properties.
		player.transform.position = enemy.transform.position + Vector3.right;
		player.TopdownMovement.MaxSpeed = 20.0f; // 20 Unity units per second.
		player.TopdownMovement.TimeToMaxSpeed = 0.0f;

		// Take a copy of the enemy's position before collision.
		Vector3 enemyPos = enemy.transform.position;

		// Moves left into the enemy and checks the enemy has not moved.
		player.Movement.MoveLeft(true);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(enemyPos, enemy.transform.position);
	}

	[UnityTest]
	public IEnumerator ProjectilesDontCollideWithShooter()
	{
		SceneManager.LoadScene(TestUtilities.GetDefaultSceneName());
		yield return null;

		// Disables the enemy to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
		RangedAttack ranged = player.GetComponent<RangedAttack>();

		// Takes a copy of the player's position before firing.
		Vector3 playerPos = player.transform.position;

		// Fires a projectile.
		ranged.Fire(Vector2.left);
		yield return new WaitForSeconds(0.1f);

		// Checks the player has not moved and projectile still exists.
		Assert.AreEqual(playerPos, player.transform.position);
		Assert.NotNull(GameObject.Find(PROJECTILE_NAME));
	}

	[UnityTest]
	public IEnumerator CharactersDontMoveToBlockedTiles()
	{
		SceneManager.LoadScene(TestUtilities.TILEBASED_SCENE_NAME);
		yield return null;

		CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
		Assert.NotNull(player.TilebasedMovement);

		player.TilebasedMovement.SecondsPerTile = 0.1f;

		// Player should be blocked after the first movement.
		player.Movement.MoveRight();
		yield return new WaitForSeconds(player.TilebasedMovement.SecondsPerTile + SAFETY_MARGIN);

		// Gets the position of the player before trying to move.
		Vector3 playerPos = player.transform.position;

		// Try to move and make sure the player was blocked.
		player.Movement.MoveRight();
		yield return new WaitForSeconds(player.TilebasedMovement.SecondsPerTile + SAFETY_MARGIN);
		Assert.AreEqual(playerPos, player.transform.position);

		// If the character ignores blocked tiles, we should be able to move
		//		right after being blocked.
		player.Movement.MoveRight();
		yield return null;
		player.Movement.MoveUp();
		yield return new WaitForSeconds(player.TilebasedMovement.SecondsPerTile + SAFETY_MARGIN);
		Assert.AreNotEqual(playerPos, player.transform.position);
	}
}
