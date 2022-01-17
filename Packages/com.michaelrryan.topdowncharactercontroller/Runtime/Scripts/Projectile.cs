using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public float Speed = 12.0f;
	public float Damage = 1.0f;
	public string ShootersTag = "";

	private void Start()
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

	private void OnCollisionEnter2D(Collision2D collision)
	{
		// Tries to get a character controller component from the collider.
		var controller =
			collision.collider.GetComponent<TopdownCharacterController>();

		// If a character controller was found, damages it.
		if (controller)
		{
			bool damaged = controller.TakeDamage(Damage, ShootersTag);

			// If damage was done, destroys this object.
			if (damaged)
				Destroy(gameObject);
		}
		else
		{
			// If the collider was not a character controller, destroys self.
			Destroy(gameObject);
		}
	}
}
