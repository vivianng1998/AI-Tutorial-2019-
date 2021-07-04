using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : MonoBehaviour
{
    [Header("AI Movement")]
    [SerializeField]
    private float moveSpeed = 50f;
    [SerializeField]
    private float rotateSpeed = 5f;

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

        GetPlayerPosition();
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            ChasePlayer();
            RotateAI();
        }
    }

    private void GetPlayerPosition()
    {
        playerTransform = player.transform;
    }

    private void ChasePlayer()
    {
        float step = moveSpeed * Time.deltaTime;
        //transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, step);
        //rb.AddForce(transform.forward * step * 10, ForceMode.Acceleration);
        rb.velocity = (playerTransform.position - transform.position).normalized * step;
        anim.SetBool("walk", true);
        
        if (Vector3.Distance(transform.position, playerTransform.position) < 1f)
        {
            anim.SetBool("walk", false);
            rb.velocity = Vector3.zero;
            GetPlayerPosition();
        }
    }

    private void RotateAI()
    {
        float step = rotateSpeed * Time.deltaTime;
        Vector3 target = playerTransform.position - transform.position;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, target, step, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDir);
    }
}
