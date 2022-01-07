﻿using NUnit.Framework;
using System.Collections;
using UnityEngine;


public class TestUtilities
{
	public const string DEFAULT_CHARACTER_NAME = "Player";
	public const string DEFAULT_SCENE_NAME = "DemoScene";

	public static TopdownCharacterController GetCharacter(string name)
	{
		// Finds the character by name and ensures it's not null.
		GameObject characterObj = GameObject.Find(name);
		Assert.NotNull(characterObj);

		// Finds the character objects and ensures it's not null.
		var character = characterObj.GetComponent<TopdownCharacterController>();
		Assert.NotNull(character);

		return character;
	}

	public static TopdownCharacterController GetDefaultCharacter()
	{
		return GetCharacter(DEFAULT_CHARACTER_NAME);
	}

	public static string GetDefaultSceneName()
    {
		return DEFAULT_SCENE_NAME;
    }
}
