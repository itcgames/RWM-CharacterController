using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class CustomisableBehavioursTests
{
    [SetUp]
    public void Setup()
    {
        SceneManager.LoadScene("DemoScene");
    }

    // A test class to be used in SlottingInNewBehaviours().
    private class TestBehaviour : CharacterBehaviour
    {
        public void Update() => Controller.MoveRight();
    }


    [UnityTest]
    public IEnumerator SlottingInNewBehaviours()
    {
        // Gets the character object and script.
        GameObject characterObj = GameObject.Find("TopdownCharacter");
        Assert.NotNull(characterObj);

        var character = characterObj.GetComponent<TopdownCharacterController>();
        Assert.NotNull(character);

        // Removes the character's default behaviour.
        Object.Destroy(characterObj.GetComponent<CharacterBehaviour>());

        // Sets the test character behaviour.
        character.gameObject.AddComponent<TestBehaviour>();

        yield return new WaitForSeconds(0.1f);

        // Gets the character's behaviour.
        CharacterBehaviour behaviour = 
            character.gameObject.GetComponent<CharacterBehaviour>();

        // Checks the behaviour was set correctly on both objects.
        Assert.IsTrue(behaviour is TestBehaviour);
        Assert.AreSame(character, behaviour.Controller);

        // Checks that the character has been moving to the right without
        //      keyboard input.
        Vector3 position = character.transform.position;
        yield return new WaitForSeconds(0.5f);
        Assert.Greater(character.transform.position.x, position.x);
    }

    [UnityTest]
    public IEnumerator DefaultBehaviourIsUserInput()
    {
        // Gets the character object and script.
        GameObject characterObj = GameObject.Find("TopdownCharacter");
        Assert.NotNull(characterObj);

        // Get's the character's default behaviour.
        CharacterBehaviour behaviour = 
            characterObj.GetComponent<CharacterBehaviour>();

        // Checks the behaviour is a user input behaviour by default.
        Assert.IsTrue(behaviour is UserInputBehaviour);
        yield return null;
    }
}
