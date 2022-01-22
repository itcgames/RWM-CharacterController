using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class AnimationTests
{
	// Animation names.
	private const string IDLE_LEFT = "IdleLeft";
	private const string IDLE_RIGHT = "IdleRight";
	private const string IDLE_UP = "IdleUp";
	private const string IDLE_DOWN = "IdleDown";

	private const string MOVING_LEFT = "MovingLeft";
	private const string MOVING_RIGHT = "MovingRight";
	private const string MOVING_UP = "MovingUp";
	private const string MOVING_DOWN = "MovingDown";

	private const string ATTACK_LEFT = "AttackLeft";
	private const string ATTACK_RIGHT = "AttackRight";
	private const string ATTACK_UP = "AttackUp";
	private const string ATTACK_DOWN = "AttackDown";

	private const string ENEMY_MOVING_LEFT = "EnemyMovingLeft";
	private const string ENEMY_MOVING_RIGHT = "EnemyMovingRight";
	private const string ENEMY_MOVING_UP = "EnemyMovingUp";
	private const string ENEMY_MOVING_DOWN = "EnemyMovingDown";

	private const string DEFAULT_ANIMATION = IDLE_DOWN;

	private const string ENEMY_CHARACTER = "Enemy";

	[SetUp]
	public void Setup()
	{
		SceneManager.LoadScene(TestUtilities.GetDefaultSceneName());
	}

	[UnityTest]
	public IEnumerator CorrectDefaultAnimation()
	{
		var player = TestUtilities.GetDefaultCharacter();

		// Checks the default animation is correct.
		Assert.AreEqual(DEFAULT_ANIMATION, GetCurrentClipName(player.Animator));

		yield return null;
	}

	[UnityTest]
	public IEnumerator CharacterFacesLeft()
	{
		// Gets the player and moves it to the left.
		var player = TestUtilities.GetDefaultCharacter();
		player.MoveLeft();
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(IDLE_LEFT, GetCurrentClipName(player.Animator));
	}

	[UnityTest]
	public IEnumerator CharacterFacesRight()
	{
		// Gets the player and moves it to the right.
		var player = TestUtilities.GetDefaultCharacter();
		player.MoveRight();
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(IDLE_RIGHT, GetCurrentClipName(player.Animator));
	}

	[UnityTest]
	public IEnumerator CharacterFacesUp()
	{
		// Gets the player and moves it upwards.
		var player = TestUtilities.GetDefaultCharacter();
		player.MoveUp();
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(IDLE_UP, GetCurrentClipName(player.Animator));
	}

	[UnityTest]
	public IEnumerator CharacterFacesDown()
	{
		// The default animation is facing downwards, so we change the
		//		animation first.
		// This test assumes the CharacterFacesUp test passed.

		// Gets the player and moves it upwards.
		var player = TestUtilities.GetDefaultCharacter();
		player.MoveUp();
		yield return null;

		// Moves the player down and waits.
		player.MoveDown();
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(IDLE_DOWN, GetCurrentClipName(player.Animator));
	}

	[UnityTest]
	public IEnumerator TilebasedCharacterFacesLeft()
	{
		// Gets the player and moves it to the left.
		var player = TestUtilities.GetDefaultCharacter();
		player.TilebasedMovement = true;
		player.SecondsPerTile = 0.1f;

		player.MoveLeft();
		yield return new WaitForSeconds(0.2f);
		Assert.AreEqual(IDLE_LEFT, GetCurrentClipName(player.Animator));
	}

	[UnityTest]
	public IEnumerator TilebasedCharacterFacesRight()
	{
		// Gets the player and moves it to the right.
		var player = TestUtilities.GetDefaultCharacter();
		player.TilebasedMovement = true;
		player.SecondsPerTile = 0.1f;

		player.MoveRight();
		yield return new WaitForSeconds(0.2f);
		Assert.AreEqual(IDLE_RIGHT, GetCurrentClipName(player.Animator));
	}

	[UnityTest]
	public IEnumerator TilebasedCharacterFacesUp()
	{
		// Gets the player and moves it upwards.
		var player = TestUtilities.GetDefaultCharacter();
		player.TilebasedMovement = true;
		player.SecondsPerTile = 0.1f;

		player.MoveUp();
		yield return new WaitForSeconds(0.2f);
		Assert.AreEqual(IDLE_UP, GetCurrentClipName(player.Animator));
	}

	[UnityTest]
	public IEnumerator TilebasedCharacterFacesDown()
	{
		// The default animation is facing downwards, so we change the
		//		animation first.
		// This test assumes the CharacterFacesUp test passed.

		// Gets the player and moves it upwards.
		var player = TestUtilities.GetDefaultCharacter();
		player.TilebasedMovement = true;
		player.SecondsPerTile = 0.1f;

		player.MoveUp();
		yield return new WaitForSeconds(0.2f);

		// Moves the player down and waits.
		player.MoveDown();
		yield return new WaitForSeconds(0.2f);
		Assert.AreEqual(IDLE_DOWN, GetCurrentClipName(player.Animator));
	}

	[UnityTest]
	public IEnumerator CharacterMovementAnimatesLeft()
	{
		var player = TestUtilities.GetDefaultCharacter();
		player.MoveLeft(true);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(MOVING_LEFT, GetCurrentClipName(player.Animator));
	}

	[UnityTest]
	public IEnumerator CharacterMovementAnimatesRight()
	{
		var player = TestUtilities.GetDefaultCharacter();
		player.MoveRight(true);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(MOVING_RIGHT, GetCurrentClipName(player.Animator));
	}

	[UnityTest]
	public IEnumerator CharacterMovementAnimatesUp()
	{
		var player = TestUtilities.GetDefaultCharacter();
		player.MoveUp(true);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(MOVING_UP, GetCurrentClipName(player.Animator));
	}

	[UnityTest]
	public IEnumerator CharacterMovementAnimatesDown()
	{
		var player = TestUtilities.GetDefaultCharacter();
		player.MoveDown(true);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(MOVING_DOWN, GetCurrentClipName(player.Animator));
	}

	[UnityTest]
	public IEnumerator TilebasedCharacterMovementAnimatesLeft()
	{
		var player = TestUtilities.GetDefaultCharacter();
		player.TilebasedMovement = true;
		player.MoveLeft(true);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(MOVING_LEFT, GetCurrentClipName(player.Animator));
	}

	[UnityTest]
	public IEnumerator TilebasedCharacterMovementAnimatesRight()
	{
		var player = TestUtilities.GetDefaultCharacter();
		player.TilebasedMovement = true;
		player.MoveRight(true);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(MOVING_RIGHT, GetCurrentClipName(player.Animator));
	}

	[UnityTest]
	public IEnumerator TilebasedCharacterMovementAnimatesUp()
	{
		var player = TestUtilities.GetDefaultCharacter();
		player.TilebasedMovement = true;
		player.MoveUp(true);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(MOVING_UP, GetCurrentClipName(player.Animator));
	}

	[UnityTest]
	public IEnumerator TilebasedCharacterMovementAnimatesDown()
	{
		var player = TestUtilities.GetDefaultCharacter();
		player.TilebasedMovement = true;
		player.MoveDown(true);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(MOVING_DOWN, GetCurrentClipName(player.Animator));
	}

	[UnityTest]
	public IEnumerator MeleeAttackLeftAnimates()
	{
		var player = TestUtilities.GetDefaultCharacter();
		player.AttackCooldown = 1.0f;
		player.MoveLeft();
		player.Attack();
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(ATTACK_LEFT, GetCurrentClipName(player.Animator));
	}

	[UnityTest]
	public IEnumerator MeleeAttackRightAnimates()
	{
		var player = TestUtilities.GetDefaultCharacter();
		player.AttackCooldown = 1.0f;
		player.MoveRight();
		player.Attack();
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(ATTACK_RIGHT, GetCurrentClipName(player.Animator));
	}

	[UnityTest]
	public IEnumerator MeleeAttackUpwardAnimates()
	{
		var player = TestUtilities.GetDefaultCharacter();
		player.AttackCooldown = 1.0f;
		player.MoveUp();
		player.Attack();
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(ATTACK_UP, GetCurrentClipName(player.Animator));
	}

	[UnityTest]
	public IEnumerator MeleeAttackDownwardAnimates()
	{
		var player = TestUtilities.GetDefaultCharacter();
		player.AttackCooldown = 1.0f;
		player.MoveDown();
		player.Attack();
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(ATTACK_DOWN, GetCurrentClipName(player.Animator));
	}

	[UnityTest]
	public IEnumerator TilebasedMeleeAttackAnimates()
	{
		var player = TestUtilities.GetDefaultCharacter();
		player.TilebasedMovement = true;
		player.AttackCooldown = 1.0f;
		player.MoveDown();
		player.Attack();
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(ATTACK_DOWN, GetCurrentClipName(player.Animator));
	}

	[UnityTest]
	public IEnumerator ReturnsToIdleAfterMelee()
	{
		var player = TestUtilities.GetDefaultCharacter();
		player.AttackCooldown = 0.1f;
		player.MoveDown();
		player.Attack();
		yield return new WaitForSeconds(0.2f);
		Assert.AreEqual(IDLE_DOWN, GetCurrentClipName(player.Animator));
	}

	[UnityTest]
	public IEnumerator CustomAnimationsFunctionCorrectly()
	{
		var enemy = TestUtilities.GetCharacterByName(ENEMY_CHARACTER);

		// Checks left movement.
		enemy.MoveLeft(true);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(ENEMY_MOVING_LEFT, GetCurrentClipName(enemy.Animator));

		// Checks right movement.
		enemy.ClearPersistentInput();
		enemy.MoveRight(true);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(ENEMY_MOVING_RIGHT, GetCurrentClipName(enemy.Animator));

		// Checks upward movement.
		enemy.ClearPersistentInput();
		enemy.MoveUp(true);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(ENEMY_MOVING_UP, GetCurrentClipName(enemy.Animator));

		// Checks downward movement.
		enemy.ClearPersistentInput();
		enemy.MoveDown(true);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(ENEMY_MOVING_DOWN, GetCurrentClipName(enemy.Animator));
	}

	private string GetCurrentClipName(Animator animator)
    {
		var info = animator.GetCurrentAnimatorClipInfo(0);
		Assert.IsNotEmpty(info);
		return info[0].clip.name;
	}
}