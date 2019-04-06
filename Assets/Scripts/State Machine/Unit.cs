using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    public StateMachine stateMachine = new StateMachine();
    public NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        stateMachine.ChangeState(new PatrolState(this)); // Initial State
    }

    private void Update()
    {
        stateMachine.Update();
        Debug.Log(transform.name + ": " + stateMachine.CurrentState);
    }
}
