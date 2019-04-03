using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public StateMachine stateMachine = new StateMachine();

    private void Start()
    {
        stateMachine.ChangeState(new ChaseState(this));
    }

    private void Update()
    {
        stateMachine.Update();
    }
}
