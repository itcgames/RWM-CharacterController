using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using TopdownCharacterController;

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
		// Gets the player and its animator.
		CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
		Animator animator = player.GetComponent<Animator>();
		Assert.NotNull(animator);

		// Checks the default animation is correct.
		Assert.AreEqual(DEFAULT_ANIMATION, TestUtilities.GetCurrentClipName(animator));

		yield return null;
	}

	[UnityTest]
	public IEnumerator CharacterFacesLeft()
	{
		// Gets the player and its animator.
		CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
		Animator animator = player.GetComponent<Animator>();
		Assert.NotNull(animator);
		Assert.NotNull(player.Movement);

		// Moves the player to the left.
		player.Movement.MoveLeft();
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(IDLE_LEFT, TestUtilities.GetCurrentClipName(animator));
	}

	[UnityTest]
	public IEnumerator CharacterFacesRight()
	{
		// Gets the player and its animator.
		CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
		Animator animator = player.GetComponent<Animator>();
		Assert.NotNull(animator);
		Assert.NotNull(player.Movement);

		player.Movement.MoveRight();
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(IDLE_RIGHT, TestUtilities.GetCurrentClipName(animator));
	}

	[UnityTest]
	public IEnumerator CharacterFacesUp()
	{
		// Gets the player and its animator.
		CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
		Animator animator = player.GetComponent<Animator>();
		Assert.NotNull(animator);
		Assert.NotNull(player.Movement);

		player.Movement.MoveUp();
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(IDLE_UP, TestUtilities.GetCurrentClipName(animator));
	}

	[UnityTest]
	public IEnumerator CharacterFacesDown()
	{
		// The default animation is facing downwards, so we change the
		//		animation first.
		// This test assumes the CharacterFacesUp test passed.

		// Gets the player and its animator.
		CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
		Animator animator = player.GetComponent<Animator>();
		Assert.NotNull(animator);
		Assert.NotNull(player.Movement);

		player.Movement.MoveUp();
		yield return null;

		// Moves the player down and waits.
		player.Movement.MoveDown();
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(IDLE_DOWN, TestUtilities.GetCurrentClipName(animator));
	}

	[UnityTest]
	public IEnumerator CharacterMovementAnimatesLeft()
	{
		// Gets the player and its animator.
		CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
		Animator animator = player.GetComponent<Animator>();
		Assert.NotNull(animator);
		Assert.NotNull(player.Movement);

		player.Movement.MoveLeft(true);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(MOVING_LEFT, TestUtilities.GetCurrentClipName(animator));
	}

	[UnityTest]
	public IEnumerator CharacterMovementAnimatesRight()
	{
		// Gets the player and its animator.
		CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
		Animator animator = player.GetComponent<Animator>();
		Assert.NotNull(animator);
		Assert.NotNull(player.Movement);

		player.Movement.MoveRight(true);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(MOVING_RIGHT, TestUtilities.GetCurrentClipName(animator));
	}

	[UnityTest]
	public IEnumerator CharacterMovementAnimatesUp()
	{
		// Gets the player and its animator.
		CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
		Animator animator = player.GetComponent<Animator>();
		Assert.NotNull(animator);
		Assert.NotNull(player.Movement);

		player.Movement.MoveUp(true);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(MOVING_UP, TestUtilities.GetCurrentClipName(animator));
	}

	[UnityTest]
	public IEnumerator CharacterMovementAnimatesDown()
	{
		// Gets the player and its animator.
		CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
		Animator animator = player.GetComponent<Animator>();
		Assert.NotNull(animator);
		Assert.NotNull(player.Movement);

		player.Movement.MoveDown(true);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(MOVING_DOWN, TestUtilities.GetCurrentClipName(animator));
	}

	[UnityTest]
	public IEnumerator MeleeAttackLeftAnimates()
	{
		CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
		Animator animator = player.GetComponent<Animator>();
		Assert.NotNull(animator);
		Assert.NotNull(player.MeleeAttack);

		player.MeleeAttack.AttackCooldown = 1.0f;

		player.Movement.MoveLeft();
		player.MeleeAttack.Attack(player.Movement.Direction);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(ATTACK_LEFT, TestUtilities.GetCurrentClipName(animator));
	}

	[UnityTest]
	public IEnumerator MeleeAttackRightAnimates()
	{
		CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
		Animator animator = player.GetComponent<Animator>();
		Assert.NotNull(animator);
		Assert.NotNull(player.MeleeAttack);

		player.MeleeAttack.AttackCooldown = 1.0f;

		player.Movement.MoveRight();
		player.MeleeAttack.Attack(player.Movement.Direction);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(ATTACK_RIGHT, TestUtilities.GetCurrentClipName(animator));
	}

	[UnityTest]
	public IEnumerator MeleeAttackUpwardAnimates()
	{
		CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
		Animator animator = player.GetComponent<Animator>();
		Assert.NotNull(animator);
		Assert.NotNull(player.MeleeAttack);

		player.MeleeAttack.AttackCooldown = 1.0f;

		player.Movement.MoveUp();
		player.MeleeAttack.Attack(player.Movement.Direction);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(ATTACK_UP, TestUtilities.GetCurrentClipName(animator));
	}

	[UnityTest]
	public IEnumerator MeleeAttackDownwardAnimates()
	{
		CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
		Animator animator = player.GetComponent<Animator>();
		Assert.NotNull(animator);
		Assert.NotNull(player.MeleeAttack);

		player.MeleeAttack.AttackCooldown = 1.0f;

		player.Movement.MoveDown();
		player.MeleeAttack.Attack(player.Movement.Direction);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(ATTACK_DOWN, TestUtilities.GetCurrentClipName(animator));
	}

	[UnityTest]
	public IEnumerator ReturnsToIdleAfterMelee()
	{
		CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
		Animator animator = player.GetComponent<Animator>();
		Assert.NotNull(animator);
		Assert.NotNull(player.MeleeAttack);

		player.MeleeAttack.AttackCooldown = 0.1f;

		player.Movement.MoveDown();
		player.MeleeAttack.Attack(player.Movement.Direction);
		yield return new WaitForSeconds(0.2f);
		Assert.AreEqual(IDLE_DOWN, TestUtilities.GetCurrentClipName(animator));
	}

	[UnityTest]
	public IEnumerator CustomAnimationsFunctionCorrectly()
	{
		var enemy = TestUtilities.GetBehaviourByCharacterName(ENEMY_CHARACTER);
		Animator animator = enemy.GetComponent<Animator>();
		Assert.NotNull(animator);
		Assert.NotNull(enemy.Movement);

		// Checks left movement.
		enemy.Movement.MoveLeft(true);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(ENEMY_MOVING_LEFT, TestUtilities.GetCurrentClipName(animator));

		// Checks right movement.
		enemy.Movement.ClearPersistentInput();
		enemy.Movement.MoveRight(true);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(ENEMY_MOVING_RIGHT, TestUtilities.GetCurrentClipName(animator));

		// Checks upward movement.
		enemy.Movement.ClearPersistentInput();
		enemy.Movement.MoveUp(true);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(ENEMY_MOVING_UP, TestUtilities.GetCurrentClipName(animator));

		// Checks downward movement.
		enemy.Movement.ClearPersistentInput();
		enemy.Movement.MoveDown(true);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(ENEMY_MOVING_DOWN, TestUtilities.GetCurrentClipName(animator));
	}
}