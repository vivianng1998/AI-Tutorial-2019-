﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderSteer : MonoBehaviour
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
    //private float dirX, dirZ;
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
        #region randomize facing direction (old code)
        //dirX = Random.Range(-1f, 1f);
        //dirZ = Random.Range(-1f, 1f);
        #endregion
        spherePos = new Vector3((transform.position.x + sphereDist), transform.position.y + 2f, transform.position.z);
        randomPos = spherePos + (Random.insideUnitSphere * sphereRadius);
        Debug.Log(randomPos);
        yield return new WaitForSeconds(time);
        newWanderDirection = false;
    }

    private void WalkForward()
    {
        anim.SetBool("walk", true);
        #region randomize facing direction (old code)
        //var desiredVelocity = new Vector3(dirX, 0, dirZ);
        #endregion
        var desiredVelocity = randomPos - transform.position;
        desiredVelocity = desiredVelocity.normalized * moveVelocity;

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spherePos, sphereRadius);
    }

    private void RotateAI()
    {
        float step = maxForce * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, velocity, step, 0.0f);
        rb.transform.rotation = Quaternion.LookRotation(newDir);
    }
}