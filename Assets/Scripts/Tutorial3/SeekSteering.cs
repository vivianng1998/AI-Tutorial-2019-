using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SeekSteering : MonoBehaviour
{
    [Header("Steering")]
    public float steeringForce = 100;
    public float maxVelocity = 2;
    public float maxForce = 3;
    private Vector3 velocity;

    [SerializeField]
    private GameObject target;
    private Transform targetTransform;

    private Rigidbody rb;
    private Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        anim = GetComponent<Animator>();

        GetPlayerPosition();
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            ChasePlayer();
            RotateAI();
        }
    }

    private void GetPlayerPosition()
    {
        targetTransform = target.transform;
    }

    private void ChasePlayer()
    {
        anim.SetBool("walk", true);

        var desiredVelocity = targetTransform.position - transform.position;
        desiredVelocity = desiredVelocity.normalized * maxVelocity;

        var steering = desiredVelocity - velocity;
        steering = Vector3.ClampMagnitude(steering, maxForce);
        steering.y = 0;
        steering /= steeringForce;

        velocity = Vector3.ClampMagnitude(velocity + steering, maxVelocity); //resultant velocity
        rb.velocity = velocity;

        Debug.DrawRay(transform.position, steering * 50, Color.green);
        Debug.DrawRay(transform.position, velocity.normalized * 5, Color.cyan);
        Debug.DrawRay(transform.position, desiredVelocity.normalized * 5, Color.yellow);
    }

    private void RotateAI()
    {
        float step = maxForce * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, velocity, step, 0.0f);
        rb.transform.rotation = Quaternion.LookRotation(newDir);
    }
}
