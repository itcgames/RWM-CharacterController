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

	private const string DEFAULT_ANIMATION = IDLE_DOWN;

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
		var info = player.Animator.GetCurrentAnimatorClipInfo(0);
		Assert.IsNotEmpty(info);
		Assert.AreEqual(DEFAULT_ANIMATION, info[0].clip.name);

		yield return null;
	}

	[UnityTest]
	public IEnumerator CharacterFacesLeft()
	{
		// Gets the player and moves it to the left.
		var player = TestUtilities.GetDefaultCharacter();
		player.MoveLeft();
		yield return new WaitForSeconds(0.1f);

		// Checks the animation is the left facing animation.
		var info = player.Animator.GetCurrentAnimatorClipInfo(0);
		Assert.IsNotEmpty(info);
		Assert.AreEqual(IDLE_LEFT, info[0].clip.name);
	}

	[UnityTest]
	public IEnumerator CharacterFacesRight()
	{
		// Gets the player and moves it to the right.
		var player = TestUtilities.GetDefaultCharacter();
		player.MoveRight();
		yield return new WaitForSeconds(0.1f);

		// Checks the animation is the right facing animation.
		var info = player.Animator.GetCurrentAnimatorClipInfo(0);
		Assert.IsNotEmpty(info);
		Assert.AreEqual(IDLE_RIGHT, info[0].clip.name);
	}

	[UnityTest]
	public IEnumerator CharacterFacesUp()
	{
		// Gets the player and moves it upwards.
		var player = TestUtilities.GetDefaultCharacter();
		player.MoveUp();
		yield return new WaitForSeconds(0.1f);

		// Checks the animation is the upward facing animation.
		var info = player.Animator.GetCurrentAnimatorClipInfo(0);
		Assert.IsNotEmpty(info);
		Assert.AreEqual(IDLE_UP, info[0].clip.name);
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

		// Checks the animation is the downward facing animation.
		var info = player.Animator.GetCurrentAnimatorClipInfo(0);
		Assert.IsNotEmpty(info);
		Assert.AreEqual(IDLE_DOWN, info[0].clip.name);
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

		// Checks the animation is the left facing animation.
		var info = player.Animator.GetCurrentAnimatorClipInfo(0);
		Assert.IsNotEmpty(info);
		Assert.AreEqual(IDLE_LEFT, info[0].clip.name);
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

		// Checks the animation is the right facing animation.
		var info = player.Animator.GetCurrentAnimatorClipInfo(0);
		Assert.IsNotEmpty(info);
		Assert.AreEqual(IDLE_RIGHT, info[0].clip.name);
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

		// Checks the animation is the upward facing animation.
		var info = player.Animator.GetCurrentAnimatorClipInfo(0);
		Assert.IsNotEmpty(info);
		Assert.AreEqual(IDLE_UP, info[0].clip.name);
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

		// Checks the animation is the downward facing animation.
		var info = player.Animator.GetCurrentAnimatorClipInfo(0);
		Assert.IsNotEmpty(info);
		Assert.AreEqual(IDLE_DOWN, info[0].clip.name);
	}
}