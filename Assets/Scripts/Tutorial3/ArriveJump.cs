using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArriveJump : MonoBehaviour, IJump
{
    [Header("Steering")]
    [SerializeField]
    private float steeringForce = 10;
    [SerializeField]
    private float maxVelocity = 2;
    [SerializeField]
    private float maxForce = 3;
    private Vector3 velocity;
    [SerializeField]
    private float slowdownRate = 0.1f;
    private float initialDrag;
    [SerializeField]
    private float maxDrag = 30f;

    [SerializeField]
    private Transform destination;

    private Rigidbody rb;
    private Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        initialDrag = rb.drag;

        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        ArriveToPoint();
        RotateAI();
    }

    private void ArriveToPoint()
    {
        anim.SetBool("walk", true);

        var desiredVelocity = destination.position - transform.position;
        desiredVelocity = desiredVelocity.normalized * maxVelocity;

        var steering = desiredVelocity - velocity;
        steering = Vector3.ClampMagnitude(steering, maxForce);
        steering.y = 0;
        steering /= steeringForce;

        velocity = Vector3.ClampMagnitude(velocity + steering, maxVelocity); //resultant velocity
        //rb.velocity = velocity;

        Vector3 adjustedVelocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
        rb.velocity = adjustedVelocity;

        Debug.DrawRay(transform.position, steering * 100, Color.green);
        Debug.DrawRay(transform.position, velocity.normalized * 1, Color.cyan);
        Debug.DrawRay(transform.position, desiredVelocity.normalized * 2, Color.magenta);

        if (Vector3.Distance(transform.position, destination.position) < 1f)
        {
            anim.SetBool("walk", false);
            rb.velocity = Vector3.zero;
            rb.drag = initialDrag;
        }
        else if (Vector3.Distance(transform.position, destination.position) < 5f)
        {
            if (rb.drag < maxDrag)
            {
                rb.drag += slowdownRate * Time.deltaTime;
            }
            else
            {
                rb.drag = maxDrag;
            }
        }

    }

    private void RotateAI()
    {
        float step = maxForce * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, velocity, step, 0.0f);
        rb.transform.rotation = Quaternion.LookRotation(newDir);
    }

    public void Jump(float speedReq ,Vector3 direction)
    {
        if (rb.velocity.magnitude > speedReq)
        {
            anim.SetTrigger("jump");
            rb.AddForce(direction, ForceMode.Impulse);
        }
    }
}
