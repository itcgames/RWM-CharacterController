using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using TopdownCharacterController;

public class TilebasedAnimationTests
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

	private const string ATTACK_DOWN = "AttackDown";

    [SetUp]
    public void Setup()
    {
        SceneManager.LoadScene(TestUtilities.TILEBASED_SCENE_NAME);
    }

	[UnityTest]
	public IEnumerator TilebasedCharacterFacesLeft()
	{
		// Gets the player and its animator.
		CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
		Animator animator = player.GetComponent<Animator>();
		Assert.NotNull(animator);
		Assert.NotNull(player.TilebasedMovement);

		player.TilebasedMovement.SecondsPerTile = 0.1f;

		player.Movement.MoveLeft();
		yield return new WaitForSeconds(0.2f);
		Assert.AreEqual(IDLE_LEFT, TestUtilities.GetCurrentClipName(animator));
	}

	[UnityTest]
	public IEnumerator TilebasedCharacterFacesRight()
	{
		// Gets the player and its animator.
		CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
		Animator animator = player.GetComponent<Animator>();
		Assert.NotNull(animator);
		Assert.NotNull(player.TilebasedMovement);

		player.TilebasedMovement.SecondsPerTile = 0.1f;

		player.Movement.MoveRight();
		yield return new WaitForSeconds(0.2f);
		Assert.AreEqual(IDLE_RIGHT, TestUtilities.GetCurrentClipName(animator));
	}

	[UnityTest]
	public IEnumerator TilebasedCharacterFacesUp()
	{
		// Gets the player and its animator.
		CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
		Animator animator = player.GetComponent<Animator>();
		Assert.NotNull(animator);
		Assert.NotNull(player.TilebasedMovement);

		player.TilebasedMovement.SecondsPerTile = 0.1f;

		player.Movement.MoveUp();
		yield return new WaitForSeconds(0.2f);
		Assert.AreEqual(IDLE_UP, TestUtilities.GetCurrentClipName(animator));
	}

	[UnityTest]
	public IEnumerator TilebasedCharacterFacesDown()
	{
		// The default animation is facing downwards, so we change the
		//		animation first.
		// This test assumes the CharacterFacesUp test passed.

		// Gets the player and its animator.
		CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
		Animator animator = player.GetComponent<Animator>();
		Assert.NotNull(animator);
		Assert.NotNull(player.TilebasedMovement);

		player.TilebasedMovement.SecondsPerTile = 0.1f;

		player.Movement.MoveUp();
		yield return new WaitForSeconds(0.2f);

		// Moves the player down and waits.
		player.Movement.MoveDown();
		yield return new WaitForSeconds(0.2f);
		Assert.AreEqual(IDLE_DOWN, TestUtilities.GetCurrentClipName(animator));
	}

	[UnityTest]
	public IEnumerator TilebasedCharacterMovementAnimatesLeft()
	{
		// Gets the player and its animator.
		CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
		Animator animator = player.GetComponent<Animator>();
		Assert.NotNull(animator);
		Assert.NotNull(player.TilebasedMovement);

		player.Movement.MoveLeft(true);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(MOVING_LEFT, TestUtilities.GetCurrentClipName(animator));
	}

	[UnityTest]
	public IEnumerator TilebasedCharacterMovementAnimatesRight()
	{
		// Gets the player and its animator.
		CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
		Animator animator = player.GetComponent<Animator>();
		Assert.NotNull(animator);
		Assert.NotNull(player.TilebasedMovement);

		player.Movement.MoveRight(true);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(MOVING_RIGHT, TestUtilities.GetCurrentClipName(animator));
	}

	[UnityTest]
	public IEnumerator TilebasedCharacterMovementAnimatesUp()
	{
		// Gets the player and its animator.
		CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
		Animator animator = player.GetComponent<Animator>();
		Assert.NotNull(animator);
		Assert.NotNull(player.TilebasedMovement);

		player.Movement.MoveUp(true);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(MOVING_UP, TestUtilities.GetCurrentClipName(animator));
	}

	[UnityTest]
	public IEnumerator TilebasedCharacterMovementAnimatesDown()
	{
		// Gets the player and its animator.
		CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
		Animator animator = player.GetComponent<Animator>();
		Assert.NotNull(animator);
		Assert.NotNull(player.TilebasedMovement);

		player.Movement.MoveDown(true);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(MOVING_DOWN, TestUtilities.GetCurrentClipName(animator));
	}

	[UnityTest]
	public IEnumerator TilebasedMeleeAttackAnimates()
	{
		CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
		Animator animator = player.GetComponent<Animator>();
		Assert.NotNull(animator);
		Assert.NotNull(player.MeleeAttack);
		Assert.NotNull(player.TilebasedMovement);

		player.MeleeAttack.AttackCooldown = 1.0f;

		player.Movement.MoveDown();
		player.MeleeAttack.Attack(player.Movement.Direction);
		yield return new WaitForSeconds(0.1f);
		Assert.AreEqual(ATTACK_DOWN, TestUtilities.GetCurrentClipName(animator));
	}
}

