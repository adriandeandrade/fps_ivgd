using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] bool patrolWaiting;
    [SerializeField] float totalWaitTime = 3f;
    [SerializeField] float switchProbability = 0.2f;
    [SerializeField] List<PatrolWaypoint> patrolPoints;

    NavMeshAgent navMeshAgent;
    int currentPatrolIndex;
    bool travelling;
    bool waiting;
    bool patrolForward;
    float waitTimer;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = this.GetComponent<NavMeshAgent>();

        if (navMeshAgent == null)
        {
            Debug.LogError("The nav mesh agent component is not attached to" + gameObject.name);
        }
        else
        {
            if(patrolPoints != null && patrolPoints.Count >= 2)
            {
                currentPatrolIndex = 0;
                SetDestination();
            }
            else
            {
                Debug.Log("Insufficient patrol points for basic patrolling behaviour");
            }
        }
    }

    private void Update()
    {
        if (travelling && navMeshAgent.remainingDistance <= 1.0f)
        {
            travelling = false;

            if (patrolWaiting)
            {
                waiting = true;
                waitTimer = 0f;
            }
            else
            {
                ChangePatrolPoint();
                SetDestination();
            }
        }

        if (waiting)
        {
            waitTimer += Time.deltaTime;
            if(waitTimer >= totalWaitTime)
            {
                waiting = false;

                ChangePatrolPoint();
                SetDestination();
            }
        }
    }

    private void SetDestination()
    {
        if(patrolPoints != null)
        {
            Vector3 targetVector = patrolPoints[currentPatrolIndex].transform.position;
            navMeshAgent.SetDestination(targetVector);
            travelling = true;
        }
    }

    private void ChangePatrolPoint()
    {
        if(UnityEngine.Random.Range(0f, 1f) <= switchProbability)
        {
            patrolForward = !patrolForward;
        }

        if (patrolForward)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
        }
        else
        {
            if(--currentPatrolIndex < 0)
            {
                currentPatrolIndex = patrolPoints.Count - 1;
            }
        }
    }
}
