using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrivingSteering : MonoBehaviour
{
    [Header("Steering")]
    [SerializeField]
    private float steeringForce = 100;
    [SerializeField]
    private float maxVelocity = 2;
    [SerializeField]
    private float maxForce = 3;
    private Vector3 velocity;

    [Header("Border")]
    [SerializeField]
    private Transform[] borderCorner;

    private Vector3 arrivalPoint;

    private Rigidbody rb;
    private Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        anim = GetComponent<Animator>();

        RandomizeArrivalPoint();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RandomizeArrivalPoint();
        }
    }

    private void FixedUpdate()
    {
        ArriveToPoint();
        RotateAI();
    }

    private void RandomizeArrivalPoint()
    {
        float posX = Random.Range(borderCorner[0].position.x, borderCorner[1].position.x);
        float posZ = Random.Range(borderCorner[0].position.z, borderCorner[1].position.z);
        arrivalPoint = new Vector3(posX, transform.position.y, posZ);
        Debug.Log(arrivalPoint);
    }

    private void ArriveToPoint()
    {
        anim.SetBool("walk", true);

        var desiredVelocity = arrivalPoint - transform.position;
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

        if (Vector3.Distance(transform.position, arrivalPoint) < 1f)
        {
            //arrived
        }
    }

    private void RotateAI()
    {
        float step = maxForce * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, velocity, step, 0.0f);
        rb.transform.rotation = Quaternion.LookRotation(newDir);
    }
}
