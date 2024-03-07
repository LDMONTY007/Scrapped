using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private float _oxygen = 100f;

    private int _scrapCount = 0;
    public int scrapCount { get{ return _scrapCount; } set{ _scrapCount = Mathf.Clamp(value, 0, 99); } }

    bool isOxygenated = false;

    public float oxygenIncrement = 1f;
    public float oxygenHealTimeIncrement = 0.1f;

    public float oxygenLossIncrement = 1f;
    public float oxygenLossTimeIncrement = 0.1f;

    public ShipController shipController;
    public GameObject playerUI;
    public GameObject ShipUI;

    private GameObject currentRepairObj;

    public float oxygen { get { return _oxygen; } set { _oxygen = Mathf.Clamp(value, 0f, 100f); } }

    Rigidbody rb;
    public GameObject cam;
    public TextMeshProUGUI oxygenText;
    public Slider oxygenSlider;
    Vector3 input;
    Vector3 moveVector;

    public GameObject RepairPanel;

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

    public bool isRepairing = false;

    //Get a mask that includes every layer other than the player layer.
    int playerMask;

    //the interactible that was last looked at,
    //used to tell the interactible when we've stopped looking at it.
    private IInteractible currentInteractible;

    [Header("Interactible Parameters")]
    public float interactionDist = 5f;
    public float focusDist = 20f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerMask = ~LayerMask.GetMask("Player");
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

            if (!isRepairing)
            {
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


                #region Check to open repair minigame

                if (Input.GetKeyDown(KeyCode.R) && Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hitInfo, 5f, playerMask))
                {
                    if (hitInfo.collider.CompareTag("Repairable"))
                    {
                        StartRepairing(hitInfo.collider.gameObject);
                    }
                }

                HandleInteraction();

                #endregion
            }



            oxygenText.text = string.Format("O<sub>2</sub>:" + (oxygen).ToString("F0")); 
            oxygenSlider.value = oxygen;
        }

    }

    private void HandleInteraction()
    {
        //int mask = LayerMask.GetMask("Interactible");

        if (Physics.SphereCast(cam.transform.position, GetComponent<Collider>().bounds.size.y / 4f,cam.transform.forward, out RaycastHit hitInfo, focusDist, playerMask))
        {
            IInteractible interactible = hitInfo.collider.GetComponent<IInteractible>();
            if (Input.GetKeyDown(KeyCode.R) && hitInfo.distance <= interactionDist && interactible != null)
            {
                //tell the interactible we are interacting with it.
                interactible.OnInteract(this);
            }
        }
    }


    //https://forum.unity.com/threads/check-if-gameobject-in-visible-on-screen.424586/
    public bool IsOnScreen(GameObject g)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        bool onScreen = screenPos.x > 0f && screenPos.x < Screen.width && screenPos.y > 0f && screenPos.y < Screen.height;

        if (onScreen && g.GetComponent<Renderer>().isVisible)
        {
            //Visible
            return true;
        }
        else
        {
            //NotVisible
            return false;
        }
    }

    private void FixedUpdate()
    {
        if (!isControllingShip)
        {
            if (!isRepairing)
            {
                if (shouldBrake)
                {
                    //Apply opposite force to stop the ship.
                    //Velocity * mass = force.
                    rb.AddForce(-rb.linearVelocity * rb.mass);
                    if (rb.linearVelocity.magnitude < 0.5)
                    {
                        rb.linearVelocity = Vector3.zero;
                    }
                }
                else
                {
                    //rb.linearVelocity = moveVector;
                    rb.AddForce(moveVector);
                }

                //Clamp to max speed.
                rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed);

                if (transform.rotation != camRotation)
                {
                    rb.MoveRotation(shipController.transform.rotation * camRotation);
                }
            }
            else
            {
                rb.linearVelocity = Vector3.zero;
            }
        }
        else
        {
            //Make rotation match the ship's rotation.
            camRotation = shipController.transform.rotation;
            rb.MoveRotation(shipController.transform.rotation);
        }
        
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.relativeVelocity.magnitude);
        if (collision.relativeVelocity.magnitude >= 1)
        {
            oxygen -= collision.relativeVelocity.magnitude * 0.75f;
        }
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Oxygenated"))
        {
            isOxygenated = true;
            //Start regaining oxygen
            StartCoroutine(OxygenateCoroutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Oxygenated"))
        {
            isOxygenated = false;
            //Start losing oxygen.
            StartCoroutine(OxygenLossCoroutine());
        }
    }

    public IEnumerator OxygenateCoroutine()
    {
        while (isOxygenated)
        {
            oxygen += oxygenIncrement;
            yield return new WaitForSeconds(oxygenHealTimeIncrement);
        }
    }

    public IEnumerator OxygenLossCoroutine()
    {
        while (!isOxygenated)
        {
            oxygen -= oxygenLossIncrement;
            yield return new WaitForSeconds(oxygenLossTimeIncrement);
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
        ShipUI.SetActive(true);
        playerUI.SetActive(false);
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
        ShipUI.SetActive(false);
        playerUI.SetActive(true);
    }

    //https://stackoverflow.com/questions/1628386/normalise-orientation-between-0-and-360
    //Wraps an angle between 0 and 360 degrees.
    public float WrapAngle(float angle)
    {
        return (angle % 360 + 360) % 360;
    }

    public void StartRepairing(GameObject repairObj)
    {
        if (currentRepairObj != null)
        {
            Debug.LogWarning("Tried to repair object while repairing another object!");
            return;
        }
        currentRepairObj = repairObj;
        RepairPanel.SetActive(true);
        isRepairing = true;
    }

    public void StopRepairing()
    {
        //Destroy the current repair object, effectively removing it from the list of 
        //repairables in the shipController. Thus "healing" it.

        //Destroy the parent obj.
        //first remove it from the shipController list.
        shipController.damageDecals.Remove(currentRepairObj.transform.parent.gameObject);
        //Then destroy the parent obj of the damage decal.
        Destroy(currentRepairObj.transform.parent.gameObject); currentRepairObj = null;
        //disable repair panel so the minigame ends.
        RepairPanel.SetActive(false);
        //set isRepairing to false.
        isRepairing = false;
    }
}
