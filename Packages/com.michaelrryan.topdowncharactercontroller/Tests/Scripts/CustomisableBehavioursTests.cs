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
    const string SAMPLE_ENEMY_NAME = "Enemy";
    const string ENEMY_PROJECTILE_NAME = "RockProjectile(Clone)";

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
        // Disables the enemy to prevent unwanted behaviour.
        TestUtilities.DisableEnemy();

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

    [UnityTest]
    public IEnumerator SampleEnemyFiresProjectiles()
    {
        // Get's the default character's behaviour and positions them far away
        //      to avoid potential collision.
        CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
        player.transform.position = new Vector3(1000.0f, 0.0f);

        // Get's the enemy character's behaviour.
        CharacterBehaviour enemy = 
            TestUtilities.GetBehaviourByCharacterName(SAMPLE_ENEMY_NAME);

        GameObject projectile = null;
        float elapsedTime = 0.0f;

        // Loops for the max fire interval.
        while (elapsedTime <= SampleEnemyBehaviour.MAX_FIRE_INTERVAL)
        {
            // Waits the action interval and checks if the enemy has fired.
            yield return new WaitForSeconds(SampleEnemyBehaviour.ACTION_INTERVAL);
            projectile = GameObject.Find(ENEMY_PROJECTILE_NAME);
            if (projectile) break; // Breaks if a projectile was fired.
        }

        Assert.NotNull(projectile);

        // Takes a reference to the player's health.
        Assert.NotNull(player.Health);
        float hp = player.Health.HP;

        // Positions the player on the projectile.
        player.transform.position = projectile.transform.position;
        yield return new WaitForSeconds(0.1f);

        // Ensures the player took damage and the projectile is now null.
        Assert.Less(player.Health.HP, hp);
        Assert.Null(GameObject.Find(ENEMY_PROJECTILE_NAME));
        
        yield return null;
    }

    [UnityTest]
    public IEnumerator SampleEnemyDamagesPlayer()
    {
        // Get's the default character's behaviour and ensures they have health.
        CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
        Assert.NotNull(player.Health);

        // Get's the enemy character's behaviour.
        CharacterBehaviour enemy =
            TestUtilities.GetBehaviourByCharacterName(SAMPLE_ENEMY_NAME);

        // Positions the player on the enemy and checks it takes damage.
        float hp = player.Health.HP;
        player.transform.position = enemy.transform.position;
        yield return new WaitForSeconds(0.1f);
        Assert.Less(player.Health.HP, hp);
    }

    [UnityTest]
    public IEnumerator PlayerCanKillEnemy()
    {
        // Get's the default character's behaviour and ensures they have Melee Attack.
        CharacterBehaviour player = TestUtilities.GetDefaultCharactersBehaviour();
        Assert.NotNull(player.MeleeAttack);
        Assert.NotNull(player.Health);
        player.MeleeAttack.AttackDamage = 1.0f;
        player.MeleeAttack.AttackCooldown = 0.0f;

        // Gives the player a ridiculous amount of health so they don't die.
        player.Health.HP = 10000000.0f;

        // Get's the enemy character's behaviour and ensures they have health.
        CharacterBehaviour enemy =
            TestUtilities.GetBehaviourByCharacterName(SAMPLE_ENEMY_NAME);
        Assert.NotNull(enemy.Health);

        enemy.Health.DamageGracePeriod = 0.1f;
        float hp = enemy.Health.HP;

        // Positions the player next to the enemy and attacks the enemy.
        player.transform.position = enemy.transform.position + Vector3.left;
        player.MeleeAttack.Attack(Vector2.right);
        yield return null;
        Assert.Less(enemy.Health.HP, hp);

        // Waits for the damage grace period and a little extra for safety.
        yield return new WaitForSeconds(enemy.Health.DamageGracePeriod + 0.1f);

        // Positions the player again in case the enemy has moved.
        player.transform.position = enemy.transform.position + Vector3.left;

        // Attacks the enemy again and waits.
        player.MeleeAttack.Attack(Vector2.right);
        yield return new WaitForSeconds(enemy.Health.DamageGracePeriod + 0.1f);

        // Attacks the enemy a final time and checks if it's dead.
        player.transform.position = enemy.transform.position + Vector3.left;
        player.MeleeAttack.Attack(Vector2.right);
        yield return null;
        Assert.Null(GameObject.Find(SAMPLE_ENEMY_NAME));
    }
}
