using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollowing : MonoBehaviour
{
    [Header("Steering")]
    [SerializeField]
    private float maxForce = 15f;
    [SerializeField]
    private float maxSpeed = 10f;
    [SerializeField]
    private float maxVelocity = 100f;
    [SerializeField]
    private float mass = 15f;
    private Vector3 velocity;

    [Header("Waypoint")]
    [SerializeField]
    private Transform[] waypoint;
    private int currentPoint = 0;
    private bool reached;
    private bool completedPath;
    private bool reverse;
    [SerializeField]
    private float timer = 5;
    private float initialTimer;

    private Rigidbody rb;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        reached = false;
        completedPath = false;
        reverse = false;
        initialTimer = timer;
    }

    private void FixedUpdate()
    {
        FollowPath();
        Arrive();
    }

    private void FollowPath()
    {
        if (!completedPath && reached)
        {
            reached = false;
            if (!reverse)
            {
                if (currentPoint < waypoint.Length - 1)
                {
                    currentPoint++;
                }
                else
                {
                    completedPath = true;
                    reverse = true;
                }
            }
            else
            {
                if (currentPoint > 0)
                {
                    currentPoint--;
                }
                else
                {
                    completedPath = true;
                    reverse = false;
                }
            }
        }

        if (completedPath)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                Debug.Log("Timer reset");
                reached = false;
                completedPath = false;

                timer = initialTimer;
            }
        }
    }

    private void Arrive()
    {
        float dist = Vector3.Distance(waypoint[currentPoint].position, transform.position);

        if (dist > 0.5f)
        {
            anim.SetBool("walk", true);
            SteeringMovement(waypoint[currentPoint].position);
        }
        else
        {
            anim.SetBool("walk", false);
            reached = true;
        }
    }

    void SteeringMovement(Vector3 targetPosition)
    {
        Vector3 desiredVelocity = targetPosition - transform.position;
        desiredVelocity = maxVelocity * Time.deltaTime * desiredVelocity.normalized;

        Vector3 steering = desiredVelocity - velocity;
        steering.y = 0;
        steering = Vector3.ClampMagnitude(steering, maxForce);
        steering /= mass;

        velocity = Vector3.ClampMagnitude(velocity + steering, maxSpeed);
        rb.velocity = velocity;

        transform.localRotation = Quaternion.LookRotation(rb.velocity);

        Debug.DrawRay(transform.position, steering * 50, Color.blue);
        Debug.DrawRay(transform.position, velocity.normalized * 5, Color.black);
        Debug.DrawRay(transform.position, desiredVelocity.normalized * 5, Color.red);
    }

    private void RotateAI()
    {
        float step = maxForce * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, velocity, step, 0.0f);
        rb.transform.rotation = Quaternion.LookRotation(newDir);
    }
}
