using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterBehaviour
{
    void SetBehaviourUser(ICharacterController characterController);
    ICharacterController GetBehaviourUser();
    void Update();
}
