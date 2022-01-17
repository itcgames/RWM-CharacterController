using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class RangedAttackTests
{
    const string PROJECTILE_NAME = "BasicProjectile(Clone)";

    [SetUp]
    public void Setup()
    {
        SceneManager.LoadScene(TestUtilities.GetDefaultSceneName());
    }

    [UnityTest]
    public IEnumerator ProjectileFires()
    {
        // Gets the player character.
        TopdownCharacterController player = 
            TestUtilities.GetDefaultCharacter();

        // Gets the ranged attack component and checks it's not null.
        TopdownRangedAttack rangedAttack = 
            player.GetComponent<TopdownRangedAttack>();
        Assert.NotNull(rangedAttack);

        // Fires a projectile and waits a frame.
        GameObject fired = rangedAttack.Fire(player.Direction);
        yield return null;

        // Ensures the projectile exists and matches the projectile returned.
        GameObject projectile = GameObject.Find(PROJECTILE_NAME);
        Assert.NotNull(projectile);
        Assert.AreSame(fired, projectile);

        // Ensures the projectile and player share the same position.
        Assert.AreEqual(player.transform.position, projectile.transform.position);
    }
}