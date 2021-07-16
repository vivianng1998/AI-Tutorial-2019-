using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SeekSteering : MonoBehaviour
{
    //[Header("AI Movement")]
    //[SerializeField]
    //private float moveSpeed = 50f;
    //[SerializeField]
    //private float rotateSpeed = 5f;

    private GameObject target;
    private Transform targetTransform;

    private Rigidbody rb;
    private Animator anim;

    #region steer
    [Header("Steer")]
    public float steeringForce = 15;
    public float maxVelocity = 3;
    public float maxForce = 15;
    private Vector3 velocity;
    #endregion

    private void Start()
    {
        target = GameObject.FindWithTag("Player");

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
        //float step = moveSpeed * Time.deltaTime;
        ////rb.AddForce(transform.forward * step * 10, ForceMode.Acceleration);
        //rb.velocity = (playerTransform.position - transform.position).normalized * step;
        anim.SetBool("walk", true);

        #region steer
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
        #endregion

        //if (Vector3.Distance(transform.position, playerTransform.position) < 1f)
        //{
        //    anim.SetBool("walk", false);
        //    rb.velocity = Vector3.zero;
        //    GetPlayerPosition();
        //}
    }

    private void RotateAI()
    {
        float step = maxForce * Time.deltaTime;
        //Vector3 target = targetTransform.position - transform.position;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, velocity, step, 0.0f);
        rb.transform.rotation = Quaternion.LookRotation(newDir);

        //Vector3 direction = (targetTransform.position - transform.position).normalized;
        //Vector3 direction = velocity.normalized;
        //rb.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
}
