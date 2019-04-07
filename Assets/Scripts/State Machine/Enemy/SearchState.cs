using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SearchState : IState
{
    float searchTime = 2f;
    float searchTimer = 0f;
    float searchRadius = 15f;
    float destinationReachedThreshold = 0.5f;
    int amountOfSearches = 2;
    bool isSearching;

    Unit owner;
    Vector3 target;
    Vector3 lastKnownPosition;

    public SearchState(Unit _owner)
    {
        this.owner = _owner; // Register this chase state to its owner.
    }

    public void Enter()
    {
        lastKnownPosition = owner.agent.destination;
        target = lastKnownPosition;
        searchTimer = searchTime;
    }

    public void Execute()
    {
        if (GameManager.instance.detected)
        {
            owner.stateMachine.ChangeState(new AttackState(this.owner));
            return;
        }

        CheckDestinationReached();

        if (isSearching)
        {
            if (searchTimer <= 0f) // If timer is done.
            {
                isSearching = false;
                amountOfSearches -= 1;
                Debug.Log("Finished Searching.");

                if (amountOfSearches <= 0)
                {
                    Debug.Log("Returning to patrol.");
                    owner.stateMachine.ChangeState(new PatrolState(this.owner));
                    return;
                }

                searchTimer = searchTime;
                target = RandomPositionFromLastKnown(lastKnownPosition);
            }
            else
            {
                searchTimer -= Time.deltaTime;
            }
        }
        else
        {
            owner.agent.SetDestination(target);
        }
    }

    public void Exit()
    {
        // TODO: If nothing found then set the global alerted to false
    }

    private void CheckDestinationReached()
    {
        float distanceToTarget = owner.agent.remainingDistance;

        if (distanceToTarget < destinationReachedThreshold)
        {
            Debug.Log("Destination Reached");
            isSearching = true;
            target = RandomPositionFromLastKnown(lastKnownPosition);
        }
    }


    private Vector3 RandomPositionFromLastKnown(Vector3 lastPos)
    {
        Vector3 searchPoint = lastPos + Random.insideUnitSphere * searchRadius;
        NavMeshHit hit;
        NavMesh.SamplePosition(owner.transform.position, out hit, searchRadius, 1);
        Debug.Log(hit.position);
        return hit.position;
    }
}