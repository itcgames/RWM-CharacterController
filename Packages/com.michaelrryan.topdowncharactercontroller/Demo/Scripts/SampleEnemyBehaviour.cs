using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopdownCharacterController
{
    public class SampleEnemyBehaviour : CharacterBehaviour
    {
        new void Start()
        {
            base.Start();

            Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
            rigidbody.isKinematic = true;
        }
    }
}