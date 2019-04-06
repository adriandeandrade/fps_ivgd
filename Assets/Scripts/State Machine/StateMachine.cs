using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    IState currentState;

    public IState CurrentState { get => currentState; set => currentState = value; }

    public void ChangeState(IState newState)
    {
        if(CurrentState != null)
        {
            CurrentState.Exit();
        }

        CurrentState = newState;
        CurrentState.Enter();
    }

    public void Update()
    {
        if(CurrentState != null)
        {
            CurrentState.Execute();
        }
    }
}
