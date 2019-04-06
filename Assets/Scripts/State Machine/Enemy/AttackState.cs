﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IState
{
    float maxDistanceFromPlayer = 5f;
    float waitBeforeMovingTime = 2f;
    float waitBeforeMovingTimer;

    bool canAttack;
    bool isMoving;

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
        owner.agent.acceleration = 8f;
        owner.agent.speed = 2.5f;
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
        }
    }

    public void Move()
    {
        owner.agent.isStopped = false;
        owner.agent.SetDestination(target.position);
    }

    public void Attack()
    {
        
    }

    public void Exit()
    {
        
    }
}
