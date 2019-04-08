using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Unit : MonoBehaviour
{
    public StateMachine stateMachine = new StateMachine();
    public NavMeshAgent agent;
    public Animator animator;

    float currentSpeed;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        //animator = GetComponent<Animator>();
    }

    private void Start()
    {
        stateMachine.ChangeState(new PatrolState(this)); // Initial State
    }

    private void Update()
    {
        stateMachine.Update();

        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        currentSpeed = agent.velocity.sqrMagnitude;
        switch (stateMachine.CurrentState.GetState())
        {
            case "SearchState":
                Debug.Log("Animating Search State");

                currentSpeed = Mathf.Clamp01(currentSpeed);
                animator.SetFloat("SpeedMagnitude", currentSpeed / 2);
                break;
            case "PatrolState":
                currentSpeed = Mathf.Clamp01(currentSpeed);
                animator.SetFloat("SpeedMagnitude", currentSpeed / 2);

                Debug.Log("Animating Patrol State");
                break;
            case "AttackState":
                currentSpeed = Mathf.Clamp01(currentSpeed);
                animator.SetFloat("SpeedMagnitude", currentSpeed);

                Debug.Log("Animating Attack State");
                break;
        }
    }
}
