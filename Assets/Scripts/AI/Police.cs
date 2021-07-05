using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Police : MonoBehaviour
{
    [Header("AI Movement")]
    [SerializeField]
    private float minSpeed = 20f;
    [SerializeField]
    private float maxSpeed = 50f;
    [SerializeField]
    private float forceMultiplier = 50f;
    [SerializeField, Tooltip("Time interval for new wander point randomizer")]
    private float timeInterval = 5f;
    [SerializeField]
    private float chaseSpeed = 30f;
    private float moveSpeed;
    private float delta;
    private bool newWanderDirection;

    [Header("AI Detection")]
    [SerializeField]
    private float radius = 2f;
    [SerializeField]
    private LayerMask layerMask;
    private Vector3 center;
    private bool playerInRange;

    private GameObject player;
    private Transform playerTransform;

    private Rigidbody rb;
    private Animator anim;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerInRange = false;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        anim = GetComponent<Animator>();

        newWanderDirection = false;
        StartCoroutine(ChangeDirection(0));
    }

    private void FixedUpdate()
    {
        if (!newWanderDirection)
        {
            StartCoroutine(ChangeDirection(timeInterval));
        }

        if (player != null)
        {
            DetectPlayer();
            if (!playerInRange)
            {
                WalkForward();
            }
            RotateAI();
        }
    }

    private void WalkForward()
    {
        float step = moveSpeed * Time.deltaTime;
        rb.AddForce(forceMultiplier * step * transform.forward, ForceMode.Acceleration);
        //rb.velocity = (arrivalPoint - transform.position).normalized * step;
        anim.SetBool("walk", true);
    }

    private void RotateAI()
    {
        if (!playerInRange)
        {
            transform.localRotation = Quaternion.Euler(0, delta, 0);
        }
        else
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            rb.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
    }

    private IEnumerator ChangeDirection(float time)
    {
        newWanderDirection = true;
        moveSpeed = Random.Range(minSpeed, maxSpeed);
        delta = Random.Range(0, 360);
        yield return new WaitForSeconds(time);
        newWanderDirection = false;
    }

    private void GetPlayerPosition()
    {
        playerTransform = player.transform;
    }

    private void DetectPlayer()
    {
        GetPlayerPosition();

        center = transform.position;
        if(Physics.CheckSphere(center, radius, layerMask))
        {
            ChasePlayer();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(center, radius);
    }

    private void ChasePlayer()
    {
        playerInRange = true;
        moveSpeed = chaseSpeed;

        float step = moveSpeed * Time.deltaTime;
        rb.velocity = (playerTransform.position - transform.position).normalized * step;
        anim.SetBool("walk", true);

        if (playerInRange)
        {
            if (Vector3.Distance(transform.position, playerTransform.position) > radius)
            {
                playerInRange = false;
            }
        }
    }
}
