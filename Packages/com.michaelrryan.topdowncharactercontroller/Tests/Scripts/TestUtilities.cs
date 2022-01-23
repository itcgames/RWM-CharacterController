using NUnit.Framework;
using System.Collections;
using UnityEngine;


public class TestUtilities
{
	public const string DEFAULT_CHARACTER_NAME = "Player";
	public const string DEFAULT_SCENE_NAME = "ZeldaDemoScene";

	public static TopdownCharacterController GetCharacterByName(string name)
	{
		// Finds the character by name and ensures it's not null.
		GameObject characterObj = GameObject.Find(name);
		Assert.NotNull(characterObj);

		// Finds the character objects and ensures it's not null.
		var character = characterObj.GetComponent<TopdownCharacterController>();
		Assert.NotNull(character);

		return character;
	}

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

	public static TopdownCharacterController GetDefaultCharacter()
	{
		return GetCharacterByName(DEFAULT_CHARACTER_NAME);
	}

	public static CharacterBehaviour GetDefaultCharactersBehaviour()
	{
		return GetBehaviourByCharacterName(DEFAULT_CHARACTER_NAME);
	}

	public static string GetDefaultSceneName()
    {
		return DEFAULT_SCENE_NAME;
    }
}
