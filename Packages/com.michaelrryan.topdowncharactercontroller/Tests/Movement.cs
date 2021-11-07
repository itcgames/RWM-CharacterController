using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class Movement
    {
        private TopdownCharacterController character;

        [SetUp]
        public void Setup()
        {
            GameObject characterObj = new GameObject();
            characterObj.AddComponent<TopdownCharacterController>();
            character =
                characterObj.GetComponent<TopdownCharacterController>();
        }

        [TearDown]
        public void Teardown()
        {
            Object.Destroy(character.gameObject);
        }

        [UnityTest]
        public IEnumerator HorizontalMovement()
        {
            // Rightward movement.
            Vector3 position = character.transform.position;
            character.MoveRight(true);
            yield return new WaitForSeconds(0.5f);
            Assert.Greater(character.transform.position.x, position.x);
            character.ClearPersistentInput();

            // Leftward movement.
            position = character.transform.position;
            character.MoveLeft(true);
            yield return new WaitForSeconds(0.5f);
            Assert.Less(character.transform.position.x, position.x);
            character.ClearPersistentInput();
        }

        [UnityTest]
        public IEnumerator VerticalMovement()
        {
            // Upwards movement.
            Vector3 position = character.transform.position;
            character.MoveUp(true);
            yield return new WaitForSeconds(0.5f);
            Assert.Greater(character.transform.position.y, position.y);
            character.ClearPersistentInput();

            // Downward movement.
            position = character.transform.position;
            character.MoveDown(true);
            yield return new WaitForSeconds(0.5f);
            Assert.Less(character.transform.position.y, position.y);
            character.ClearPersistentInput();
        }

        [UnityTest]
        public IEnumerator DiagonalMovement()
        {
            // Up + right movement.
            Vector3 position = character.transform.position;
            character.MoveUp(true);
            character.MoveRight(true);
            yield return new WaitForSeconds(0.5f);
            Assert.Greater(character.transform.position.y, position.y);
            Assert.Greater(character.transform.position.x, position.x);
            character.ClearPersistentInput();

            // Down + left movement.
            position = character.transform.position;
            character.MoveDown(true);
            character.MoveLeft(true);
            yield return new WaitForSeconds(0.5f);
            Assert.Less(character.transform.position.y, position.y);
            Assert.Less(character.transform.position.x, position.x);
            character.ClearPersistentInput();
        }

        [UnityTest]
        public IEnumerator NoMovementOnOppositeInput()
        {
            // Vertical movement.
            Vector3 position = character.transform.position;
            character.MoveUp(true);
            character.MoveDown(true);
            yield return new WaitForSeconds(0.5f);
            Assert.AreEqual(character.transform.position.y, position.y);
            character.ClearPersistentInput();

            // Horizontal movement.
            position = character.transform.position;
            character.MoveLeft(true);
            character.MoveRight(true);
            yield return new WaitForSeconds(0.5f);
            Assert.AreEqual(character.transform.position.y, position.y);
            character.ClearPersistentInput();
        }

        [UnityTest]
        public IEnumerator NoDiagonalMovementWhenDisabled()
        {
            character.DiagonalMovementAllowed = false;

            // The last input is horizontal by default, so the character should
            //      only move horizontally in these tests.

            // Up + right movement.
            Vector3 position = character.transform.position;
            character.MoveUp(true);
            character.MoveRight(true);
            yield return new WaitForSeconds(0.5f);
            Assert.AreEqual(character.transform.position.y, position.y);
            Assert.Greater(character.transform.position.x, position.x);
            character.ClearPersistentInput();

            // Down + left movement.
            position = character.transform.position;
            character.MoveDown(true);
            character.MoveLeft(true);
            yield return new WaitForSeconds(0.5f);
            Assert.AreEqual(character.transform.position.y, position.y);
            Assert.Less(character.transform.position.x, position.x);
            character.ClearPersistentInput();
        }
    }
}
