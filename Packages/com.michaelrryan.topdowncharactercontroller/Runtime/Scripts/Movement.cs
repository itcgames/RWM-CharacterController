using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopdownCharacterController
{
	public abstract class Movement : MonoBehaviour
	{
		// ==== Properties ====

		public bool HandleAnimationEvents = true;
		public Vector2 Direction = Vector2.down;
		public bool PreferHorizontal = false;

		// Is a property so it's not serialized (e.g. it doesn't show in the inspector).
		public bool DiagonalMovement { get; set; }

		// ==== Protected Variables ====

		protected const string DIRECTION_HORIZONTAL = "DirectionHorizontal";
		protected const string DIRECTION_VERTICAL = "DirectionVertical";
		protected const string SPEED = "Speed";

		protected Rigidbody2D _rigidbody;
		protected MeleeAttack _meleeAttack;
		protected Animator _animator;

		protected Vector2 _frameInput = Vector2.zero;
		protected Vector2 _persistentInput = Vector2.zero;

		// ==== Public Custom Methods ====

		public void MoveRight(bool persistent = false)
		{
			if (persistent)
				_persistentInput.x = Mathf.Min(_persistentInput.x + 1.0f, 1.0f);
			else
				_frameInput.x = Mathf.Min(_frameInput.x + 1.0f, 1.0f);
		}

		public void MoveLeft(bool persistent = false)
		{
			if (persistent)
				_persistentInput.x = Mathf.Max(_persistentInput.x - 1.0f, -1.0f);
			else
				_frameInput.x = Mathf.Max(_frameInput.x - 1.0f, -1.0f);
		}

		public void MoveUp(bool persistent = false)
		{
			if (persistent)
				_persistentInput.y = Mathf.Min(_persistentInput.y + 1.0f, 1.0f);
			else
				_frameInput.y = Mathf.Min(_frameInput.y + 1.0f, 1.0f);
		}

		public void MoveDown(bool persistent = false)
		{
			if (persistent)
				_persistentInput.y = Mathf.Max(_persistentInput.y - 1.0f, -1.0f);
			else
				_frameInput.y = Mathf.Max(_frameInput.y - 1.0f, -1.0f);
		}

		public void ClearPersistentInput()
		{
			_persistentInput = Vector2.zero;
		}

		public abstract float GetSpeed();

		// ==== Unity Method Overloads ====

		protected void Start()
		{
			_rigidbody = GetComponent<Rigidbody2D>();

			// If no Rigidbody exists, add one.
			if (!_rigidbody)
			{
				gameObject.AddComponent<Rigidbody2D>();
				_rigidbody = GetComponent<Rigidbody2D>();
			}

			_meleeAttack = GetComponent<MeleeAttack>();

			// Gets and sets up the animator.
			_animator = GetComponent<Animator>();

			if (_animator && HandleAnimationEvents)
			{
				_animator.SetFloat(DIRECTION_HORIZONTAL, Direction.x);
				_animator.SetFloat(DIRECTION_VERTICAL, Direction.y);
				_animator.SetFloat(SPEED, 0.0f);
			}
		}

		// ==== Protected Custom Methods ====

		protected Vector2 GetInput()
		{
			// Gets the combined input of persistent and frame input and clamps.
			Vector2 input = _persistentInput + _frameInput;
			input.x = Mathf.Clamp(input.x, -1.0f, 1.0f);
			input.y = Mathf.Clamp(input.y, -1.0f, 1.0f);

			// If diagonal movement is not allowed and there's input along both axis.
			if (!DiagonalMovement
				&& input.x != 0.0f && input.y != 0.0f)
			{
				// Nullify whichever axis is preferred.
				if (PreferHorizontal) input.y = 0.0f;
				else input.x = 0.0f;
			}

			return input;
		}
	}
}