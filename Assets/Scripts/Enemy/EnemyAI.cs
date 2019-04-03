using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    Transform target;
    NavMeshAgent agent;
    Animator anim;
    public float maxSight;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, target.position);

        if(distance < maxSight)
        {
            agent.updatePosition = true;
            agent.SetDestination(target.position);
        }
        else
        {
            agent.updatePosition = false;
        }
    }
}
