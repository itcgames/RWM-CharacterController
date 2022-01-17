using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopdownRangedAttack : MonoBehaviour
{
    public GameObject projectilePrefab;

    [SerializeField]
    private float _cooldown = 0.5f;
    public float Cooldown { get { return _cooldown; }
                            set { SetCooldown(value); } }

    private float _lastAttackTime;

    public GameObject Fire(Vector2 direction)
    {
        // Ensures the cooldown has expired before firing.
        if (Time.time >= _lastAttackTime + _cooldown)
        {
            _lastAttackTime = Time.time;
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.back);
            return Instantiate(projectilePrefab, transform.position, rotation);
        }

        return null;
    }

    private void Start()
    {
        // Ensures no cooldown needs to be waited on when the scene begins.
        SetCooldown(_cooldown);
    }

    private void OnValidate()
    {
        // Ensures the properties' set functions are called when one changes.
        Cooldown = _cooldown;
    }

    private void SetCooldown(float value)
    {
        _cooldown = value;

        // Sets the last attack time so no cooldown needs to be waited on.
        _lastAttackTime = Time.time - _cooldown * 2.0f;
    }
}