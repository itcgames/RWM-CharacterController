using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInputBehaviour : CharacterBehaviour
{
    new void Start()
    {
        base.Start();

        // Disables the behaviour if the required components are null.
        if (!Movement || !MeleeAttack || !RangedAttack)
            enabled = false;
    }

    void Update()
    {
        // Horizontal Input.
        if (Input.GetKey(KeyCode.LeftArrow)) Movement.MoveLeft();
        if (Input.GetKey(KeyCode.RightArrow)) Movement.MoveRight();

        // Vertical Input.
        if (Input.GetKey(KeyCode.UpArrow)) Movement.MoveUp();
        if (Input.GetKey(KeyCode.DownArrow)) Movement.MoveDown();

        if (!Movement.DiagonalMovement)
        {
            // Checks for which direction was pressed last.
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
                Movement.PreferHorizontal = true;
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
                Movement.PreferHorizontal = false;
        }

        if (Input.GetKey(KeyCode.C))
            MeleeAttack.Attack(Movement.Direction);

        if (Input.GetKeyDown(KeyCode.X))
            RangedAttack.Fire(Movement.Direction);

    }
}
