using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardState : IState
{
    string stateName = "GuardState";

    Unit owner;

    public GuardState(Unit _owner)
    {
        this.owner = _owner;
    }

    public void Enter()
    {
        
    }

    public void Execute()
    {
        if (GameManager.instance.detected)
        {
            owner.stateMachine.ChangeState(new AttackState(this.owner));
            return;
        }
    }

    public void Exit()
    {
        
    }

    public string GetState()
    {
        return stateName;
    }
}
