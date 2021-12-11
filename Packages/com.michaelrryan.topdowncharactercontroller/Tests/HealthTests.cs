using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace Tests
{
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
    }
}
