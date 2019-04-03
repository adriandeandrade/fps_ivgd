using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IState
{
    Unit owner;

    public ChaseState(Unit _owner)
    {
        this.owner = _owner; // Register this chase state to its owner.
    }

    public void Enter()
    {
        Debug.Log("Entering chase state.");
        owner.stateMachine.ChangeState(new PatrolState(this.owner));
    }

    public void Execute()
    {
        Debug.Log("Updating chase state.");
    }

    public void Exit()
    {
        Debug.Log("Exiting chase state.");
    }
}
