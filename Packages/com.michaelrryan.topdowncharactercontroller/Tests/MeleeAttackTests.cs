using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class MeleeAttackTests
{
	const string NPC_NAME = "Enemy";

	[SetUp]
	public void Setup()
	{
		SceneManager.LoadScene(TestUtilities.GetDefaultSceneName());
	}

	[UnityTest]
	public IEnumerator AttackDamagesCharactersInARadius()
	{
		const float ATTACK_RADIUS = 1.0f;
		const float ATTACK_DAMAGE = 1.0f;
		const float ENEMY_HEALTH = 2.0f;

		// Gets the characters.
		var character = TestUtilities.GetDefaultCharacter();
		var enemy = TestUtilities.GetCharacterByName(NPC_NAME);

		// Sets the attack radius, attack damage, and enemy health.
		character.AttackRadius = ATTACK_RADIUS;
		character.AttackDamage = ATTACK_DAMAGE;
		enemy.Health = ENEMY_HEALTH;

		// Makes the character face the right.
		character.MoveRight();
		yield return null;

		// Positions the enemy.
		enemy.transform.position = character.transform.position 
			+ Vector3.right * ATTACK_RADIUS;
		yield return null;
		
		// Attacks and waits a frame.
		character.Attack();
		yield return null;

		// Checks the enemy's health has decreased.
		Assert.AreEqual(ENEMY_HEALTH - ATTACK_DAMAGE, enemy.Health);
	}
}