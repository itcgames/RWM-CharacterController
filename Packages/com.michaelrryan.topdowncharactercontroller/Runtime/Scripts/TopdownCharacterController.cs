using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopdownCharacterController : MonoBehaviour
{
    [SerializeField]
    private float _maxSpeed = 10.0f;
    public float MaxSpeed { get { return _maxSpeed; } set { _maxSpeed = value; } }

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!rb) throw new UnityException(
            "No Rigidbody2D on TopdownCharacterController " + name + ".");
    }

    void Update()
    {
        rb.velocity = GetInputVector() * _maxSpeed;
    }

    private Vector2 GetInputVector()
    {
        Vector2 input = Vector2.zero;

        // Horizontal Input.
        if (Input.GetKey(KeyCode.LeftArrow))
            input.x -= 1.0f;

        if (Input.GetKey(KeyCode.RightArrow))
            input.x += 1.0f;

        // Vertical Input.
        if (Input.GetKey(KeyCode.UpArrow))
            input.y += 1.0f;

        if (Input.GetKey(KeyCode.DownArrow))
            input.y -= 1.0f;

        return input;
    }
}