using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    Rigidbody rb;
    public Camera cam;
    Vector2 input;
    Vector2 moveVector;

    public float moveSpeed = 5f;

    bool shouldBrake => Input.GetKey(KeyCode.Space);

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveVector = input * moveSpeed;
    }

    private void FixedUpdate()
    {
        if (shouldBrake)
        {
            //Apply opposite force to stop the ship.
            //Velocity * mass = force.
            rb.AddForce(-rb.velocity * rb.mass);
        }
        else
        {
            rb.AddForce(moveVector);
        }
    }
}
