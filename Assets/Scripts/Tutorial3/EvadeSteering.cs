using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EvadeSteering : MonoBehaviour
{
    [Header("Steering")]
    [SerializeField]
    private float steeringForce = 100;
    [SerializeField]
    private float maxVelocity = 2;
    [SerializeField]
    private float maxForce = 3;
    private Vector3 velocity;
    private Vector3 newDir;

    [Header("AI Detection")]
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private float predictValue = 1;
    [SerializeField]
    private float radius = 2f;
    [SerializeField]
    private LayerMask layerMask;
    private Vector3 center;
    private bool playerInRange;
    private Vector3 target;
    [SerializeField]
    private float delayValue = 2;
    private float timer;

    private Rigidbody rb;
    private Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        anim = GetComponent<Animator>();

        timer = delayValue;
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            DetectPlayer();
            PursuePlayer();
            RotateAI();
        }
    }

    private void PursuePlayer()
    {
        anim.SetBool("walk", true);

        if (!playerInRange)
        {
            target = player.transform.position - new Vector3(predictValue, 0, predictValue);
        }
        else
        {
            target = player.transform.position;
        }

        var desiredVelocity = target - transform.position;
        desiredVelocity = desiredVelocity.normalized * maxVelocity;

        var steering = desiredVelocity - velocity;
        steering = Vector3.ClampMagnitude(steering, maxForce);
        steering.y = 0;
        steering /= steeringForce;

        velocity = Vector3.ClampMagnitude(velocity + steering, maxVelocity); //resultant velocity

        if (!playerInRange)
        {
            rb.velocity = velocity;
        }
        else
        {
            rb.velocity = -velocity;
        }

        Debug.DrawRay(transform.position, steering * 50, Color.green);
        Debug.DrawRay(transform.position, velocity.normalized * 5, Color.cyan);
        Debug.DrawRay(transform.position, desiredVelocity.normalized * 10, Color.yellow);
    }

    private void DetectPlayer()
    {
        center = transform.position;
        if (Physics.CheckSphere(center, radius, layerMask))
        {
            playerInRange = true;
        }

        if (playerInRange)
        {
            if (Vector3.Distance(transform.position, player.transform.position) > radius)
            {
                timer -= Time.deltaTime;
                if (delayValue <= 0)
                {
                    playerInRange = false;
                    timer = delayValue;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(center, radius);
    }

    private void RotateAI()
    {
        float step = maxForce * Time.deltaTime;
        if (!playerInRange)
        {
            newDir = Vector3.RotateTowards(transform.forward, velocity, step, 0.0f);
        }
        else
        {
            newDir = Vector3.RotateTowards(transform.forward, -velocity, step, 0.0f);
        }
        rb.transform.rotation = Quaternion.LookRotation(newDir);
    }
}
