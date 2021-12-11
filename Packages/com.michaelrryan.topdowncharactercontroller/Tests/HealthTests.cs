using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class HealthTests
{
    [SetUp]
    public void Setup()
    {
        SceneManager.LoadScene("DemoScene");
    }

    [UnityTest]
    public IEnumerator CharacterDiesOnZeroHealth()
    {
        // Gets the character and the script, checking both are not null.
        GameObject playerObj = GameObject.Find("TopdownCharacter");
        Assert.NotNull(playerObj);
        var player = playerObj.GetComponent<TopdownCharacterController>();
        Assert.NotNull(player);

        // Damages the character equal to its health, waits a frame and
        //      checks they are now null.
        player.TakeDamage(player.Health);
        yield return null;
        playerObj = GameObject.Find("TopdownCharacter");
        Assert.Null(playerObj);
    }

    [UnityTest]
    public IEnumerator NoDamageTakenDuringGracePeriod()
    {
        // Gets the character and the script, checking both are not null.
        GameObject playerObj = GameObject.Find("TopdownCharacter");
        Assert.NotNull(playerObj);
        var player = playerObj.GetComponent<TopdownCharacterController>();
        Assert.NotNull(player);

        // Damages the character equal to half its health and stores the
        //      remaining health.
        player.TakeDamage(player.Health / 2.0f);
        float health = player.Health;

        // Waits half the grace period and tries to attack again.
        yield return new WaitForSeconds(player.DamageGracePeriod / 2.0f);
        player.TakeDamage(player.Health / 2.0f);

        // Checks that the health has not changed.
        Assert.AreEqual(health, player.Health);
    }

    [UnityTest]
    public IEnumerator DamageCanBeDoneAfterGracePeriod()
    {
        // An added margin to avoid floating point errors.
        const float ERROR_MARGIN = 0.05f;

        // Gets the character and the script, checking both are not null.
        GameObject playerObj = GameObject.Find("TopdownCharacter");
        Assert.NotNull(playerObj);
        var player = playerObj.GetComponent<TopdownCharacterController>();
        Assert.NotNull(player);

        // Damages the character equal to half its health.
        player.TakeDamage(player.Health / 2.0f);

        // Waits for the grace period to end and tries to attack again.
        yield return new WaitForSeconds(player.DamageGracePeriod + ERROR_MARGIN);
        player.TakeDamage(player.Health);

        // Waits a frame and checks the character is dead.
        yield return null;
        playerObj = GameObject.Find("TopdownCharacter");
        Assert.Null(playerObj);
    }
}