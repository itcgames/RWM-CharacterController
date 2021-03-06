using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using TopdownCharacterController;

public class HealthTests
{
	// Variable and method used in DeathCallbacksAreRunOnDeath().
	private bool _deathCallbackRun = false;
	private Dictionary<string, string> _damageInfo = null;
	private float _newPlayerHealth = 0.0f;

	void DeathCallback(Dictionary<string, string> damageInfo)
	{
		_deathCallbackRun = true;
	}

	void DeathCallbackWithInfo(Dictionary<string, string> damageInfo)
	{
		_damageInfo = damageInfo;
	}

	void HealthChangedCallback(float newHP, Dictionary<string, string> damageInfo)
	{
		_damageInfo = damageInfo;
		_newPlayerHealth = newHP;
	}

	[SetUp]
	public void Setup()
	{
		SceneManager.LoadScene(TestUtilities.GetDefaultSceneName());
	}

	[UnityTest]
	public IEnumerator CharacterDiesOnZeroHealth()
	{
		// Disables the enemy to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		// Damages the character equal to its health, waits a frame and
		//      checks they are now null.
		Health health = GetDefaultCharacterHealth();
		health.TakeDamage(health.HP);
		yield return null;
		Assert.Null(GameObject.Find(TestUtilities.DEFAULT_CHARACTER_NAME));
	}

	[UnityTest]
	public IEnumerator NoDamageTakenDuringGracePeriod()
	{
		// Disables the enemy to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		Health health = GetDefaultCharacterHealth();

		// Stashes these to avoid interfering with the wait time.
		float waitTime = health.DamageGracePeriod * 0.25f;
		float halfHealth = health.HP * 0.5f;

		// Damages the character equal to half its health and stores the
		//      remaining health.
		health.TakeDamage(halfHealth);
		float hp = health.HP;

		// Waits half the grace period and tries to attack again.
		yield return new WaitForSeconds(waitTime);
		health.TakeDamage(halfHealth);

		// Checks that the health has not changed.
		Assert.AreEqual(hp, health.HP);
	}

	[UnityTest]
	public IEnumerator DamageCanBeDoneAfterGracePeriod()
	{
		// Disables the enemy to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		// An added margin to avoid floating point errors.
		const float ERROR_MARGIN = 0.05f;

		// Damages the character equal to half its health.
		Health health = GetDefaultCharacterHealth();
		health.TakeDamage(health.HP / 2.0f);

		// Waits for the grace period to end and tries to attack again.
		yield return new WaitForSeconds(health.DamageGracePeriod + ERROR_MARGIN);
		health.TakeDamage(health.HP);

		// Waits a frame and checks the character is dead.
		yield return null;
		GameObject characterObj = GameObject.Find("TopdownCharacter");
		Assert.Null(characterObj);
	}

	[UnityTest]
	public IEnumerator FlashesDuringGracePeriod()
	{
		// Disables the enemy to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		// An added margin to avoid floating point errors.
		const float ERROR_MARGIN = 0.05f;

		// Sets the properties to be used.
		float gracePeriod = 1.0f;
		int flashes = 2;

		// Stash for later to avoid interfering with the wait time.
		float flashLength = gracePeriod / (flashes * 2);
		float halfFlash = flashLength * 0.5f;

		// Gets the character's health component and sets up the fields.
		Health health = GetDefaultCharacterHealth();
		health.DamageGracePeriod = gracePeriod;
		health.GracePeriodFlashes = flashes;

		// Takes reference to the character's renderer and ensures its not null.
		Renderer renderer = health.GetComponent<Renderer>();
		Assert.IsNotNull(renderer);

		// Damage the character and checks the character is invisible less than
		//      one flash length later.
		health.TakeDamage(health.HP / 2.0f);
		yield return new WaitForSeconds(halfFlash);
		Assert.IsFalse(renderer.enabled);

		// Checks the character is visible again after the first flash.
		yield return new WaitForSeconds(halfFlash + ERROR_MARGIN);
		Assert.IsTrue(renderer.enabled);
	}

	[UnityTest]
	public IEnumerator IsVisibleAfterGracePeriod()
	{
		// Disables the enemy to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		// Gets the character's health and renderer components.
		Health health = GetDefaultCharacterHealth();
		Renderer renderer = health.GetComponent<Renderer>();
		Assert.IsNotNull(renderer);

		// Damages the character and checks its visible after the grace period.
		health.TakeDamage(health.HP / 2.0f);
		yield return new WaitForSeconds(health.DamageGracePeriod);
		Assert.IsTrue(renderer.enabled);
	}

	[UnityTest]
	public IEnumerator WhitelistedTagsCanDamage()
	{
		// Disables the enemy to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		// Gets the character's health component and whitelists the "Enemy" tag.
		Health health = GetDefaultCharacterHealth();
		health.DamageWhitelistTags.Add("Enemy");

		// Takes a copy for later.
		float hp = health.HP;

		// Damages the character, passing the whitelisted tag.
		health.TakeDamage(health.HP / 2.0f, "Enemy");

		// Checks the character's health has gone down the next frame.
		yield return null;
		Assert.Less(health.HP, hp);
	}

