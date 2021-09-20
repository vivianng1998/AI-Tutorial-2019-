using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class T6Thief : MonoBehaviour
{
    private enum ThiefState
    {
        STEAL,
        FLEE,
        HIDE
    }

    private ThiefState thiefState;
    private NavMeshAgent agent;

    [Header("Steering")]
    private float initialSpeed;
    private float maxSpeed;

    [Header("Target")]
    [SerializeField]
    private Transform treasureChest;
    [SerializeField]
    private Transform[] hidingSpot;
    private int randomHideSpot;
    private Vector3 arrivalPoint;

    [Header("Enemy Detection")]
    [SerializeField]
    private Transform enemy;
    [SerializeField]
    private float radius = 2f;
    [SerializeField]
    private LayerMask enemyLayerMask;
    private Vector3 center;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        initialSpeed = agent.speed;
        maxSpeed = initialSpeed + 1;
    }

    private void Update()
    {
        // Start stealing
        if (Input.GetKeyDown(KeyCode.E))
        {
            agent.speed = initialSpeed;
            GameManager.instance.CommenceSteal();
            thiefState = ThiefState.STEAL;
        }
    }

    private void FixedUpdate()
    {
        switch (thiefState)
        {
            case ThiefState.STEAL:
                SetTarget(treasureChest);
                if (Vector3.Distance(transform.position, arrivalPoint) < 0.3f)
                {
                    GameManager.instance.treasureStolen = true;
                    GameManager.instance.stolenText.text = "Yes";
                    randomHideSpot = Random.Range(0, 2);
                    thiefState = ThiefState.HIDE;
                }
                break;

            case ThiefState.FLEE:
                Vector3 fleeTarget = transform.position + (transform.position - enemy.transform.position);               
                SetTarget(fleeTarget);
                break;

            case ThiefState.HIDE:
                agent.speed = maxSpeed;
                SetTarget(hidingSpot[randomHideSpot]);
                if (Vector3.Distance(transform.position, arrivalPoint) < 0.5f)
                {
                    GameManager.instance.thiefHidden = true;
                }
                break;
        }     

        RotateAI();

        if (GameManager.instance.treasureStolen == false)
        {
            DetectEnemy();
        }
    }

    private void SetTarget(Transform target)
    {
        arrivalPoint = new Vector3(target.position.x, transform.position.y, target.position.z);
        agent.SetDestination(arrivalPoint);
    }
    
    private void SetTarget(Vector3 target)
    {
        arrivalPoint = new Vector3(target.x, transform.position.y, target.z);
        agent.SetDestination(arrivalPoint);
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
            thiefState = ThiefState.FLEE;
        }
        else
        {
            thiefState = ThiefState.STEAL;
        }
    }

    private void OnDrawGizmos()
    {
        if (thiefState == ThiefState.STEAL)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(center, radius);
        }
    }
}
