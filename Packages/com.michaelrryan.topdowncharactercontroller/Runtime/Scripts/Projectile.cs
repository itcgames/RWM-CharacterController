using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public float Speed = 12.0f;
	public float Damage = 1.0f;
	public float ExpireTime = 5.0f; // The time before the bullet destroys itself.
	public string ShootersTag = ""; // The tag of the shooter.

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

		StartCoroutine(DestroyAfterDelay());
	}

	private IEnumerator DestroyAfterDelay()
	{
		// Waits for the expire time to elapse before destroying the game object.
		yield return new WaitForSeconds(ExpireTime);
		if (gameObject)
			Destroy(gameObject);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		// Tries to get a character health component from the collider.
		Health characterHealth =
			collision.collider.GetComponent<Health>();

		// If a character health was found, damages it.
		if (characterHealth)
		{
			bool damaged = characterHealth.TakeDamage(Damage, ShootersTag);

			// If damage was done, destroys this object.
			if (damaged)
				Destroy(gameObject);
		}
		else
		{
			// If the collider did not have a health component, destroys self.
			Destroy(gameObject);
		}
	}
}
