using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopdownCharacterController
{
	public class SampleEnemyBehaviour : CharacterBehaviour
	{
		public GameObject projectile;

		public const float ACTION_INTERVAL = 1.0f;
		public const float MAX_FIRE_INTERVAL = 10.0f;

		private float _lastActionTime;
		private float _lastFireTime;

		// The maximum distance the enemy is able to walk during an action
		//		interval.
		private float _actionMovementDistance;

		CapsuleCollider2D _collider;

		private new void Start()
		{
			base.Start();

			Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
			_collider = GetComponent<CapsuleCollider2D>();

			// Disables the script if it doesn't have the necessary components.
			if (!_collider || !rigidbody || !TopdownMovement || !RangedAttack)
				enabled = false;
			else
			{
				rigidbody.isKinematic = true;

				_lastActionTime = Time.time - ACTION_INTERVAL;
				_actionMovementDistance = ACTION_INTERVAL * TopdownMovement.MaxSpeed;

				RangedAttack.projectilePrefab = projectile;
			}
		}

		private void Update()
		{
			// If the action interval has elapsed, act again.
			if (Time.time >= _lastActionTime + ACTION_INTERVAL)
			{
				// Sets the new last action time.
				_lastActionTime = Time.time;

				// Clears all previous input.
				Movement.ClearPersistentInput();

				// Performs a random action and has a change to fire a projectile.
				PerformRandomAction();
				PossiblyFireAProjectile();
			}
		}

		private void PerformRandomAction()
		{
			bool actionFound = false;

			while (!actionFound)
            {
				int action = Random.Range(0, 5);

				// Moves in a direction depending on the action number, if the
				//		direction is not blocked.
				// Does nothing if the action number is 0.
				switch (action)
				{
					case 0:
						actionFound = true;
						break;
					case 1:
						if (!IsDirectionBlocked(Vector2.left))
                        {
							Movement.MoveLeft(true);
							actionFound = true;
						}
						break;
					case 2:
						if (!IsDirectionBlocked(Vector2.right))
						{
							Movement.MoveRight(true);
							actionFound = true;
						}
						break;
					case 3:
						if (!IsDirectionBlocked(Vector2.up))
						{
							Movement.MoveUp(true);
							actionFound = true;
						}
						break;
					case 4:
						if (!IsDirectionBlocked(Vector2.down))
						{
							Movement.MoveDown(true);
							actionFound = true;
						}
						break;
				}
			}
		}
			
		private bool IsDirectionBlocked(Vector2 direction)
		{
			Vector2 nextPosition = (Vector2)transform.position 
				+ direction * _actionMovementDistance;

			Collider2D collided = Physics2D.OverlapCircle(nextPosition,
				_collider.size.x / 2.0f);

			// If a collision with something other then the player occured,
			//		returns true.
			return collided != null && !collided.CompareTag("Player");
		}

		private void PossiblyFireAProjectile()
        {
			// Works out the chance to fire. Get's closer to 100% as time without
			//      firing increases.
			float timeSinceLastShot = Time.time - _lastFireTime;
			int shotChance = (int)Mathf.Max(MAX_FIRE_INTERVAL - timeSinceLastShot, 0.0f);

			if (Random.Range(0, shotChance) == 0)
			{
				_lastFireTime = Time.time;
				RangedAttack.Fire(Movement.Direction);
			}
		}
	}
}