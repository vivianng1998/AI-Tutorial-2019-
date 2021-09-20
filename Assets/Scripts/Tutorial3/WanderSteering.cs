using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderSteering : MonoBehaviour
{
    [Header("Steering")]
    [SerializeField]
    private float steeringForce = 10;
    [SerializeField]
    private float minVelocity = 1;
    [SerializeField]
    private float maxVelocity = 3;
    [SerializeField]
    private float maxForce = 3;
    private Vector3 velocity;

    [Header("Randomizer")]
    [SerializeField]
    private float timeInterval = 5f;
    [SerializeField]
    private float sphereDist = 1f;
    [SerializeField]
    private float sphereRadius = 1f;
    private Vector3 spherePos;
    private Vector3 randomPos;
    private float moveVelocity;
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
        spherePos = transform.localRotation * new Vector3(0, 0, sphereDist);
        spherePos = transform.localPosition + spherePos;
        randomPos = spherePos + Random.insideUnitSphere.normalized * sphereRadius;
        Debug.Log(spherePos);
        yield return new WaitForSeconds(time);
        newWanderDirection = false;
    }

    private void WalkForward()
    {
        anim.SetBool("walk", true);
        var desiredVelocity = randomPos - transform.position;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * sphereDist, sphereRadius);
    }

    private void RotateAI()
    {
        float step = maxForce * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, velocity, step, 0.0f);
        rb.transform.rotation = Quaternion.LookRotation(newDir);
    }
}
