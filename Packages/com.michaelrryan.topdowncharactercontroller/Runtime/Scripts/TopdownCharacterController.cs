﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopdownCharacterController : MonoBehaviour
{
	// ==== Properties ====

	[Header("Movement")]
	[SerializeField]
	private bool _tilebasedMovement = false;
	public bool TilebasedMovement { get { return _tilebasedMovement; } 
									set { SetTilebasedMovement(value); } }

	public Vector2 Direction = Vector2.down;

	[Header("Regular Movement")]
	public float MaxSpeed = 5.0f;

	[SerializeField]
	private bool _diagonalMovementAllowed = true;
	public bool DiagonalMovementAllowed { get { return _diagonalMovement; }
										  set { SetDiagonalMovementAllowed(value); } }

	[SerializeField]
	private bool _preferHorizontal = false;
	public bool PreferHorizontal { get { return _preferHorizontal; }
								   set { _preferHorizontal = value; } }

	[SerializeField]
	private float _timeToMaxSpeed = 0.0f;
	public float TimeToMaxSpeed { get { return _timeToMaxSpeed; }
								  set { SetTimeToMaxSpeed(value); } }

	[SerializeField]
	private float _timeToFullStop = 0.0f;
	public float TimeToFullStop { get { return _timeToFullStop; }
								  set { SetTimeToFullStop(value); } }

	[Header("Tilebased Movement")]
	public float TileSize = 1.0f;
	public float SecondsPerTile = 0.25f;

	[Header("Animation")]
	public bool HandleAnimationEvents = true;
	public Animator Animator;

	// ==== Private Variables ====

	private const string DIRECTION_HORIZONTAL = "DirectionHorizontal";
	private const string DIRECTION_VERTICAL = "DirectionVertical";
	private const string SPEED = "Speed";

	private MeleeAttack _meleeAttack;

	private Rigidbody2D _rb;
	private Vector2 _frameInput = Vector2.zero;
	private Vector2 _persistentInput = Vector2.zero;
	private float _acceleration = 0.0f;
	private float _deceleration = 0.0f;

	// Used internally for both regular and tilebased movement, similar to
	//      _diagonalMovementAllowed but can change without a users intention.
	private bool _diagonalMovement = true;

	// Tilebased movement variables.
	private Vector3 _previousPosition = Vector3.zero;
	private Vector3 _destination = Vector3.zero;
	private float? _secondsSinceMovementStarted = null;


	// ==== Unity Method Overloads ====

	private void OnValidate()
	{
		// Ensures the properties' set functions are called when one changes.
		TilebasedMovement = _tilebasedMovement;
		DiagonalMovementAllowed = _diagonalMovementAllowed;
		TimeToMaxSpeed = _timeToMaxSpeed;
		TimeToFullStop = _timeToFullStop;
	}

	private void Start()
	{
		_rb = GetComponent<Rigidbody2D>();

		// If no Rigidbody exists, add one.
		if (!_rb)
		{
			gameObject.AddComponent<Rigidbody2D>();
			_rb = GetComponent<Rigidbody2D>();
		}

		// Gets and sets up the animator.
		Animator = GetComponent<Animator>();

		if (Animator && HandleAnimationEvents)
		{
			Animator.SetFloat(DIRECTION_HORIZONTAL, Direction.x);
			Animator.SetFloat(DIRECTION_VERTICAL, Direction.y);
			Animator.SetFloat(SPEED, 0.0f);
		}

		_meleeAttack = GetComponent<MeleeAttack>();
	}

	private void Update()
	{ 
		if (_tilebasedMovement)
			UpdateTilebasedMovement();
		else
			UpdateRegularMovement();

		_frameInput = Vector2.zero;
	}

	// ==== Custom Methods ====

	private void UpdateRegularMovement()
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
				_rb.velocity += Direction * _acceleration * Time.deltaTime;

				// Checks if the character's speed is greater than the max (using
				//      squares for performance)
				if (_rb.velocity.sqrMagnitude > MaxSpeed * MaxSpeed)
					_rb.velocity = _rb.velocity.normalized * MaxSpeed;
			}
			else
			{
				// Moves at a constant speed toward to input direction.
				_rb.velocity = Direction * MaxSpeed;
			}

			// Sets the animation properties.
			if (Animator && HandleAnimationEvents)
			{
				Animator.SetFloat(DIRECTION_HORIZONTAL, Direction.x);
				Animator.SetFloat(DIRECTION_VERTICAL, Direction.y);
				Animator.SetFloat(SPEED, _rb.velocity.sqrMagnitude);
			}
		}
		else
		{
			if (_timeToFullStop != 0.0f)
			{
				Vector2 direction = _rb.velocity.normalized;
				_rb.velocity -= direction * Mathf.Min(_deceleration * Time.deltaTime, Mathf.Abs(GetSpeed()));
			}
			else
			{
				// Brings the character to a complete stop.
				_rb.velocity = Vector2.zero;
			}

			// Sets the animation speed property to 0.
			if (Animator && HandleAnimationEvents)
				Animator.SetFloat(SPEED, 0.0f);
		}
	}

	private void UpdateTilebasedMovement()
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

						if (Animator && HandleAnimationEvents)
							Animator.SetFloat(SPEED, 1.0f);
					}

					// Updates the direction property and animator.
					Direction = input.normalized;

					if (Animator && HandleAnimationEvents)
					{
						Animator.SetFloat(DIRECTION_HORIZONTAL, Direction.x);
						Animator.SetFloat(DIRECTION_VERTICAL, Direction.y);
					}
				}

				// If no input, set the animators speed property to zero.
				// We do this here opposed to when the movement is complete to
				//		avoid prematurely ending the animation between tiles
				//		while a movement key is held.
				else if (Animator && HandleAnimationEvents)
					Animator.SetFloat(SPEED, 0.0f);
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
	}

	private Vector2 GetInput()
	{
		// Gets the combined input of persistent and frame input and clamps.
		Vector2 input = _persistentInput + _frameInput;
		input.x = Mathf.Clamp(input.x, -1.0f, 1.0f);
		input.y = Mathf.Clamp(input.y, -1.0f, 1.0f);

		// If diagonal movement is not allowed and there's input along both axis.
		if (!_diagonalMovement 
			&& input.x != 0.0f && input.y != 0.0f)
		{
			// Nullify whichever axis is preferred.
			if (_preferHorizontal) input.y = 0.0f;
			else input.x = 0.0f;
		}

		return input;
	}

	// ==== Setter Methods ====

	private void SetTilebasedMovement(bool value)
	{
		_tilebasedMovement = value;

		if (_tilebasedMovement)
		{
			if (_rb)
			{
				_rb.velocity = Vector2.zero;
				_rb.isKinematic = true;
			}
			
			_diagonalMovement = false;
		}
		else
		{
			if (_rb) _rb.isKinematic = false;
			_diagonalMovement = _diagonalMovementAllowed;
		}
	}

	private void SetDiagonalMovementAllowed(bool value)
	{
		_diagonalMovementAllowed = value;

		if (!_tilebasedMovement)
			_diagonalMovement = value;
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

	// ==== Public Methods ====

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

	public float GetSpeed()
	{
		return _rb.velocity.magnitude;
	}
}