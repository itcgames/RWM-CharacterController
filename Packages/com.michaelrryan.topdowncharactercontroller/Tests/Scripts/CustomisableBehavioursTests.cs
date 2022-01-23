using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using TopdownCharacterController;

public class CustomisableBehavioursTests
{
    const string BEHAVIOUR_TEST_SCENE = "BehaviourTestScene";

    [SetUp]
    public void Setup()
    {
        SceneManager.LoadScene(BEHAVIOUR_TEST_SCENE);
    }

    // A test class to be used in SlottingInNewBehaviours().
    private class TestBehaviour : CharacterBehaviour
    {
        new void Start() 
        {
            base.Start();
            
            // Disables the behaviour if Movement is null.
            if (!Movement) enabled = false; 
        }

        void Update() => Movement.MoveRight();
    }


    [UnityTest]
    public IEnumerator SlottingInNewBehaviours()
    { 
        CharacterBehaviour behaviour = TestUtilities.GetDefaultCharactersBehaviour();
        GameObject character = behaviour.gameObject;

        // Removes the character's default behaviour.
        Object.Destroy(behaviour);

        // Sets the test character behaviour.
        character.AddComponent<TestBehaviour>();

        yield return new WaitForSeconds(0.1f);

        // Gets the character's behaviour.
        behaviour = character.GetComponent<CharacterBehaviour>();

        // Checks the behaviour was set correctly.
        Assert.IsTrue(behaviour is TestBehaviour);

        // Checks that the character has been moving to the right without
        //      keyboard input.
        Vector3 position = character.transform.position;
        yield return new WaitForSeconds(0.5f);
        Assert.Greater(character.transform.position.x, position.x);
    }

    [UnityTest]
    public IEnumerator DefaultBehaviourIsUserInput()
    {
        // Get's the default character's default behaviour.
        CharacterBehaviour behaviour = TestUtilities.GetDefaultCharactersBehaviour();

        // Checks the behaviour is a user input behaviour by default.
        Assert.IsTrue(behaviour is UserInputBehaviour);
        yield return null;
    }
}
