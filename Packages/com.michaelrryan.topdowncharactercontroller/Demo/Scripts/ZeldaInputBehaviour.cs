using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeldaInputBehaviour : CharacterBehaviour
{
    private float _maxHealth = 4.0f;

    new void Start()
    {
        base.Start();

        if (Health) Health.HP = _maxHealth;
        Controller.PreferHorizontal = true;
    }

    void Update()
    {
        // Horizontal Input.
        if (Input.GetKey(KeyCode.LeftArrow)) Controller.MoveLeft();
        if (Input.GetKey(KeyCode.RightArrow)) Controller.MoveRight();

        // Vertical Input.
        if (Input.GetKey(KeyCode.UpArrow)) Controller.MoveUp();
        if (Input.GetKey(KeyCode.DownArrow)) Controller.MoveDown();

        if (Input.GetKey(KeyCode.C))
        {
            if (Health && Health.HP == _maxHealth)
                RangedAttack.Fire(Controller.Direction);

            if (MeleeAttack) MeleeAttack.Attack(Controller.Direction);
        }
    }
}
