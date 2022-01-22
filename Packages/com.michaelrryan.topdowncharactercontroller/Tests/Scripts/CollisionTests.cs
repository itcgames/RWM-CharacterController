using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;


public class CollisionTests
{
	private const string ENEMY_NAME = "Enemy";

	[SetUp]
	public void Setup()
	{
		SceneManager.LoadScene(TestUtilities.GetDefaultSceneName());
	}

	[UnityTest]
	public IEnumerator CharacterIsBlockedByWalls()
	{
		var player = TestUtilities.GetDefaultCharacter();

		// Positions the player by the bottom wall and sets its properties.
		player.transform.position = new Vector3(0.0f, -3.0f);
		player.MaxSpeed = 20.0f; // 20 Unity units per second.
		player.TimeToMaxSpeed = 0.0f;

		// Moves down and checks the player has been blocked.
		player.MoveDown(true);
		yield return new WaitForSeconds(0.1f);
		Assert.Greater(player.transform.position.y, -3.5f);
	}

	[UnityTest]
	public IEnumerator CharacterIsBlockedByCharacters()
	{
		var player = TestUtilities.GetDefaultCharacter();
		var enemy = TestUtilities.GetCharacterByName(ENEMY_NAME);

		// Positions the player by the enemy and sets its properties.
		player.transform.position = enemy.transform.position + Vector3.right;
		player.MaxSpeed = 20.0f; // 20 Unity units per second.
		player.TimeToMaxSpeed = 0.0f;

		// Moves left into the enemy and checks the player has been blocked.
		player.MoveLeft(true);
		yield return new WaitForSeconds(0.1f);
		Assert.Greater(player.transform.position.x, enemy.transform.position.x);
	}

	[UnityTest]
	public IEnumerator CollidingEnemyIsUnmoved()
	{
		var player = TestUtilities.GetDefaultCharacter();
		var enemy = TestUtilities.GetCharacterByName(ENEMY_NAME);

		// Positions the player by the enemy and sets its properties.
		player.transform.position = enemy.transform.position + Vector3.right;
		player.MaxSpeed = 20.0f; // 20 Unity units per second.
		player.TimeToMaxSpeed = 0.0f;

		// Take a copy of the enemy's position before collision.
		Vector3 enemyPos = enemy.transform.position;

		// Moves left into the enemy and checks the enemy has not moved.
		player.MoveLeft(true);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(enemyPos, enemy.transform.position);
	}
}
