using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wandering : MonoBehaviour
{
    [Header("AI Movement")]
    [SerializeField]
    private float minSpeed = 10f;
    [SerializeField]
    private float maxSpeed = 50f;
    [SerializeField]
    private float forceMultiplier = 50f;
    [SerializeField, Tooltip("Time interval for new wander point randomizer")]
    private float timeInterval = 5f;
    private float moveSpeed;
    private float delta;
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
        moveSpeed = Random.Range(minSpeed, maxSpeed);
        delta = Random.Range(0, 360);
        //Debug.Log("speed " + moveSpeed + " delta " + delta);
        yield return new WaitForSeconds(time);
        newWanderDirection = false;
    }

    private void WalkForward()
    {
        float step = moveSpeed * Time.deltaTime;
        rb.AddForce(forceMultiplier * step * transform.forward, ForceMode.Acceleration);
        //rb.velocity = (arrivalPoint - transform.position).normalized * step;
        anim.SetBool("walk", true);
    }

    private void RotateAI()
    {
        transform.localRotation = Quaternion.Euler(0, delta, 0);
    }
}
