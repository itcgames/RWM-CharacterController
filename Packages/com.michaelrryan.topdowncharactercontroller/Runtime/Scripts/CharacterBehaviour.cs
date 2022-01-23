using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBehaviour : MonoBehaviour
{
    public TopdownCharacterController Controller { get; private set; }
    public Health Health { get; private set; }
    public MeleeAttack MeleeAttack { get; private set; }
    public RangedAttack RangedAttack { get; private set; }

    public void Start()
    {
        // Tries to get the character controller component.
        Controller = GetComponent<TopdownCharacterController>();

        // Disables this behaviour if no controller found.
        if (!Controller)
            enabled = false;
        else
        {
            Health = GetComponent<Health>();
            MeleeAttack = GetComponent<MeleeAttack>();
            RangedAttack = GetComponent<RangedAttack>();
        }
    }
}
