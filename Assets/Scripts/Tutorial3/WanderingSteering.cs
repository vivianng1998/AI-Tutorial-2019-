using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderingSteering : MonoBehaviour
{
    [Header("Steering")]
    [SerializeField]
    private float steeringForce = 100;
    [SerializeField]
    private float minVelocity = 1;
    [SerializeField]
    private float maxVelocity = 3;
    [SerializeField]
    private float maxForce = 3;
    private Vector3 velocity;

    [SerializeField, Tooltip("Time interval for new wander point randomizer")]
    private float timeInterval = 5f;
    private float moveVelocity;
    private float dirX, dirZ;
    private bool newWanderDirection;

    private Rigidbody rb;
    private Animator anim;

    private void Start()
    {
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
        WalkForward();
        RotateAI();
    }

    private IEnumerator ChangeDirection(float time)
    {
        newWanderDirection = true;
        moveVelocity = Random.Range(minVelocity, maxVelocity);
        dirX = Random.Range(-1f, 1f);
        dirZ = Random.Range(-1f, 1f);
        yield return new WaitForSeconds(time);
        newWanderDirection = false;
    }

    private void WalkForward()
    {
        anim.SetBool("walk", true);

        var desiredVelocity = new Vector3(dirX, 0, dirZ);
        desiredVelocity = desiredVelocity.normalized * moveVelocity;

        var steering = desiredVelocity - velocity;
        steering = Vector3.ClampMagnitude(steering, maxForce);
        steering.y = 0;
        steering /= steeringForce;

        velocity = Vector3.ClampMagnitude(velocity + steering, maxVelocity); //resultant velocity
        rb.velocity = velocity;

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
