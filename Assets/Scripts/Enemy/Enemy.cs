using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // Public Variables
    [SerializeField] private float attackRange;
    [SerializeField] private float canAttackDistance;
    [SerializeField] private float forgetTime = 3f; // Time in seconds, how long it takes to forget about target after chasing.

    public enum State { IDLE, SEARCH, PATROL, ATTACK, CHASE }
    public State currentState;

    // Private Variavles
    float forgetTimer;
    bool hasTarget;
    bool doForgetTimer;

    Vector3 destination;

    // Componenents
    Transform target;
    NavMeshAgent agent;
    FieldOfView fov;
    EnemyPatrol patrol;

    private void Awake()
    {
        patrol = GetComponent<EnemyPatrol>();
        agent = GetComponent<NavMeshAgent>();
        fov = GetComponent<FieldOfView>();
        hasTarget = false;
        doForgetTimer = false;
    }

    private void Start()
    {
        currentState = State.IDLE;
        target = GameObject.FindGameObjectWithTag("Player").transform;
        forgetTimer = forgetTime;

        if (target != null)
        {
            SwitchState(State.CHASE);
            hasTarget = true;
        }
    }

    private void Update()
    {
        MoveToDestination();

        if (doForgetTimer)
        {
            forgetTimer -= Time.deltaTime;
            if (forgetTimer <= 0)
            {
                doForgetTimer = false;
                forgetTimer = forgetTime;
                // Return to patrol state.
            }
        }

        //UpdateState();
    }

    private void MoveToDestination()
    {
        if (hasTarget)
        {
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        if (currentState == State.CHASE && !doForgetTimer)
                        {
                            doForgetTimer = true;
                        }
                    }
                }

                float distance = Vector3.Distance(target.position, transform.position);
                if (distance <= canAttackDistance)
                {
                    agent.SetDestination(target.position);
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (fov.visibleTargets.Count > 0)
        {
            hasTarget = true;
            target = fov.visibleTargets[0];
        }
        else
        {
            hasTarget = false;
            target = null;
        }
    }

    private void SwitchState(State stateToSwitchTo)
    {
        currentState = stateToSwitchTo;
    }

    private void UpdateState()
    {
        switch (currentState)
        {
            case State.ATTACK:
                AttackState();
                break;
            case State.CHASE:
                ChaseState();
                break;
            case State.IDLE:
                IdleState();
                break;
            case State.PATROL:
                PatrolState();
                break;
            case State.SEARCH:
                SearchState();
                break;

        }
    }

    private void AttackState()
    {
        float distanceFromTarget = Vector3.Distance(transform.position, target.position);
        if (distanceFromTarget > canAttackDistance)
        {
            currentState = State.CHASE;
            return;
        }

        // Attack the target. (Shoot the player)
    }

    private void ChaseState()
    {
        if (agent.remainingDistance > 0)
        {
            Vector3 heading = transform.position - target.position;
            float distance = heading.magnitude;

            if (distance > canAttackDistance)
            {

            }
        }
    }

    private void IdleState()
    {

    }

    private void PatrolState()
    {

    }

    private void SearchState()
    {

    }

    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f;

        while (hasTarget)
        {
            if (currentState == State.CHASE)
            {
                Vector3 directionToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - directionToTarget * canAttackDistance;

                agent.SetDestination(targetPosition);
            }
            else if (currentState == State.ATTACK)
            {
                float distanceFromTarget = Vector3.Distance(transform.position, target.position);
                if (distanceFromTarget > canAttackDistance)
                {
                    currentState = State.CHASE;
                }
            }
        }
        yield return new WaitForSeconds(refreshRate);
    }
}
