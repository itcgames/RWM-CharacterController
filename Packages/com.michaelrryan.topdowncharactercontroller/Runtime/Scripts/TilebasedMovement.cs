using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopdownCharacterController
{
	public class TilebasedMovement : Movement
	{
		// ==== Properties ====

		public float TileSize = 1.0f;
		public float SecondsPerTile = 0.25f;

		// ==== Private Variables ====

		private Vector3 _previousPosition = Vector3.zero;
		private Vector3 _destination = Vector3.zero;
		private float? _secondsSinceMovementStarted = null;

		// ==== Public Custom Methods ====

		public override float GetSpeed()
		{
			if (_secondsSinceMovementStarted == null)
				return 0.0f;
			else
				return TileSize / SecondsPerTile;
		}

		// ==== Unity Method Overloads ====

		new void Start()
		{
			DiagonalMovement = false;

			base.Start();
		}

		void Update()
		{
			float currentTime = Time.realtimeSinceStartup;

			// If the characer is not moving.
			if (_secondsSinceMovementStarted == null)
			{
				// If freeze on attack is enabled, check the attack cooldown has expired.
				if (_meleeAttack == null || !_meleeAttack.FreezeOnAttack
					|| _meleeAttack.CanAttack())
				{
					Vector2 input = GetInput();

					if (input != Vector2.zero)
					{
						_previousPosition = transform.position;
						Vector3 dest = transform.position + (Vector3)input * TileSize;

						// Checks there's no colliders in the next tile before moving.
						if (Physics2D.OverlapBox(dest, new Vector2(
							TileSize - 0.1f, TileSize - 0.1f), 0.0f) == null)
						{
							_destination = dest;
							_secondsSinceMovementStarted = currentTime;
							Direction = input.normalized;

							if (_animator && HandleAnimationEvents)
								_animator.SetFloat(SPEED, 1.0f);
						}

						// Updates the direction property and animator.
						Direction = input.normalized;

						if (_animator && HandleAnimationEvents)
						{
							_animator.SetFloat(DIRECTION_HORIZONTAL, Direction.x);
							_animator.SetFloat(DIRECTION_VERTICAL, Direction.y);
						}
					}

					// If no input, set the animators speed property to zero.
					// We do this here opposed to when the movement is complete to
					//		avoid prematurely ending the animation between tiles
					//		while a movement key is held.
					else if (_animator && HandleAnimationEvents)
						_animator.SetFloat(SPEED, 0.0f);
				}
			}

			// If the character is moving across a tile.
			else if (currentTime - _secondsSinceMovementStarted < SecondsPerTile)
			{
				Vector3 vectorToDest = _destination - _previousPosition;
				float interp = (currentTime - (float)_secondsSinceMovementStarted) / SecondsPerTile;
				transform.position = _previousPosition + vectorToDest * interp;
			}

			// If the character has reached the end of their movement.
			else
			{
				transform.position = _destination;
				_secondsSinceMovementStarted = null;
			}

			_frameInput = Vector2.zero;
		}
	}
}