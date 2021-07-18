using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeSteering : MonoBehaviour
{
    [Header("Steering")]
    [SerializeField]
    private float steeringForce = 100;
    [SerializeField]
    private float maxVelocity = 2;
    [SerializeField]
    private float maxForce = 3;
    private Vector3 velocity;

    [Header("AI Detection")]
    [SerializeField]
    private float radius = 2f;
    [SerializeField]
    private LayerMask layerMask;
    private Vector3 center;

    [SerializeField]
    private GameObject player;
    private Transform playerTransform;
    
    private Rigidbody rb;
    private Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            DetectPlayer();
            RotateAI();
        }
    }

    private void DetectPlayer()
    {
        GetPlayerPosition();

        center = transform.position;
        if (Physics.CheckSphere(center, radius, layerMask))
        {
            EscapePlayer();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(center, radius);
    }

    private void GetPlayerPosition()
    {
        playerTransform = player.transform;
    }

    private void EscapePlayer()
    {
        anim.SetBool("walk", true);

        var desiredVelocity = playerTransform.position - transform.position;
        desiredVelocity = desiredVelocity.normalized * maxVelocity;

        var steering = desiredVelocity - velocity;
        steering = Vector3.ClampMagnitude(steering, maxForce);
        steering.y = 0;
        steering /= steeringForce;

        velocity = Vector3.ClampMagnitude(velocity + steering, maxVelocity); //resultant velocity
        rb.velocity = -velocity;

        Debug.DrawRay(transform.position, steering * 50, Color.green);
        Debug.DrawRay(transform.position, velocity.normalized * 5, Color.cyan);
        Debug.DrawRay(transform.position, desiredVelocity.normalized * 5, Color.yellow);

        if (Vector3.Distance(transform.position, playerTransform.position) > radius)
        {
            anim.SetBool("walk", false);
        }
    }

    private void RotateAI()
    {
        float step = maxForce * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, -velocity, step, 0.0f);
        rb.transform.rotation = Quaternion.LookRotation(newDir);
    }
}
