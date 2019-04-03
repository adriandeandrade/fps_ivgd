using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IState
{
    Unit owner;

    public PatrolState(Unit _owner)
    {
        this.owner = _owner; // Register this chase state to its owner.
    }

    public void Enter()
    {
        
    }

    public void Execute()
    {
        
    }

    public void Exit()
    {
        
    }
}
