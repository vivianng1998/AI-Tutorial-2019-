using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class T6Guard : MonoBehaviour
{
    private enum GuardState
    {
        PATROL,
        CHASE,
    }

    private GuardState guardState;
    private NavMeshAgent agent;

    [Header("Waypoint")]
    [SerializeField]
    private Transform[] waypoint;
    private int currentPoint = 0;
    private bool reached;

    [Header("Enemy Detection")]
    [SerializeField]
    private Transform enemy;
    [SerializeField]
    private float radius = 2f;
    [SerializeField]
    private LayerMask enemyLayerMask;
    private Vector3 center;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        reached = false;
    }

    private void FixedUpdate()
    {
        switch (guardState)
        {
            case GuardState.PATROL:
                Patrol();
                break;
            case GuardState.CHASE:
                if (GameManager.instance.treasureStolen == true && GameManager.instance.thiefHidden == true)
                {
                    guardState = GuardState.PATROL;
                }
                else
                {
                    agent.SetDestination(enemy.position);
                }
                if (Vector3.Distance(enemy.transform.position, transform.position) < 1f)
                {
                    GameManager.instance.GameOver();
                }
                break;
        }

        RotateAI();
        DetectEnemy();

        if (GameManager.instance.treasureStolen == true && GameManager.instance.thiefHidden == false)
        {
            guardState = GuardState.CHASE;
        }
    }

    private void Patrol()
    {
        if (reached)
        {
            reached = false;
            if (currentPoint < waypoint.Length - 1)
            {
                currentPoint++;
            }
            else
            {
                currentPoint = 0;
            }
        }
        else
        {
            if (Vector3.Distance(waypoint[currentPoint].position, transform.position) < 0.3f)
            {
                reached = true;
            }
            else
            {
                agent.SetDestination(waypoint[currentPoint].position);
            }
        }
    }

    private void RotateAI()
    {
        float step = agent.speed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, agent.velocity, step, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDir);
    }

    private void DetectEnemy()
    {
        center = transform.position;
        if (Physics.CheckSphere(center, radius, enemyLayerMask))
        {
            if (GameManager.instance.thiefHidden == false)
            {
                guardState = GuardState.CHASE;
            }
        }
        else
        {
            guardState = GuardState.PATROL;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, radius);
    }
}
