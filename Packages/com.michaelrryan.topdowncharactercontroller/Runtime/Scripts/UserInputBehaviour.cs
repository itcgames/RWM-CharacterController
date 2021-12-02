using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInputBehaviour : ICharacterBehaviour
{
    ICharacterController _characterController;

    public void SetBehaviourUser(ICharacterController characterController) =>
            _characterController = characterController;

    public ICharacterController GetBehaviourUser() => _characterController;

    public void Update()
    {
        // Horizontal Input.
        if (Input.GetKey(KeyCode.LeftArrow)) _characterController.MoveLeft();
        if (Input.GetKey(KeyCode.RightArrow)) _characterController.MoveRight();

        // Vertical Input.
        if (Input.GetKey(KeyCode.UpArrow)) _characterController.MoveUp();
        if (Input.GetKey(KeyCode.DownArrow)) _characterController.MoveDown();

        if (!_characterController.DiagonalMovementAllowed)
        {
            // Checks for which direction was pressed last.
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
                _characterController.PreferHorizontal = true;
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
                _characterController.PreferHorizontal = false;
        }
    }
}
