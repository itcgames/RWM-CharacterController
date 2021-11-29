using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CustomisableBehavioursTests
{
    private TopdownCharacterController character;

    [SetUp]
    public void Setup()
    {
        GameObject characterObj = new GameObject();
        characterObj.AddComponent<TopdownCharacterController>();
        character = characterObj.GetComponent<TopdownCharacterController>();
    }

    [TearDown]
    public void Teardown()
    {
        Object.Destroy(character.gameObject);
    }


    // A test class to be used in SlottingInNewBehaviours().
    private class TestBehaviour : ICharacterBehaviour
    {
        private ICharacterController _characterController;

        public void SetBehaviourUser(ICharacterController characterController) =>
            _characterController = characterController;

        public ICharacterController GetBehaviourUser() => _characterController;
        public void Update() => _characterController.MoveRight();
    }


    [UnityTest]
    public IEnumerator SlottingInNewBehaviours()
    {
        // Sets the test character behaviour.
        ICharacterBehaviour behaviour = new TestBehaviour();
        character.SetBehaviour(behaviour);

        // Checks the behaviour was set correctly on both objects.
        Assert.AreEqual(behaviour, character.GetBehaviour());
        Assert.AreEqual(character, behaviour.GetBehaviourUser());

        // Checks that the character has been moving to the right without
        //      keyboard input.
        Vector3 position = character.transform.position;
        yield return new WaitForSeconds(0.5f);
        Assert.Greater(character.transform.position.x, position.x);
    }
}
