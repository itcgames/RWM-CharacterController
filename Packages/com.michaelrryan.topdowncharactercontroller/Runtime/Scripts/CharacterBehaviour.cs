using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopdownCharacterController
{
    public abstract class CharacterBehaviour : MonoBehaviour
    {
        // A generic reference to the movement component.
        public Movement Movement { get; private set; }

        // In most cases, only one of these should be valid, while the other is null.
        public TopdownMovement TopdownMovement { get; private set; }
        public TilebasedMovement TilebasedMovement { get; private set; }

        public Health Health { get; private set; }
        public MeleeAttack MeleeAttack { get; private set; }
        public RangedAttack RangedAttack { get; private set; }

        public void Start()
        {
            Movement = GetComponent<Movement>();
            TopdownMovement = GetComponent<TopdownMovement>();
            TilebasedMovement = GetComponent<TilebasedMovement>();
            Health = GetComponent<Health>();
            MeleeAttack = GetComponent<MeleeAttack>();
            RangedAttack = GetComponent<RangedAttack>();
        }
    }
}