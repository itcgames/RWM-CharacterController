using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopdownCharacterController : MonoBehaviour
{
	// ==== Properties ====

	[Header("Health & Damage")]

	[SerializeField]
	private float _health = 5.0f;
	public float Health { get { return _health; }
						  set { _health = value; } }

	[SerializeField]
	private float _damageGracePeriod = 0.8f;
	public float DamageGracePeriod { get { return _damageGracePeriod; }
									 set { SetDamageGracePeriod(value); } }

	[SerializeField]
	private int _gracePeriodFlashes = 4;
	public int GracePeriodFlashes { get { return _gracePeriodFlashes; }
									set { _gracePeriodFlashes = value; } }

	[SerializeField]
	private List<string> _damageWhitelistTags = new List<string>();
	public List<string> DamageWhitelistTags { get { return _damageWhitelistTags; }
											  private set { } }

	public delegate void DeathCallback();
	public List<DeathCallback> DeathCallbacks = new List<DeathCallback>();

	[Header("Movement")]
	[SerializeField]
	private bool _tilebasedMovement = false;
	public bool TilebasedMovement { get { return _tilebasedMovement; } 
									set { SetTilebasedMovement(value); } }

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
	public float SecondsPerTile = 0.5f;


	// ==== Private Variables ====

	private Rigidbody2D _rb;
	private Renderer _renderer;
	private Vector2 _frameInput = Vector2.zero;
	private Vector2 _persistentInput = Vector2.zero;
	private float _acceleration = 0.0f;
	private float _deceleration = 0.0f;
	private float _lastHitTaken = 0.0f;

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
		DamageGracePeriod = _damageGracePeriod;
		TilebasedMovement = _tilebasedMovement;
		DiagonalMovementAllowed = _diagonalMovementAllowed;
		TimeToMaxSpeed = _timeToMaxSpeed;
		TimeToFullStop = _timeToFullStop;
	}

	private void Start()
	{
		_renderer = GetComponent<Renderer>();
		_rb = GetComponent<Rigidbody2D>();

		// If no Rigidbody exists, add one.
		if (!_rb)
		{
			gameObject.AddComponent<Rigidbody2D>();
			_rb = GetComponent<Rigidbody2D>();
		}

		// Ensures the rigidbody is set up correctly.
		_rb.isKinematic = true;
		_rb.useFullKinematicContacts = true;

		// Sets the last hit taken time so the character can immediately start taking damage.
		_lastHitTaken = Time.time - _damageGracePeriod * 2.0f;
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

		if (input != Vector2.zero)
		{
			if (_timeToMaxSpeed != 0.0f)
			{
				// Accelerates towards the input direction.
				_rb.velocity += input.normalized * _acceleration * Time.deltaTime;

				// Checks if the character's speed is greater than the max (using
				//      squares for performance)
				if (_rb.velocity.sqrMagnitude > MaxSpeed * MaxSpeed)
					_rb.velocity = _rb.velocity.normalized * MaxSpeed;
			}
			else
			{
				// Moves at a constant speed toward to input direction.
				_rb.velocity = input.normalized * MaxSpeed;
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
		}
	}

	private void UpdateTilebasedMovement()
	{
		float currentTime = Time.realtimeSinceStartup;

		// If the characer is not moving.
		if (_secondsSinceMovementStarted == null)
		{
			Vector2 input = GetInput();

			if (input != Vector2.zero)
			{
				_previousPosition = transform.position;
				_destination = transform.position + (Vector3)GetInput() * TileSize;
				_secondsSinceMovementStarted = currentTime;
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

	private IEnumerator FlashForGracePeriod()
	{
		yield return null;

		// Checks there's a renderer as the character can't flash without it.
		if (_renderer)
		{
			// Works out the number of iterations to flash for.
			int iterations = _gracePeriodFlashes * 2;
			for (int i = 0; i < iterations; ++i)
			{
				// Toggles the visibility and waits.
				_renderer.enabled = !_renderer.enabled;
				yield return new WaitForSeconds(_damageGracePeriod / iterations);
			}
		}
	}


	// ==== Setter Methods ====

	private void SetDamageGracePeriod(float value)
	{
		_damageGracePeriod = value;
		_lastHitTaken = Time.time - _damageGracePeriod * 2.0f;
	}

	private void SetTilebasedMovement(bool value)
	{
		_tilebasedMovement = value;

		if (_tilebasedMovement)
		{
			if (_rb) _rb.velocity = Vector2.zero;
			_diagonalMovement = false;
		}
		else
			_diagonalMovement = _diagonalMovementAllowed;
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

	public void TakeDamage(float damage, string attackersTag = null)
	{
		// Checks the grace period has elapsed.
		if (Time.time >= _lastHitTaken + _damageGracePeriod)
		{
			// Checks if the attackers tag is whitelisted to damage this object.
			if (attackersTag == null || _damageWhitelistTags.Contains(attackersTag))
			{
				// Sets the new last hit taken time and takes the damage.
				_lastHitTaken = Time.time;
				_health -= damage;

				// Checks if health has reached zero and destroys if so.
				if (_health <= 0.0f)
				{
					foreach (var callback in DeathCallbacks) callback();
					Destroy(gameObject);
				}
				// Starts the flash coroutine if not dead.
				else StartCoroutine(FlashForGracePeriod());
			}
		}
	}

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