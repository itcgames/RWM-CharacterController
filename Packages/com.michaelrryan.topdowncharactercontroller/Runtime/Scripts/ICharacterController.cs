using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterController
{
    bool PreferHorizontal { get; set; }
    bool DiagonalMovementAllowed { get; set; }

    void SetBehaviour(ICharacterBehaviour behaviour);
    ICharacterBehaviour GetBehaviour();
    void MoveRight(bool persistent = false);
    void MoveLeft(bool persistent = false);
    void MoveUp(bool persistent = false);
    void MoveDown(bool persistent = false);
    void ClearPersistentInput();
    float GetSpeed();
}
