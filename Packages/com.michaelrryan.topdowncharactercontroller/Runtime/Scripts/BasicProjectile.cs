using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : MonoBehaviour
{
    public float Speed = 12.0f;

    void Start()
    {
        // Gets the rigidbody component.
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();

        // If a rigidbody component was found.
        if (rigidbody)
        {
            // Gets the direction as a Vector2 and uses it for velocity.
            Vector3 direction = transform.rotation * Vector3.right;
            rigidbody.velocity = direction * Speed;
        }
    }
}
