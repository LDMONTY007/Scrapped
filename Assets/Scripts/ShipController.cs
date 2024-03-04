using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    private float _health = 100f;

    public float health { get { return _health; } set { _health = Mathf.Clamp(value, 0f, 100f); } }

    Rigidbody rb;
    public Camera cam;
    Vector3 input;
    Vector3 moveVector;

    Quaternion camRotation;

    public float maxSpeed = 20f;
    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;

    //Shift slows the player down to zero.
    bool shouldBrake => Input.GetKey(KeyCode.LeftShift);

    //up axis for up/down movement
    //Space -> Up
    //LeftCtrl -> Down
    float upAxis => (Input.GetKey(KeyCode.Space) ? 1 : 0) + (Input.GetKey(KeyCode.LeftControl) ? -1 : 0);

    //__I__
    //_JKL_
    //For rotation.
    Vector3 camInput => new Vector3((Input.GetKey(KeyCode.J) ? 1 : 0) + (Input.GetKey(KeyCode.L) ? -1 : 0), (Input.GetKey(KeyCode.I) ? 1 : 0) + (Input.GetKey(KeyCode.K) ? -1 : 0), (Input.GetKey(KeyCode.Q) ? 1 : 0) + (Input.GetKey(KeyCode.E) ? -1 : 0));

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        camRotation = rb.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        input = new Vector3(Input.GetAxisRaw("Horizontal"), upAxis, Input.GetAxisRaw("Vertical"));
        //Vector projection to map inputs to match the orientation of the ship/camera.
        moveVector = transform.right * input.x + transform.forward * input.z + transform.up * input.y;
        moveVector = moveVector * moveSpeed;
        
        //Rotation
        
        camRotation = camRotation * Quaternion.Euler(new Vector3(-camInput.y, -camInput.x, camInput.z) * Time.deltaTime * rotationSpeed);
    }

    private void FixedUpdate()
    {
        if (shouldBrake)
        {
            //Apply opposite force to stop the ship.
            //Velocity * mass = force.
            rb.AddForce(-rb.velocity * rb.mass);
            if (rb.velocity.magnitude < 0.1)
            {
                rb.velocity = Vector3.zero;
            }
        }
        else
        {
            rb.AddForce(moveVector);
        }

        //Clamp to max speed.
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);

        if (transform.rotation != camRotation)
            rb.MoveRotation(camRotation);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.relativeVelocity.magnitude);
        if (collision.relativeVelocity.magnitude >= 1)
        {
            health -= collision.relativeVelocity.magnitude * 0.75f;
        }
    }
}
