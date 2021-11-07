using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopdownCharacterController : MonoBehaviour
{
    // ==== Properties ====

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
    private float _timeToMaxSpeed = 0.0f;
    public float TimeToMaxSpeed { get { return _timeToMaxSpeed; }
                                  set { SetTimeToMaxSpeed(value); } }

    [Header("Tilebased Movement")]
    public float TileSize = 1.0f;
    public float SecondsPerTile = 0.5f;


    // ==== Private Variables ====

    private Rigidbody2D _rb;
    private Vector2 _frameInput = Vector2.zero;
    private Vector2 _persistentInput = Vector2.zero;
    private float _acceleration = 0.0f;
    private bool _lastInputWasVertical = false;

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
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>(); // The Rigidbody component.

        // If no Rigidbody exists, add one.
        if (!_rb)
        {
            gameObject.AddComponent<Rigidbody2D>();
            _rb = GetComponent<Rigidbody2D>();
        }

        // Ensures the rigidbody is set up correctly.
        _rb.isKinematic = true;
        _rb.useFullKinematicContacts = true;
    }

    private void Update()
    {
        CheckForInput();

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
            _rb.velocity = Vector2.zero;
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

    private void CheckForInput()
    {
        // Horizontal Input.
        if (Input.GetKey(KeyCode.LeftArrow)) MoveLeft();
        if (Input.GetKey(KeyCode.RightArrow)) MoveRight();

        // Vertical Input.
        if (Input.GetKey(KeyCode.UpArrow)) MoveUp();
        if (Input.GetKey(KeyCode.DownArrow)) MoveDown();

        if (!_diagonalMovement)
        {
            // Checks for which direction was pressed last.
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
                _lastInputWasVertical = false;
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
                _lastInputWasVertical = true;
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
            // Nullify whichever axis was not pressed last.
            if (_lastInputWasVertical) input.x = 0.0f;
            else input.y = 0.0f;
        }

        return input;
    }


    // ==== Setter Methods ====

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
}