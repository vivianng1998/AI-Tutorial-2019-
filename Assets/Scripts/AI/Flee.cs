using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : MonoBehaviour
{
    [Header("AI Movement")]
    [SerializeField]
    private float moveSpeed = 50f;
    [SerializeField]
    private float forceMultiplier = 50f;
    [SerializeField]
    private float rotateSpeed = 5f;

    [Header("AI Detection")]
    [SerializeField]
    private float radius = 2f;
    [SerializeField]
    private LayerMask layerMask;
    private Vector3 center;

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
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            DetectPlayer();
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
        float step = moveSpeed * Time.deltaTime;
        rb.AddForce(forceMultiplier * step * transform.forward, ForceMode.Acceleration);
        //rb.velocity = (playerTransform.position - transform.position).normalized * step;
        anim.SetBool("walk", true);

        if (Vector3.Distance(transform.position, playerTransform.position) > radius)
        {
            anim.SetBool("walk", false);
        }
    }

    private void RotateAI()
    {
        float step = rotateSpeed * Time.deltaTime;
        Vector3 target = playerTransform.position - transform.position;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, -target, step, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDir);
    }
}
