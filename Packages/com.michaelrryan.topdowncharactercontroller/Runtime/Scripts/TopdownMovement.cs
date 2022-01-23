using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopdownCharacterController
{
	public class TopdownMovement : Movement
	{
		// ==== Properties ====

		[SerializeField]
		private bool _diagonalMovementAllowed = true;
		public bool DiagonalMovementAllowed
		{
			get { return _diagonalMovementAllowed; }
			set { SetDiagonalMovementAllowed(value); }
		}

		public float MaxSpeed = 5.0f;

		[SerializeField]
		private float _timeToMaxSpeed = 0.0f;
		public float TimeToMaxSpeed
		{
			get { return _timeToMaxSpeed; }
			set { SetTimeToMaxSpeed(value); }
		}

		[SerializeField]
		private float _timeToFullStop = 0.0f;
		public float TimeToFullStop
		{
			get { return _timeToFullStop; }
			set { SetTimeToFullStop(value); }
		}

		// ==== Private Variables ====

		private float _acceleration = 0.0f;
		private float _deceleration = 0.0f;

		// ==== Public Custom Methods ====

		public override float GetSpeed()
		{
			return _rigidbody.velocity.magnitude;
		}

		// ==== Unity Method Overloads ====

		private void OnValidate()
		{
			// Ensures the properties' set functions are called when one changes.
			SetDiagonalMovementAllowed(_diagonalMovementAllowed);
			SetTimeToMaxSpeed(_timeToMaxSpeed);
			SetTimeToFullStop(_timeToFullStop);
		}

		new void Start()
		{
			SetDiagonalMovementAllowed(_diagonalMovementAllowed);
			SetTimeToMaxSpeed(_timeToMaxSpeed);
			SetTimeToFullStop(_timeToFullStop);

			base.Start();

			// Ensures the rigidbody is set up correctly.
			if (_rigidbody) _rigidbody.isKinematic = false;
		}

		void Update()
		{
			Vector2 input = GetInput();

			// If there's input and not frozen on attack.
			if (input != Vector2.zero
				&& (_meleeAttack == null || !_meleeAttack.FreezeOnAttack
					|| _meleeAttack.CanAttack()))
			{
				Direction = input.normalized;

				if (_timeToMaxSpeed != 0.0f)
				{
					// Accelerates towards the input direction.
					_rigidbody.velocity += Direction * _acceleration * Time.deltaTime;

					// Checks if the character's speed is greater than the max (using
					//      squares for performance)
					if (_rigidbody.velocity.sqrMagnitude > MaxSpeed * MaxSpeed)
						_rigidbody.velocity = _rigidbody.velocity.normalized * MaxSpeed;
				}
				else
				{
					// Moves at a constant speed toward to input direction.
					_rigidbody.velocity = Direction * MaxSpeed;
				}

				// Sets the animation properties.
				if (_animator && HandleAnimationEvents)
				{
					_animator.SetFloat(DIRECTION_HORIZONTAL, Direction.x);
					_animator.SetFloat(DIRECTION_VERTICAL, Direction.y);
					_animator.SetFloat(SPEED, _rigidbody.velocity.sqrMagnitude);
				}
			}
			else
			{
				if (_timeToFullStop != 0.0f)
				{
					Vector2 direction = _rigidbody.velocity.normalized;
					_rigidbody.velocity -= direction 
						* Mathf.Min(_deceleration 
							* Time.deltaTime, Mathf.Abs(GetSpeed()));
				}
				else
				{
					// Brings the character to a complete stop.
					_rigidbody.velocity = Vector2.zero;
				}

				// Sets the animation speed property to 0.
				if (_animator && HandleAnimationEvents)
					_animator.SetFloat(SPEED, 0.0f);
			}

			// Resets the frame input to zero as the frame has ended.
			_frameInput = Vector2.zero;
		}

		// ==== Private Custom Methods ====

		private void SetDiagonalMovementAllowed(bool value)
		{
			_diagonalMovementAllowed = value;
			DiagonalMovement = value;
		}

		private void SetTimeToMaxSpeed(float value)
		{
			_timeToMaxSpeed = value;

			// Avoids a divide by zero - No need to set acceleration if
			//      _timeToMaxSpeed is zero as it won't be used.
			if (_timeToMaxSpeed != 0.0f)
				_acceleration = MaxSpeed / _timeToMaxSpeed;
		}

		private void SetTimeToFullStop(float value)
		{
			_timeToFullStop = value;

			// Avoids a divide by zero - No need to set deceleration if
			//      _timeToFullStop is zero as it won't be used.
			if (_timeToFullStop != 0.0f)
				_deceleration = MaxSpeed / _timeToFullStop;
		}
	}
}