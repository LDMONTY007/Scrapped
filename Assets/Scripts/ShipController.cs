using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ShipController : MonoBehaviour
{
    private float _health = 100f;

    public float health { get { return _health; } set { _health = Mathf.Clamp(value, 0f, 100f); } }

    [HideInInspector]
    public FixedJoint fixedJoint;
    [HideInInspector]
    public Rigidbody rb;
    public Camera cam;
    Vector3 input;
    Vector3 moveVector;

    public TextMeshProUGUI hullIntegrityText;
    public TextMeshProUGUI speedText;
    public GameObject damagePrefab;



    Quaternion camRotation;

    [HideInInspector]
    public List<GameObject> damageDecals = new List<GameObject>();

    public bool isPlayerControlled;
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


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        fixedJoint = GetComponent<FixedJoint>();
    }

    // Start is called before the first frame update
    void Start()
    {
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

        //if you get 10 damages you die.
        health = 100f - (damageDecals.Count * 10);
        hullIntegrityText.text = "Hull Integrity: " + health.ToString("F0");

        speedText.text = "Speed:" + (rb.linearVelocity.magnitude).ToString("F2") + "M/s";
    }

    private void FixedUpdate()
    {
        if (shouldBrake)
        {
            //Apply opposite force to stop the ship.
            //Velocity * mass = force.
            rb.AddForce(-rb.linearVelocity * rb.mass);
            if (rb.linearVelocity.magnitude < 0.1)
            {
                rb.linearVelocity = Vector3.zero;
            }
        }
        else
        {
            rb.AddForce(moveVector);
        }

        //Clamp to max speed.
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed);

        if (transform.rotation != camRotation)
            rb.MoveRotation(camRotation);
    }



    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.relativeVelocity.magnitude);

        //TODO:
        //get hit normal of the collision and spawn
        //the "Damage" decal on that position using Quaternion.lookRotation to look at the direction of the normal.
        //Also Make sure to keep a list of these objects.
        if (!collision.collider.CompareTag("Player") && !collision.collider.CompareTag("NoDamage") && collision.relativeVelocity.magnitude >= 1)
        {
            //check that we don't stack a repairable on top of another repairable.
            foreach (ContactPoint c in collision.contacts)
            {
                if (c.thisCollider.CompareTag("Repairable"))
                {
                    //Do not instantiate a repairable if the colliding object
                    //is a repairable.
                    return;
                }
            }
            GameObject go = Instantiate(damagePrefab, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal), transform);
            go.transform.localScale = new Vector3(go.transform.localScale.x / transform.localScale.x, go.transform.localScale.y / transform.localScale.y, go.transform.localScale.z / transform.localScale.z);

            damageDecals.Add(go);
        }
    }

    public void FreezeShip()
    {
        rb.linearVelocity = Vector3.one;
        rb.isKinematic = true;
    }

    public void UnfreezeShip()
    {
        rb.isKinematic = false;
    }
}
