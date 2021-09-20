using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EvadeSteering : MonoBehaviour
{
    [Header("Steering")]
    [SerializeField]
    private float steeringForce = 10;
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
    private Vector3 target;

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
            EvadePlayer();
            RotateAI();
        }
    }

    private void EvadePlayer()
    {
        anim.SetBool("walk", true);

        target = player.transform.position + (player.transform.localRotation * new Vector3(0, 0, predictValue));

        var desiredVelocity = target - transform.position;
        desiredVelocity = desiredVelocity.normalized * maxVelocity;

        var steering = desiredVelocity - velocity;
        steering = Vector3.ClampMagnitude(steering, maxForce);
        steering.y = 0;
        steering /= steeringForce;

        velocity = Vector3.ClampMagnitude(velocity + steering, maxVelocity); //resultant velocity
        rb.velocity = -velocity;

        Debug.DrawRay(transform.position, steering * 100, Color.green);
        Debug.DrawRay(transform.position, velocity.normalized * 1, Color.cyan);
        Debug.DrawRay(transform.position, desiredVelocity.normalized * 2, Color.magenta);
    }

    private void RotateAI()
    {
        float step = maxForce * Time.deltaTime;
        newDir = Vector3.RotateTowards(transform.forward, -velocity, step, 0.0f);
        rb.transform.rotation = Quaternion.LookRotation(newDir);
    }
}
