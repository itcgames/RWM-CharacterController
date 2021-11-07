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


        // ==== Movement On Input Tests ====

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
        }


        // ==== Restrict Movement to Cardinal Directions Test ====

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
        }

        
        // ==== Tilebased Movement Test ====
        [UnityTest]
        public IEnumerator TilebasedMovement()
        {
            // A small buffer to allow for timing based issues in the test.
            const float TimeErrorBuffer = 0.01f;

            // Sets up the character for tile based movement.
            character.TilebasedMovement = true;
            character.SecondsPerTile = 0.75f;
            character.TileSize = 1.27f;

            // Works out the character's position after moving one tile.
            Vector3 destination = character.transform.position 
                + Vector3.right * character.TileSize;

            // Checks the character hasn't reached the destination halfway
            //      through the time.
            character.MoveRight();
            // Waits half the movement time.
            yield return new WaitForSeconds(character.SecondsPerTile / 2.0f); 
            Assert.AreNotEqual(character.transform.position, destination);

            // Waits another half the movement time plus an error buffer and
            //      checks the character is at the destination.
            yield return new WaitForSeconds(character.SecondsPerTile / 2.0f 
                + TimeErrorBuffer);
            Assert.AreEqual(character.transform.position, destination);
        }

        // ==== Acceleration Tests ====
        [UnityTest]
        public IEnumerator Acceleration()
        {
            character.TimeToMaxSpeed = 0.5f;

            character.MoveRight(true);
            yield return new WaitForSeconds(character.TimeToMaxSpeed / 2.0f);
            Assert.AreNotEqual(character.GetSpeed(), character.MaxSpeed);
            yield return new WaitForSeconds(character.TimeToMaxSpeed / 2.0f);
            Assert.AreEqual(character.GetSpeed(), character.MaxSpeed);
        }
    }
}
