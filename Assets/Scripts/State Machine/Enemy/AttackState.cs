using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IState
{
    float maxDistanceFromPlayer = 5f;
    float waitBeforeMovingTime = 2f;
    float waitBeforeMovingTimer;

    bool canAttack;
    bool isMoving;

    string stateName = "AttackState";

    Unit owner;
    Transform target;

    public AttackState(Unit _owner)
    {
        this.owner = _owner; // Register this chase state to its owner.
    }

    public void Enter()
    {
        target = GameManager.instance.playerT;
        canAttack = false;
        isMoving = true;
    }

    public void Execute()
    {
        if (!GameManager.instance.detected)
        {
            owner.stateMachine.ChangeState(new SearchState(this.owner));
            return;
        }

        float distance = Vector3.Distance(owner.transform.position, target.position);

        if (distance <= (maxDistanceFromPlayer - 0.1f))
        {
            canAttack = true;
            isMoving = false;
            owner.agent.isStopped = true;
        }
        else
        {
            canAttack = false;
            isMoving = true;
            Move();
        }

        if(canAttack)
        {
            Attack();
            owner.animator.SetBool("CanShoot", true);
        } else
        {
            owner.animator.SetBool("CanShoot", false);
        }
    }

    public void Move()
    {
        owner.agent.isStopped = false;
        owner.agent.SetDestination(target.position);
    }

    public void Attack()
    {
        // Stop and shoot until too far again.   
    }

    public void Exit()
    {
        
    }

    public string GetState()
    {
        return stateName;
    }
}
