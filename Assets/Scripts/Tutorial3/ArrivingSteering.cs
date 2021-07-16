using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrivingSteering : MonoBehaviour
{
    [Header("AI Movement")]
    [SerializeField]
    private float moveSpeed = 50f;
    [SerializeField]
    private float rotateSpeed = 5f;

    [Header("Border")]
    [SerializeField]
    private Transform[] borderCorner;

    private Vector3 arrivalPoint;
    private bool hasArrived;

    private Rigidbody rb;
    private Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        anim = GetComponent<Animator>();

        hasArrived = false;
        RandomizeArrivalPoint();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && hasArrived)
        {
            hasArrived = false;
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
        float step = moveSpeed * Time.deltaTime;
        //rb.AddForce(transform.forward * step * forceMultiplier, ForceMode.Acceleration);
        rb.velocity = (arrivalPoint - transform.position).normalized * step;
        anim.SetBool("walk", true);

        if (Vector3.Distance(transform.position, arrivalPoint) < 1f)
        {
            anim.SetBool("walk", false);
            rb.velocity = Vector3.zero;
            hasArrived = true;
        }
    }

    private void RotateAI()
    {
        float step = rotateSpeed * Time.deltaTime;
        Vector3 target = arrivalPoint - transform.position;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, target, step, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDir);
    }
}
