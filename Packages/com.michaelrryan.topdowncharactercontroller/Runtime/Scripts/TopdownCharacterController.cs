using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopdownCharacterController : MonoBehaviour
{
    // Properties must have a serialised counterpart to be accessible in the inspector.

    [SerializeField]
    private float _maxSpeed = 5.0f;
    public float MaxSpeed { get { return _maxSpeed; } set { _maxSpeed = value; } }

    [SerializeField]
    private bool _diagonalMovementAllowed = true;
    public bool DiagonalMovementAllowed { get { return _diagonalMovementAllowed; } 
                                          set { _diagonalMovementAllowed = value; } }

    private Rigidbody2D _rb;
    private Vector2 _frameInput = Vector2.zero;
    private Vector2 _persistentInput = Vector2.zero;
    private bool _lastInputWasVertical = false;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>(); // The Rigidbody component.

        // If no Rigidbody exists, add one with the correct setup.
        if (!_rb)
        {
            gameObject.AddComponent<Rigidbody2D>();
            _rb = GetComponent<Rigidbody2D>();
            _rb.isKinematic = true;
            _rb.useFullKinematicContacts = true;
        }
    }

    private void Update()
    {
        CheckForInput();
        _rb.velocity = GetInput() * _maxSpeed;
        _frameInput = Vector2.zero;
    }

    private void CheckForInput()
    {
        // Horizontal Input.
        if (Input.GetKey(KeyCode.LeftArrow)) MoveLeft();
        if (Input.GetKey(KeyCode.RightArrow)) MoveRight();

        // Vertical Input.
        if (Input.GetKey(KeyCode.UpArrow)) MoveUp();
        if (Input.GetKey(KeyCode.DownArrow)) MoveDown();

        if (!_diagonalMovementAllowed)
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
        if (!_diagonalMovementAllowed && input.x != 0.0f && input.y != 0.0f)
        {
            // Nullify whichever axis was not pressed last.
            if (_lastInputWasVertical) input.x = 0.0f;
            else input.y = 0.0f;
        }

        return input;
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
}