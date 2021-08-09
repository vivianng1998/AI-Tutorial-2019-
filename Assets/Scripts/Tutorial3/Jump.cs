using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    [SerializeField]
    private float jumpForce;
    private Vector3 jumpDirection;

    public float speedRequirement = 2;

    // Start is called before the first frame update
    void Start()
    {
        jumpDirection = Vector3.up * jumpForce;
    }

    private void OnTriggerStay(Collider other)
    {
        IJump iJump = other.gameObject.GetComponent<IJump>();

        if (iJump != null)
        {
            iJump.Jump(speedRequirement, jumpDirection);
        }
    }
}
