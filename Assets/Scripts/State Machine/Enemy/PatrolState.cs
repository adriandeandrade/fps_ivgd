using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IState
{
    float timeToWait = 1f; // Time between waypoints.
    float waitTimer;
    int waypointIndex = 0;
    bool isWaiting;

    Unit owner;
    Transform target;

    public PatrolState(Unit _owner)
    {
        this.owner = _owner; // Register this chase state to its owner.
    }

    public void Enter()
    {
        target = Waypoints.points[0];
        owner.agent.acceleration = 1f;
        owner.agent.speed = 5f;
    }

    public void Execute()
    {
        if(GameManager.instance.detected)
        {
            owner.stateMachine.ChangeState(new AttackState(this.owner));
            return;
        }

        owner.agent.SetDestination(target.position);

        if(owner.agent.remainingDistance <= 0.5f)
        {
            isWaiting = true;
        }

        if(isWaiting)
        {
            if(waitTimer <= 0)
            {
                isWaiting = false;
                GetNextWaypoint();
                waitTimer = timeToWait;
            } else
            {
                waitTimer -= Time.deltaTime;
            }
        }
    }

    public void Exit()
    {
        if(owner.agent.hasPath)
        {
            owner.agent.isStopped = true;
        }
    }

    private void GetNextWaypoint()
    {
        if(waypointIndex >= Waypoints.points.Length - 1)
        {
            waypointIndex = 0;
            target = Waypoints.points[0];
            return;
        }

        waypointIndex++;
        target = Waypoints.points[waypointIndex];
    }
}