	[UnityTest]
	public IEnumerator NonWhitelistedTagsCannotDamage()
	{
		// Disables the enemy to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		// Gets the character's health component and whitelists the "Enemy" tag.
		Health health = GetDefaultCharacterHealth();
		health.DamageWhitelistTags.Add("Enemy");

		// Takes a copy for later.
		float hp = health.HP;

		// Damages the character, passing the a non whitelisted tag.
		health.TakeDamage(health.HP / 2.0f, "Friend");

		// Checks the character's health has not changed.
		yield return null;
		Assert.AreEqual(health.HP, hp);
	}

	[UnityTest]
	public IEnumerator AllWhitelistTagAllowsAnyTag()
	{
		// Disables the enemy to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		// Gets the character's health component and whitelists the "All" tag.
		Health health = GetDefaultCharacterHealth();
		health.DamageWhitelistTags.Add("All");

		// Takes a copy for later.
		float hp = health.HP;

		// Damages the character, passing a non whitelisted tag.
		health.TakeDamage(health.HP / 2.0f, "Enemy");

		// Checks the character's health has gone down the next frame.
		yield return null;
		Assert.Less(health.HP, hp);
	}

	[UnityTest]
	public IEnumerator DeathCallbacksAreRunOnDeath()
	{
		// Disables the enemy to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		// Gets the character's health component, adds the callback and
		//		initialises the checker variable.
		Health health = GetDefaultCharacterHealth();
		health.DeathCallbacks.Add(DeathCallback);
		_deathCallbackRun = false;

		// Kills the character and checks the callback was run next frame.
		health.TakeDamage(health.HP);
		yield return null;
		Assert.IsTrue(_deathCallbackRun);
	}

	[UnityTest]
	public IEnumerator DeathCallbacksPassDamageInfo()
	{
		// Disables the enemy to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		// Sets the info to null to start.
		_damageInfo = null;

		// Gets the character's health component and adds the callback.
		Health health = GetDefaultCharacterHealth();
		health.DeathCallbacks.Add(DeathCallbackWithInfo);

		const string KEY_NAME = "test_key";
		const string VALUE_NAME = "test_value";

		Dictionary<string, string> attackInfo = new Dictionary<string, string>();
		attackInfo.Add(KEY_NAME, VALUE_NAME);

        // Kills the character and checks the callback was run next frame.
        health.TakeDamage(health.HP, null, attackInfo);
		yield return null;
		Assert.NotNull(_damageInfo);
		Assert.AreEqual(VALUE_NAME, _damageInfo[KEY_NAME]);
	}

	[UnityTest]
	public IEnumerator HealthChangedCallbacksAreRunWhenDamaged()
	{
		// Disables the enemy to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		// Initialises the checker variables.
		_damageInfo = null;
		_newPlayerHealth = 0.0f;

		// Gets the character's health component and adds the callback.
		Health health = GetDefaultCharacterHealth();
		health.HealthChangedCallbacks.Add(HealthChangedCallback);

		const string KEY_NAME = "test_key";
		const string VALUE_NAME = "test_value";

		Dictionary<string, string> attackInfo = new Dictionary<string, string>();
		attackInfo.Add(KEY_NAME, VALUE_NAME);

		// Damages the character and checks the callback was run next frame.
		health.TakeDamage(health.HP * 0.5f, null, attackInfo);
		yield return null;

		// Checks the damage info was passed through correctly.
		Assert.NotNull(_damageInfo);
		Assert.AreEqual(VALUE_NAME, _damageInfo[KEY_NAME]);

		// Checks the new health is correct.
		Assert.AreEqual(health.HP, _newPlayerHealth);
	}

	[UnityTest]
	public IEnumerator HealthChangedCallbacksAreRunWhenHealed()
	{
		// Disables the enemy to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		// Initialises the checker variables.
		_damageInfo = null;
		_newPlayerHealth = 0.0f;

		// Gets the character's health component and adds the callback.
		Health health = GetDefaultCharacterHealth();
		health.HealthChangedCallbacks.Add(HealthChangedCallback);

		// Heals the character and checks the callback was run next frame.
		health.HP += 5.0f;
		yield return null;

		// Checks the damage info is null.
		Assert.Null(_damageInfo);

		// Checks the new health is correct.
		Assert.AreEqual(health.HP, _newPlayerHealth);
	}

	[UnityTest]
	public IEnumerator HealthChangedCallbacksAreRunOnDeath()
	{
		// Disables the enemy to prevent unwanted behaviour.
		TestUtilities.DisableEnemy();

		// Initialises the checker variables.
		_damageInfo = null;
		_newPlayerHealth = 0.0f;

		// Gets the character's health component and adds the callback.
		Health health = GetDefaultCharacterHealth();
		health.HealthChangedCallbacks.Add(HealthChangedCallback);

		const string KEY_NAME = "test_key";
		const string VALUE_NAME = "test_value";

		Dictionary<string, string> attackInfo = new Dictionary<string, string>();
		attackInfo.Add(KEY_NAME, VALUE_NAME);

		// Kills the character and checks the callback was run next frame.
		health.TakeDamage(health.HP, null, attackInfo);
		yield return null;

		// Checks the damage info was passed through correctly.
		Assert.NotNull(_damageInfo);
		Assert.AreEqual(VALUE_NAME, _damageInfo[KEY_NAME]);

		// Checks the new health is correct.
		Assert.AreEqual(0.0f, health.HP);
		Assert.AreEqual(health.HP, _newPlayerHealth);
	}

	private Health GetDefaultCharacterHealth()
	{
		CharacterBehaviour character = TestUtilities.GetDefaultCharactersBehaviour();
		Assert.NotNull(character.Health);
		return character.Health;
	}
}