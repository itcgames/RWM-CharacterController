using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopdownRangedAttack : MonoBehaviour
{
    public GameObject projectilePrefab;

    public GameObject Fire(Vector2 direction)
    {
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.back);
        return Instantiate(projectilePrefab, transform.position, rotation);
    }
}