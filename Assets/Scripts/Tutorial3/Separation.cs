using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Separation : MonoBehaviour
{
    public SeparationHandler separationHandler;

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

    private GameObject[] monsters;

    Rigidbody rb;
    Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        monsters = GameObject.FindGameObjectsWithTag("Separate");
    }

    private void FixedUpdate()
    {
        Arrive();
        Vector3 separationForce = SeparationForce();
        rb.AddForce(separationForce * 10);
    }

    private Vector3 SeparationForce()
    {
        Vector3 totalSeparation = Vector3.zero;
        int numNeighbors = 0;

        foreach (GameObject monster in monsters)
        {
            Separation neighbor = monster.GetComponent<Separation>();

            Vector3 separationVector = transform.position - neighbor.transform.position;
            float distance = separationVector.magnitude;

            // If it's a neighbor within our vicinity
            if (distance > 0 && distance < separationHandler.desiredSeparation)
            {
                separationVector.Normalize();

                // The closer a neighbor (smaller the distance), the more we should flee
                separationVector /= distance;

                totalSeparation += separationVector;
                numNeighbors++;
            }
        }

        if (numNeighbors > 0)
        {
            // Compute its average separation vector
            Vector3 averageSeparation = totalSeparation / numNeighbors;
            averageSeparation.Normalize();
            averageSeparation *= maxSpeed;

            // Compute the separation force we need to apply
            Vector3 separationForce = averageSeparation - rb.velocity;

            // Cap that separation force
            if (separationForce.magnitude > maxForce)
            {
                separationForce.Normalize();
                separationForce *= maxForce;
            }

            separationForce.y = 0;

            return separationForce;
        }

        return Vector3.zero;
    }

    private void Arrive()
    {
        float dist = Vector3.Distance(separationHandler.destinationPoint.position, transform.position);
        if (dist > 1.5f)
        {
            anim.SetBool("walk", true);
            SteeringMovement(separationHandler.destinationPoint.position);
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
}
