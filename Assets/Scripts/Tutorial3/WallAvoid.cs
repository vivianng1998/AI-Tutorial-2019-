using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WallAvoid : MonoBehaviour
{
    [Header("Steering")]
    [SerializeField]
    private float steeringForce = 10;
    [SerializeField]
    private float maxVelocity = 2;
    [SerializeField]
    private float maxForce = 3;
    private Vector3 velocity;
    private Vector3 desiredVelocity;
    private Vector3 steering;

    private Vector3 target;
    private bool hasObstacle;
    [SerializeField]
    private float avoidDistance = 1;

    private Rigidbody rb;
    private Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        Ray ray = new Ray(transform.position + new Vector3(0, 0.3f, 0), transform.forward);
        Ray leftRay = new Ray(transform.position + new Vector3(0, 0.3f, 0), (Quaternion.Euler(0, -15f, 0) * transform.forward));
        Ray rightRay = new Ray(transform.position + new Vector3(0, 0.3f, 0), (Quaternion.Euler(0, 15f, 0) * transform.forward));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, avoidDistance) || Physics.Raycast(leftRay, out hit, avoidDistance) || Physics.Raycast(rightRay, out hit, avoidDistance))
        {
            hasObstacle = true;
            target = hit.point + hit.normal * avoidDistance;
        }
        else
        {
            hasObstacle = false;
        }
        Debug.DrawRay(ray.origin, ray.direction * avoidDistance, Color.red);
        Debug.DrawRay(leftRay.origin, leftRay.direction * (avoidDistance), Color.blue);
        Debug.DrawRay(rightRay.origin, rightRay.direction * (avoidDistance), Color.blue);
    }

    private void FixedUpdate()
    {
        MoveForward();
        RotateAI();
    }

    private void MoveForward()
    {
        anim.SetBool("walk", true);

        if (hasObstacle)
        {
            desiredVelocity = (target - transform.position).normalized;
            desiredVelocity.y = 0;
            desiredVelocity *= maxVelocity;

            steering = desiredVelocity - velocity;
            steering = Vector3.ClampMagnitude(steering, maxForce);
            steering.y = 0;
            steering /= steeringForce;

            velocity = Vector3.ClampMagnitude(velocity + steering, maxVelocity); //resultant velocity
            rb.velocity = velocity;
        }
        else
        {
            steering = Vector3.zero;
            rb.velocity = transform.forward;
        }

        Debug.DrawRay(transform.position, steering * 100, Color.green);
        Debug.DrawRay(transform.position, velocity.normalized * 1, Color.cyan);
        Debug.DrawRay(transform.position, desiredVelocity.normalized * 2, Color.magenta);
    }

    private void RotateAI()
    {
        float step = maxForce * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, velocity, step, 0.0f);
        rb.transform.rotation = Quaternion.LookRotation(newDir);
    }
}
