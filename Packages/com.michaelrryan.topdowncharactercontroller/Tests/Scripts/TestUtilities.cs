using NUnit.Framework;
using System.Collections;
using UnityEngine;
using TopdownCharacterController;


public class TestUtilities
{
	public const string DEFAULT_CHARACTER_NAME = "Player";
	public const string DEFAULT_ENEMY_NAME = "Enemy";
	public const string DEFAULT_SCENE_NAME = "ZeldaDemoScene";
	public const string TILEBASED_SCENE_NAME = "ZeldaTilebasedDemoScene";
	public const string PROJECTILE_INSTANCE_NAME = "BasicProjectile(Clone)";
	public const string PROJECTILE_TAG = "Projectile";

	public static CharacterBehaviour GetBehaviourByCharacterName(string name)
    {
		// Finds the character by name and ensures it's not null.
		GameObject characterObj = GameObject.Find(name);
		Assert.NotNull(characterObj);

		// Gets the character behaviour and ensures it's not null.
		var behaviour = characterObj.GetComponent<CharacterBehaviour>();
		Assert.NotNull(behaviour);

		return behaviour;
	}

	public static CharacterBehaviour GetDefaultCharactersBehaviour()
	{
		return GetBehaviourByCharacterName(DEFAULT_CHARACTER_NAME);
	}

	public static string GetDefaultSceneName()
    {
		return DEFAULT_SCENE_NAME;
    }

	public static string GetCurrentClipName(Animator animator)
	{
		var info = animator.GetCurrentAnimatorClipInfo(0);
		Assert.IsNotEmpty(info);
		return info[0].clip.name;
	}

	public static void DisableEnemy()
	{
		var enemy = GetBehaviourByCharacterName(DEFAULT_ENEMY_NAME);
		Assert.NotNull(enemy.Movement);
		enemy.enabled = false;
		enemy.Movement.ClearPersistentInput();

		GameObject[] projectiles = GameObject.FindGameObjectsWithTag(PROJECTILE_TAG);
		foreach (GameObject projectile in projectiles)
            Object.Destroy(projectile);
	}
}
