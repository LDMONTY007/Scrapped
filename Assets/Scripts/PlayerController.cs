using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float _oxygen = 100f;

    public ShipController shipController;

    public float oxygen { get { return _oxygen; } set { _oxygen = Mathf.Clamp(value, 0f, 100f); } }

    Rigidbody rb;
    public GameObject cam;
    public TextMeshProUGUI oxygenText;
    Vector3 input;
    Vector3 moveVector;



    Quaternion camRotation;
    Vector3 curCamRot = Vector3.zero;

    public float maxSpeed = 20f;
    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;
    public bool isControllingShip;

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


    bool shouldAlignWithShip => Input.GetKey(KeyCode.G);

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StopControllingShip();
        camRotation = rb.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isControllingShip)
            {
                StopControllingShip();
            }
            else
            {
                StartControllingShip();
            }
        }

        if (!isControllingShip)
        {
            input = new Vector3(Input.GetAxisRaw("Horizontal"), upAxis, Input.GetAxisRaw("Vertical"));
            //Vector projection to map inputs to match the orientation of the ship/camera.
            moveVector = transform.right * input.x + transform.forward * input.z + transform.up * input.y;
            moveVector = moveVector * moveSpeed;

            //Rotation
            if (!shouldAlignWithShip)
            {
                camRotation = camRotation * Quaternion.Euler(new Vector3(-camInput.y, -camInput.x, camInput.z) * Time.deltaTime * rotationSpeed);
            }
            else
            {
                //Make rotation match the ship's rotation.
                //camRotation = camRotation * Quaternion.Euler(new Vector3(-camInput.y, -camInput.x, camInput.z) * Time.deltaTime * rotationSpeed);

                /*Quaternion fromToRotation = Quaternion.Inverse(shipController.transform.rotation) * camRotation;

                camRotation = Quaternion.Euler(camRotation.x, camRotation.y, camRotation.z);*/
                //curCamRot += camInput * Time.deltaTime * rotationSpeed;
                //curCamRot.x = WrapAngle(curCamRot.x);
                //curCamRot.y = WrapAngle(curCamRot.y);
                //curCamRot.z = WrapAngle(curCamRot.z);
                //camRotation *= Quaternion.Euler(transform.up * -curCamRot.x/* * Time.deltaTime * rotationSpeed*/);
                
                //Vector3 direction = shipController.transform.position - transform.position;
                //direction = Vector3.ProjectOnPlane(direction, shipController.transform.up);
                //transform.rotation = Quaternion.LookRotation(direction);

                camRotation = shipController.transform.rotation * Quaternion.Euler(shipController.transform.up * -camInput.x * Time.deltaTime * rotationSpeed);
            }

            oxygenText.text = "O<sub>2</sub>:" + oxygen; 
        }

        
    }

    private void FixedUpdate()
    {
        if (!isControllingShip)
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
            {
                rb.MoveRotation(shipController.transform.rotation * camRotation);
            }
        }
        else
        {
            //Make rotation match the ship's rotation.
            camRotation = shipController.transform.rotation;
            rb.MoveRotation(shipController.transform.rotation);
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.relativeVelocity.magnitude);
        if (collision.relativeVelocity.magnitude >= 1)
        {
            oxygen -= collision.relativeVelocity.magnitude * 0.75f;
        }
    }

    public void StartControllingShip()
    {
        isControllingShip = true;
        shipController.enabled = true;
        //deactivate our cam so it switches to the ship cam.
        //In the future make it so that we render from a different cam
        //to a render texture that is displayed on a screen in the ship.
        cam.SetActive(false);
        shipController.UnfreezeShip();
        //Lock the player to the ship.
        shipController.fixedJoint.connectedBody = rb;
    }

    public void StopControllingShip()
    {
        isControllingShip = false;
        shipController.enabled = false;
        //activate our cam so it switches to the player cam.
        //In the future make it so that we render from a different cam
        //to a render texture that is displayed on a screen in the ship.
        cam.SetActive(true);
        shipController.FreezeShip();
        //unlock the player from the ship.
        shipController.fixedJoint.connectedBody = null;
    }

    //https://stackoverflow.com/questions/1628386/normalise-orientation-between-0-and-360
    //Wraps an angle between 0 and 360 degrees.
    public float WrapAngle(float angle)
    {
        return (angle % 360 + 360) % 360;
    }
}
