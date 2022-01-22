using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class MovementTests
{
    [SetUp]
    public void Setup()
    {
        SceneManager.LoadScene(TestUtilities.GetDefaultSceneName());
    }

    // ==== Movement On Input Tests ====

    [UnityTest]
    public IEnumerator HorizontalMovement()
    {
        var player = TestUtilities.GetDefaultCharacter();

        // Rightward movement.
        Vector3 position = player.transform.position;
        player.MoveRight(true);
        yield return new WaitForSeconds(0.5f);
        Assert.Greater(player.transform.position.x, position.x);
        player.ClearPersistentInput();

        // Leftward movement.
        position = player.transform.position;
        player.MoveLeft(true);
        yield return new WaitForSeconds(0.5f);
        Assert.Less(player.transform.position.x, position.x);
    }

    [UnityTest]
    public IEnumerator VerticalMovement()
    {
        var player = TestUtilities.GetDefaultCharacter();

        // Upwards movement.
        Vector3 position = player.transform.position;
        player.MoveUp(true);
        yield return new WaitForSeconds(0.5f);
        Assert.Greater(player.transform.position.y, position.y);
        player.ClearPersistentInput();

        // Downward movement.
        position = player.transform.position;
        player.MoveDown(true);
        yield return new WaitForSeconds(0.5f);
        Assert.Less(player.transform.position.y, position.y);
    }

    [UnityTest]
    public IEnumerator DiagonalMovement()
    {
        var player = TestUtilities.GetDefaultCharacter();
        player.DiagonalMovementAllowed = true;

        // Up + right movement.
        Vector3 position = player.transform.position;
        player.MoveUp(true);
        player.MoveRight(true);
        yield return new WaitForSeconds(0.5f);
        Assert.Greater(player.transform.position.y, position.y);
        Assert.Greater(player.transform.position.x, position.x);
        player.ClearPersistentInput();

        // Down + left movement.
        position = player.transform.position;
        player.MoveDown(true);
        player.MoveLeft(true);
        yield return new WaitForSeconds(0.5f);
        Assert.Less(player.transform.position.y, position.y);
        Assert.Less(player.transform.position.x, position.x);
    }

    [UnityTest]
    public IEnumerator NoMovementOnOppositeInput()
    {
        var player = TestUtilities.GetDefaultCharacter();

        // Vertical movement.
        Vector3 position = player.transform.position;
        player.MoveUp(true);
        player.MoveDown(true);
        yield return new WaitForSeconds(0.5f);
        Assert.AreEqual(player.transform.position.y, position.y);
        player.ClearPersistentInput();

        // Horizontal movement.
        position = player.transform.position;
        player.MoveLeft(true);
        player.MoveRight(true);
        yield return new WaitForSeconds(0.5f);
        Assert.AreEqual(player.transform.position.y, position.y);
    }


    // ==== Restrict Movement to Cardinal Directions Test ====

    [UnityTest]
    public IEnumerator NoDiagonalMovementWhenDisabled()
    {
        var player = TestUtilities.GetDefaultCharacter();

        // Giving these properties the following values, the character should
        //      only move horizontally when moving vertically and horizontally.
        player.DiagonalMovementAllowed = false;
        player.PreferHorizontal = true;

        // Up + right movement.
        Vector3 position = player.transform.position;
        player.MoveUp(true);
        player.MoveRight(true);
        yield return new WaitForSeconds(0.5f);
        Assert.AreEqual(player.transform.position.y, position.y);
        Assert.Greater(player.transform.position.x, position.x);
        player.ClearPersistentInput();

        // Down + left movement.
        position = player.transform.position;
        player.MoveDown(true);
        player.MoveLeft(true);
        yield return new WaitForSeconds(0.5f);
        Assert.AreEqual(player.transform.position.y, position.y);
        Assert.Less(player.transform.position.x, position.x);
    }

        
    // ==== Tilebased Movement Test ====
    [UnityTest]
    public IEnumerator TilebasedMovement()
    {
        var player = TestUtilities.GetDefaultCharacter();

        // A small buffer to allow for timing based issues in the test.
        const float TimeErrorBuffer = 0.01f;

        // Sets up the character for tile based movement.
        player.TilebasedMovement = true;
        player.SecondsPerTile = 0.75f;
        player.TileSize = 1.27f;

        // Works out the character's position after moving one tile.
        Vector3 destination = player.transform.position 
            + Vector3.right * player.TileSize;

        // Checks the character hasn't reached the destination halfway
        //      through the time.
        player.MoveRight();
        // Waits half the movement time.
        yield return new WaitForSeconds(player.SecondsPerTile / 2.0f); 
        Assert.AreNotEqual(player.transform.position, destination);

        // Waits another half the movement time plus an error buffer and
        //      checks the character is at the destination.
        yield return new WaitForSeconds(player.SecondsPerTile / 2.0f 
            + TimeErrorBuffer);
        Assert.AreEqual(player.transform.position, destination);
    }

    // ==== Acceleration Test ====
    [UnityTest]
    public IEnumerator Acceleration()
    {
        var player = TestUtilities.GetDefaultCharacter();

        player.TimeToMaxSpeed = 0.5f;

        player.MoveRight(true);
        yield return new WaitForSeconds(player.TimeToMaxSpeed / 2.0f);
        Assert.AreNotEqual(player.GetSpeed(), player.MaxSpeed);
        yield return new WaitForSeconds(player.TimeToMaxSpeed / 2.0f);
        Assert.AreEqual(player.GetSpeed(), player.MaxSpeed);
    }

    // ==== Deceleration Test ====
    [UnityTest]
    public IEnumerator Deceleration()
    {
        var player = TestUtilities.GetDefaultCharacter();

        player.TimeToFullStop = 0.5f;

        player.MoveLeft(true);
        yield return new WaitForSeconds(0.5f);
        Assert.AreEqual(player.GetSpeed(), player.MaxSpeed);

        player.ClearPersistentInput();
        yield return new WaitForSeconds(player.TimeToFullStop / 2.0f);
        Assert.AreNotEqual(0.0f, player.GetSpeed());

        yield return new WaitForSeconds(player.TimeToFullStop / 2.0f);
        Assert.AreEqual(0.0f, player.GetSpeed());
    }
}
