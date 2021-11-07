using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopdownCharacterController : MonoBehaviour
{
    [SerializeField]
    private float _maxSpeed = 10.0f;
    public float MaxSpeed { get { return _maxSpeed; } set { _maxSpeed = value; } }

    private Rigidbody2D _rb;
    private Vector2 _frameInput = Vector2.zero;
    private Vector2 _persistentInput = Vector2.zero;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (!_rb) throw new UnityException(
            "No Rigidbody2D on TopdownCharacterController " + name + ".");
    }

    public void Update()
    {
        GetInput();
        _rb.velocity = (_persistentInput + _frameInput) * _maxSpeed;
        _frameInput = Vector2.zero;
    }

    private void GetInput()
    {
        // Horizontal Input.
        if (Input.GetKey(KeyCode.LeftArrow)) MoveLeft();
        if (Input.GetKey(KeyCode.RightArrow)) MoveRight();

        // Vertical Input.
        if (Input.GetKey(KeyCode.UpArrow)) MoveUp();
        if (Input.GetKey(KeyCode.DownArrow)) MoveDown();
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