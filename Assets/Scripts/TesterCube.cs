using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TesterCube : MonoBehaviour
{
    Rigidbody rb;
    public float moveSpeed;
    public bool isJumping = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isJumping)
        {
            rb.velocity = transform.forward * moveSpeed;
        }
    }
}
