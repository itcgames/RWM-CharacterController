using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopdownCharacterController
{
    public class ZeldaInputBehaviour : CharacterBehaviour
    {
        private float _maxHealth = 4.0f;

        new void Start()
        {
            base.Start();

            // Disables the behaviour if the required components are null.
            if (!Movement || !MeleeAttack || !RangedAttack || !Health)
                enabled = false;
            else
            {
                Health.HP = _maxHealth;
                Movement.PreferHorizontal = true;
            }
        }

        void Update()
        {
            // Horizontal Input.
            if (Input.GetKey(KeyCode.LeftArrow)) Movement.MoveLeft();
            if (Input.GetKey(KeyCode.RightArrow)) Movement.MoveRight();

            // Vertical Input.
            if (Input.GetKey(KeyCode.UpArrow)) Movement.MoveUp();
            if (Input.GetKey(KeyCode.DownArrow)) Movement.MoveDown();

            if (Input.GetKey(KeyCode.C))
            {
                if (Health && Health.HP == _maxHealth)
                    RangedAttack.Fire(Movement.Direction);

                if (MeleeAttack) MeleeAttack.Attack(Movement.Direction);
            }
        }
    }
}