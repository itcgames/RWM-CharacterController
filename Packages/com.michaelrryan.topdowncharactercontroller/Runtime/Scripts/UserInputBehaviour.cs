using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInputBehaviour : CharacterBehaviour
{
    void Update()
    {
        // Horizontal Input.
        if (Input.GetKey(KeyCode.LeftArrow)) Controller.MoveLeft();
        if (Input.GetKey(KeyCode.RightArrow)) Controller.MoveRight();

        // Vertical Input.
        if (Input.GetKey(KeyCode.UpArrow)) Controller.MoveUp();
        if (Input.GetKey(KeyCode.DownArrow)) Controller.MoveDown();

        if (!Controller.DiagonalMovementAllowed)
        {
            // Checks for which direction was pressed last.
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
                Controller.PreferHorizontal = true;
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
                Controller.PreferHorizontal = false;
        }
    }
}
