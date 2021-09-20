using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gangster : MonoBehaviour
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
    [SerializeField]
    private GameObject police;
    private Transform policeTransform;
    private bool policeInRange;
    [SerializeField]
    private GameObject thief;
    private Transform thiefTransform;
    private Thief thiefScript;

    private Rigidbody rb;
    private Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        anim = GetComponent<Animator>();
        thiefScript = thief.GetComponent<Thief>();
    }

    private void FixedUpdate()
    {
        if (police != null && thief != null)
        {
            DetectPolice();
            if (!policeInRange && thiefScript.hasArrived)
            {
                ChaseThief();
                RotateAI(thiefTransform, false);
            }
            else if (policeInRange)
            {
                EscapePolice();
                RotateAI(policeTransform, true);
            }
        }
    }

    private void DetectPolice()
    {
        GetTargetPosition();

        center = transform.position;
        if (Physics.CheckSphere(center, radius, layerMask))
        {
            policeInRange = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(center, radius);
    }

    private void GetTargetPosition()
    {
        policeTransform = police.transform;
        thiefTransform = thief.transform;
    }

    private void EscapePolice()
    {
        float step = moveSpeed * Time.deltaTime;
        rb.AddForce(forceMultiplier * step * transform.forward, ForceMode.Acceleration);
        //rb.velocity = (playerTransform.position - transform.position).normalized * step;
        anim.SetBool("walk", true);

        if (Vector3.Distance(transform.position, policeTransform.position) > radius)
        {
            anim.SetBool("walk", false);
            policeInRange = false;
        }
    }

    private void ChaseThief()
    {
        float step = moveSpeed * Time.deltaTime;
        //rb.AddForce(transform.forward * step * 10, ForceMode.Acceleration);
        rb.velocity = (thiefTransform.position - transform.position).normalized * step;
        anim.SetBool("walk", true);
    }

    private void RotateAI(Transform target, bool isEscaping)
    {
        if (isEscaping)
        {
            rotateDirection = -(target.position - transform.position).normalized;
        }
        else
        {
            rotateDirection = (target.position - transform.position).normalized;
        }
        
        rb.transform.rotation = Quaternion.LookRotation(rotateDirection, Vector3.up);
    }
}
