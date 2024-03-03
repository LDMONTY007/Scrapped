using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    Rigidbody rb;
    public Camera cam;
    Vector3 input;
    Vector3 moveVector;

    public float moveSpeed = 5f;

    //Shift slows the player down to zero.
    bool shouldBrake => Input.GetKey(KeyCode.LeftShift);

    //up axis for up/down movement
    //Space -> Up
    //LeftCtrl -> Down
    float upAxis => (Input.GetKey(KeyCode.Space) ? 1 : 0) + (Input.GetKey(KeyCode.LeftControl) ? -1 : 0);

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        input = new Vector3(Input.GetAxisRaw("Horizontal"), upAxis, Input.GetAxisRaw("Vertical"));
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
