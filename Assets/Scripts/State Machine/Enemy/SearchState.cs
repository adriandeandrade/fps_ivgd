using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchState : IState
{
    float searchTime = 2f;
    float searchTimer = 0f;
    float searchRadius = 3f;
    int amountOfSearches = 2;
    bool searching = true;

    Unit owner;
    Transform target;
    Transform lastKnownPosition;

    public SearchState(Unit _owner)
    {
        this.owner = _owner; // Register this chase state to its owner.
    }

    public void Enter()
    {
        target = GameManager.instance.playerT;
        searching = true;
        owner.StartCoroutine(Search());
    }

    public void Execute()
    {
        if (GameManager.instance.detected)
        {
            owner.stateMachine.ChangeState(new AttackState(this.owner));
            return;
        }

        //Vector3 searchPoint = new Vector3(lastKnownPosition.position.x, lastKnownPosition.position.y, lastKnownPosition.position.z) + Random.insideUnitSphere * searchRadius;
        //target.position = searchPoint;
        //owner.agent.SetDestination(searchPoint);
        //Debug.Log(searchPoint);
    }

    public void Exit()
    {

    }

    IEnumerator Search()
    {
        for (int i = 0; i < amountOfSearches - 1; i++)
        {
            Vector3 searchPoint = new Vector3(lastKnownPosition.position.x, lastKnownPosition.position.y, lastKnownPosition.position.z) + Random.insideUnitSphere * searchRadius;
            target.position = searchPoint;
            owner.agent.SetDestination(searchPoint);
            yield return HasPath();
        }
    }

    private bool HasPath()
    {
        return owner.agent.hasPath && owner.agent.remainingDistance <= 0.9;
    }
}
