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

	[UnityTest]
	public IEnumerator CannotAttackDuringCooldown()
	{
		// SETS UP THE CHARACTERS.
		const float ATTACK_RADIUS = 1.0f;
		const float ATTACK_DAMAGE = 1.0f;
		const float ATTACK_COOLDOWN = 0.5f;
		const float ENEMY_HEALTH = 3.0f;
		const float ENEMY_DAMAGE_GRACE_PERIOD = 0.0f;

		// Gets the characters.
		var character = TestUtilities.GetDefaultCharacter();
		var enemy = TestUtilities.GetCharacterByName(NPC_NAME);

		// Sets the player and enemy properties.
		character.AttackRadius = ATTACK_RADIUS;
		character.AttackDamage = ATTACK_DAMAGE;
		character.AttackCooldown = ATTACK_COOLDOWN;
		enemy.Health = ENEMY_HEALTH;
		enemy.DamageGracePeriod = ENEMY_DAMAGE_GRACE_PERIOD;

		// Makes the character face the right.
		character.MoveRight();
		yield return null;

		// Positions the enemy within attack range.
		enemy.transform.position = character.transform.position
			+ Vector3.right * ATTACK_RADIUS;
		yield return null;


		// CHECKS THE ATTACK WORKS.
		// Attacks, waits a frame and checks the enemy's health has decreased.
		character.Attack();
		yield return null;
		Assert.AreEqual(ENEMY_HEALTH - ATTACK_DAMAGE, enemy.Health);


		// CHECK THE COOLDOWN STOPS THE ATTACK.
		// Attacks, waits a frame and checks the enemy's health has not decreased further.
		character.Attack();
		yield return null;
		Assert.AreEqual(ENEMY_HEALTH - ATTACK_DAMAGE, enemy.Health);


		// CHECKS THE COOLDOWN HAS EXPIRED.
		// Waits for the attack cooldown to elapse before attacking again.
		yield return new WaitForSeconds(ATTACK_COOLDOWN + 0.1f);

		// Attacks and waits a frame.
		character.Attack();
		yield return null;

		// Checks the enemy's health has not decreased further.
		Assert.AreEqual(ENEMY_HEALTH - ATTACK_DAMAGE * 2.0f, enemy.Health);
	}
}