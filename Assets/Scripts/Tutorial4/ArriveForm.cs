using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArriveForm : MonoBehaviour
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
    private float oriVel;
    private float boostVel;

    public Transform formationPosition;

    private Rigidbody rb;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        oriVel = maxVelocity;
        boostVel = maxVelocity + 50f;
    }

    private void FixedUpdate()
    {
        if (formationPosition != null)
        {
            Arrive();
        }
    }

    private void Arrive()
    {
        float dist = Vector3.Distance(formationPosition.position, transform.position);

        if (dist > 0.5f)
        {
            maxVelocity = oriVel;
            if (dist > 1f)
            {
                maxVelocity = boostVel;
            }
            anim.SetBool("walk", true);
            SteeringMovement(formationPosition.position);
        }
        else
        {
            anim.SetBool("walk", false);
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
