using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class HealthTests
{
	// Variable and method used in DeathCallbacksAreRunOnDeath().
	private bool _deathCallbackRun = false;

	void DeathCallback()
    {
		_deathCallbackRun = true;
	}

	[SetUp]
	public void Setup()
	{
		SceneManager.LoadScene("DemoScene");
	}

	[UnityTest]
	public IEnumerator CharacterDiesOnZeroHealth()
	{
		// Damages the character equal to its health, waits a frame and
		//      checks they are now null.
		var character = GetCharacter();
		character.TakeDamage(character.Health);
		yield return null;

		GameObject characterObj = GameObject.Find("TopdownCharacter");
		Assert.Null(characterObj);
	}

	[UnityTest]
	public IEnumerator NoDamageTakenDuringGracePeriod()
	{
		var character = GetCharacter();

		// Stashes these to avoid interfering with the wait time.
		float waitTime = character.DamageGracePeriod * 0.25f;
		float halfHealth = character.Health * 0.5f;

		// Damages the character equal to half its health and stores the
		//      remaining health.
		character.TakeDamage(halfHealth);
		float health = character.Health;

		// Waits half the grace period and tries to attack again.
		yield return new WaitForSeconds(waitTime);
		character.TakeDamage(halfHealth);

		// Checks that the health has not changed.
		Assert.AreEqual(health, character.Health);
	}

	[UnityTest]
	public IEnumerator DamageCanBeDoneAfterGracePeriod()
	{
		// An added margin to avoid floating point errors.
		const float ERROR_MARGIN = 0.05f;

		// Damages the character equal to half its health.
		var character = GetCharacter();
		character.TakeDamage(character.Health / 2.0f);

		// Waits for the grace period to end and tries to attack again.
		yield return new WaitForSeconds(character.DamageGracePeriod + ERROR_MARGIN);
		character.TakeDamage(character.Health);

		// Waits a frame and checks the character is dead.
		yield return null;
		GameObject characterObj = GameObject.Find("TopdownCharacter");
		Assert.Null(characterObj);
	}

	[UnityTest]
	public IEnumerator FlashesDuringGracePeriod()
	{
		// An added margin to avoid floating point errors.
		const float ERROR_MARGIN = 0.05f;

		// Sets the properties to be used.
		float gracePeriod = 1.0f;
		int flashes = 2;

		// Stash for later to avoid interfering with the wait time.
		float flashLength = gracePeriod / (flashes * 2);
		float halfFlash = flashLength * 0.5f;

		// Gets the character and sets up the fields.
		var character = GetCharacter();
		character.DamageGracePeriod = gracePeriod;
		character.GracePeriodFlashes = flashes;

		// Takes reference to the character's renderer and ensures its not null.
		Renderer renderer = character.GetComponent<Renderer>();
		Assert.IsNotNull(renderer);

		// Damage the character and checks the character is invisible less than
		//      one flash length later.
		character.TakeDamage(character.Health / 2.0f);
		yield return new WaitForSeconds(halfFlash);
		Assert.IsFalse(renderer.enabled);

		// Checks the character is visible again after the first flash.
		yield return new WaitForSeconds(halfFlash + ERROR_MARGIN);
		Assert.IsTrue(renderer.enabled);
	}

	[UnityTest]
	public IEnumerator IsVisibleAfterGracePeriod()
	{
		// Gets the character and the renderer.
		var character = GetCharacter();
		Renderer renderer = character.GetComponent<Renderer>();
		Assert.IsNotNull(renderer);

		// Damages the character and checks its visible after the grace period.
		character.TakeDamage(character.Health / 2.0f);
		yield return new WaitForSeconds(character.DamageGracePeriod);
		Assert.IsTrue(renderer.enabled);
	}

	[UnityTest]
	public IEnumerator WhitelistedTagsCanDamage()
	{
		// Gets the character and whitelists the "Enemy" tag.
		var character = GetCharacter();
		character.DamageWhitelistTags.Add("Enemy");

		// Takes a copy for later.
		float health = character.Health;

		// Damages the character, passing the whitelisted tag.
		character.TakeDamage(character.Health / 2.0f, "Enemy");

		// Checks the character's health has gone down the next frame.
		yield return null;
		Assert.Less(character.Health, health);
	}

	[UnityTest]
	public IEnumerator NonWhitelistedTagsCannotDamage()
	{
		// Gets the character and whitelists the "Enemy" tag.
		var character = GetCharacter();
		character.DamageWhitelistTags.Add("Enemy");

		// Takes a copy for later.
		float health = character.Health;

		// Damages the character, passing the a non whitelisted tag.
		character.TakeDamage(character.Health / 2.0f, "Friend");

		// Checks the character's health has not changed.
		yield return null;
		Assert.AreEqual(character.Health, health);
	}

	[UnityTest]
	public IEnumerator DeathCallbacksAreRunOnDeath()
	{
		// Gets the character adds the callback and initialises the checker variable.
		var character = GetCharacter();
		character.DeathCallbacks.Add(DeathCallback);
		_deathCallbackRun = false;

		// Kills the character and checks the callback was run next frame.
		character.TakeDamage(character.Health);
		yield return null;
		Assert.IsTrue(_deathCallbackRun);
	}

	private TopdownCharacterController GetCharacter()
	{ 
		// Gets the character and the script, checking both are not null.
		GameObject characterObj = GameObject.Find("TopdownCharacter");
		Assert.NotNull(characterObj);
		var character = characterObj.GetComponent<TopdownCharacterController>();
		Assert.NotNull(character);

		return character;
	}
}