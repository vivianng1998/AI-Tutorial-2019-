using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thief : MonoBehaviour
{
    [Header("AI Movement")]
    [SerializeField]
    private float moveSpeed = 50f;
    [SerializeField]
    private float forceMultiplier = 50f;
    private Vector3 rotateDirection;

    [Header("AI Detection")]
    [SerializeField]
    private float radius = 2f;
    [SerializeField]
    private LayerMask layerMask;
    private Vector3 center;
    private bool playerInRange;

    [Header("Border")]
    [SerializeField]
    private Transform[] borderCorner;

    [Header("Chest")]
    [SerializeField]
    private GameObject chestPrefab;
    private Vector3 arrivalPoint;
    public bool hasArrived;

    private GameObject player;
    private Transform playerTransform;

    private Rigidbody rb;
    private Animator anim;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        anim = GetComponent<Animator>();

        hasArrived = false;
        RandomizeArrivalPoint();
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            DetectPlayer();
            if (!playerInRange)
            {
                ArriveToPoint();
            }
            RotateAI();
        }
    }

    private void DetectPlayer()
    {
        GetPlayerPosition();

        center = transform.position;
        if (Physics.CheckSphere(center, radius, layerMask))
        {
            EscapePlayer();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(center, radius);
    }

    private void GetPlayerPosition()
    {
        playerTransform = player.transform;
    }

    private void EscapePlayer()
    {
        playerInRange = true;

        float step = moveSpeed * Time.deltaTime;
        rb.AddForce(forceMultiplier * step * transform.forward, ForceMode.Acceleration);
        //rb.velocity = (playerTransform.position - transform.position).normalized * step;
        anim.SetBool("walk", true);

        if (Vector3.Distance(transform.position, playerTransform.position) > radius)
        {
            playerInRange = false;
        }
    }

    private void RandomizeArrivalPoint()
    {
        float posX = Random.Range(borderCorner[0].position.x, borderCorner[1].position.x);
        float posZ = Random.Range(borderCorner[0].position.z, borderCorner[1].position.z);
        arrivalPoint = new Vector3(posX, transform.position.y, posZ);
        Instantiate(chestPrefab, new Vector3(arrivalPoint.x, arrivalPoint.y, arrivalPoint.z), Quaternion.Euler(-90f, 0, 0));
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
        if (!playerInRange)
        {
            rotateDirection = (arrivalPoint - transform.position).normalized;
        }
        else
        {
            rotateDirection = -(playerTransform.position - transform.position).normalized;
        }
        rb.transform.rotation = Quaternion.LookRotation(rotateDirection, Vector3.up);
    }
}
