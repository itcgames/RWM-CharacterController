using NUnit.Framework;
using System.Collections;
using UnityEngine;


public class TestUtilities
{
	public const string DEFAULT_CHARACTER_NAME = "Player";
	public const string DEFAULT_SCENE_NAME = "ZeldaDemoScene";
	public const string TILEBASED_SCENE_NAME = "ZeldaTilebasedDemoScene";


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
}
