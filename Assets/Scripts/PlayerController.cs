using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public GameObject grappleEndPrefab;
    private GameObject grappleEnd;
    public float grappleDistance = 20f;
    Vector3 grapplingPoint = Vector3.zero;

    public GameObject playerModel;

    private LineRenderer grappleLine;

    private bool didGrapple = false;
    private bool startGrapplingHook => Input.GetMouseButtonDown(0);
    private bool doGrapplingHook => Input.GetMouseButton(0);

    public static PlayerController instance;

    //The directions we bind our rotation to. 
    [HideInInspector]
    public Vector3 right;
    [HideInInspector]
    public Vector3 up;
    [HideInInspector]
    public Vector3 fwd;

    public bool hasAtlasBeacon;
    public bool hasApollyonBeacon;
    public bool hasZeusBeacon;
    public bool hasApollyonAstronaut;

    private float _oxygen = 100f;

    private int _scrapCount = 0;
    public int scrapCount { get{ return _scrapCount; } set{ _scrapCount = Mathf.Clamp(value, 0, 99); } }

    bool isOxygenated = false;

    public float oxygenIncrement = 1f;
    public float oxygenHealTimeIncrement = 0.1f;

    public float oxygenLossIncrement = 1f;
    public float oxygenLossTimeIncrement = 0.1f;

    public FixedJoint fixedJoint; 

    public TextGradient promptTextGradient;
    public TextMeshProUGUI promptText;
    public List<Light> lights = new List<Light>();  
    public ShipController shipController;
    public GameObject playerUI;
    public GameObject ShipUI;

    private GameObject currentRepairObj;

    public PauseMenu pauseMenu;

    public float oxygen { get { return _oxygen; } set { _oxygen = Mathf.Clamp(value, 0f, 100f); } }

    Rigidbody rb;
    public GameObject cam;
    public CinemachineBasicMultiChannelPerlin noise;
    public TextMeshProUGUI oxygenText;
    public TextMeshProUGUI scrapText;
    public Slider oxygenSlider;
    Vector3 input;
    Vector3 moveVector;

    public Transform playerControlPos;
    public GameObject RepairPanel;

    Quaternion camRotation;
    Vector3 curCamRot = Vector3.zero;

    public float maxSpeed = 20f;
    public float moveSpeed = 5f;
    public float mouseRotationSpeed = 250f;
    public float rollRotationSpeed = 50f;
    public float sensitivity_WebGL = 25f;
    public float roll_sensitivity_WebGL = 5f;
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
    //Vector3 camInput => new Vector3((Input.GetKey(KeyCode.J) ? 1 : 0) + (Input.GetKey(KeyCode.L) ? -1 : 0), (Input.GetKey(KeyCode.I) ? 1 : 0) + (Input.GetKey(KeyCode.K) ? -1 : 0), (Input.GetKey(KeyCode.Q) ? 1 : 0) + (Input.GetKey(KeyCode.E) ? -1 : 0));
    Vector3 camInput => new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), (Input.GetKey(KeyCode.Q) ? 1 : 0) + (Input.GetKey(KeyCode.E) ? -1 : 0));


    public bool forceAlignWithShip = false;
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
        StartCoroutine(ExecuteAfterFixedUpdateCoroutine());
        instance = this;
        noise = cam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        grappleLine = GetComponent<LineRenderer>();
        grappleLine.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        StopControllingShip();
        rb.rotation = shipController.transform.rotation;
        camRotation = rb.rotation;

        //init this stuff.
        fwd = transform.forward;
        up = transform.up;
        right = transform.right;
    }

    // Update is called once per frame
    void Update()
    {
        if (grappleEnd != null && !doGrapplingHook)
        {
            Destroy(grappleEnd);
            grappleEnd = null;
            didGrapple = false;
            grappleLine.enabled = false;
        }

        

        if (Cursor.lockState == CursorLockMode.Locked && !shouldAlignWithShip && !forceAlignWithShip)
        {
            HandleGrappling();
            HandleCamRotation();
        }
        

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.Pause();
        }

        RaycastHit[] hits = Physics.RaycastAll(cam.transform.position, cam.transform.forward, 10f, playerMask);

        bool didHitConsole = false;
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("ShipControls"))
            {
                didHitConsole = true; break;
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && didHitConsole || Input.GetKeyDown(KeyCode.R) && isControllingShip)
        {
            Debug.Log("HERE");
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
                    //camRotation = camRotation * Quaternion.Euler(new Vector3(-camInput.x * mouseRotationSpeed, camInput.y * mouseRotationSpeed, camInput.z * rollRotationSpeed) * Time.deltaTime);
                    //camRotation = camRotation * Quaternion.Euler(new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * Time.deltaTime * rotationSpeed);
                    //Switch to using ship relative locomotion.
                    up = transform.up;
                    fwd = transform.forward;
                    right = transform.right;
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

                    //camRotation = shipController.transform.rotation * Quaternion.Euler(shipController.transform.up * -camInput.x * Time.deltaTime * mouseRotationSpeed/*rotationSpeed*/);
                    //Switch to using ship relative locomotion.
                    up = shipController.transform.up;
                    fwd = shipController.transform.forward;
                    right = shipController.transform.right;
                }


                #region Check to open repair minigame

                if (Input.GetKeyDown(KeyCode.R) && Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hitInfo, 5f, playerMask))
                {
                    if (hitInfo.collider.CompareTag("Repairable"))
                    {
                        if (scrapCount > 0)
                        {
                            StartRepairing(hitInfo.collider.gameObject);
                        }
                        else
                        {
                            promptText.gameObject.SetActive(true);
                            promptTextGradient.StartAnimatingGradient();
                            promptText.text = "Out of\nScrap";
                        }
                    }
                }

                HandleInteraction();

                #endregion
            }



            oxygenText.text = string.Format("O<sub>2</sub>:" + (oxygen).ToString("F0"));
            scrapText.text = "Scrap:" + scrapCount;
            oxygenSlider.value = oxygen;
        }

    }

    public void HandleGrappling()
    {
        
        if (startGrapplingHook)
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hitInfo, grappleDistance, playerMask))
            {
                grapplingPoint = hitInfo.point;
                if (doGrapplingHook)
                {
                    if (grappleEnd != null)
                    {
                        Destroy(grappleEnd);
                        grappleEnd = null;
                    }
                    grappleEnd = Instantiate(grappleEndPrefab, grapplingPoint, Quaternion.LookRotation(hitInfo.normal));
                    grappleLine.SetPosition(0, transform.position);
                    grappleLine.SetPosition(1, grappleEnd.transform.position);
                    grappleLine.enabled = true;
                }
                
                didGrapple = true;
            }
        }

        if (doGrapplingHook && didGrapple)
        {
            grappleLine.SetPosition(0, transform.position);
            rb.AddForce((grapplingPoint - transform.position).normalized * 5f);
        }
    }

    public void HandleCamRotation()
    {

        if (!isControllingShip)
        {
            //This is the fix for WebGL having different mouse sensitivity.
            float x = Input.GetAxisRaw("Mouse X");
            float y = Input.GetAxisRaw("Mouse Y");
            float z = camInput.z;
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                x *= sensitivity_WebGL;
                y *= sensitivity_WebGL;
                z *= roll_sensitivity_WebGL;
            }
            else
            {
                x *= mouseRotationSpeed; // your custom sensitivity value...
                y *= mouseRotationSpeed; // ...which may be user-adjustable
                z *= rollRotationSpeed;
            }


            //Combine all the rotations around their respective axes relative to the current directions
            //which are either the player's Right, Up, and Fwd or the ship's Right, Up, and Fwd.
            Quaternion newRotation = Quaternion.AngleAxis(x * Time.deltaTime, up) * Quaternion.AngleAxis(-y * Time.deltaTime, right) * Quaternion.AngleAxis(z * Time.deltaTime, fwd);
            /*Quaternion newRotation = Quaternion.AngleAxis(camInput.y, transform.up) * Quaternion.AngleAxis(-camInput.x, transform.right) * Quaternion.AngleAxis(camInput.z, transform.forward);*/

            if (shouldAlignWithShip)
                newRotation = Quaternion.AngleAxis(camInput.y, transform.up);
            //camRotation *= newRotation;
            if (shouldAlignWithShip)
            {
                //SUPER CLOSE TO WORKING
                //The rotation required to get us from the 
                //Current rotation to looking at the ship. 
                //Quaternion wishRot = transform.rotation * newRotation;
                //This is the rotation we'd need to add to our current rotation to be at 
                //the ship's rotation. 
                Quaternion diff = shipController.transform.rotation * Quaternion.Inverse(transform.rotation);
                //The next line is the closest I can get to solving binding it to the proper axes.
                //Quaternion diff = (shipController.transform.rotation * newRotation) * Quaternion.Inverse(transform.rotation);
                //Quaternion diff = shipController.transform.rotation * Quaternion.Inverse(transform.rotation * Quaternion.Inverse(camRotation));
                //Quaternion diff = Quaternion.Inverse(transform.rotation * camRotation) * shipController.transform.rotation;
                //Quaternion diff2 = transform.rotation * Quaternion.Inverse(shipController.transform.rotation);

                /*                    Quaternion zRot = Quaternion.AngleAxis(camInput.z, fwd);
                                    newRotation = Quaternion.AngleAxis(camInput.y, up) * Quaternion.AngleAxis(-camInput.x, right);
                                    Quaternion tempRot = newRotation * rb.rotation;*/
                /*newRotation = Quaternion.AngleAxis(camInput.y, up) * Quaternion.AngleAxis(-camInput.x, right) * Quaternion.AngleAxis(Quaternion.Angle(shipController.transform.rotation., ))*/
                newRotation = diff/* * wishRot*/;


            }
            //NEW ROTATION MUST BE FIRST BECAUSE QUATERNION MULTIPLICATION IS NOT COMMUNICATIVE.
            camRotation = newRotation * rb.rotation;
        }
        else
        {
            camRotation = shipController.transform.rotation;
        }
    }

    public void TestCameraShake()
    {
        DoCameraShake(0.5f, 0.5f, 0.5f);
    }
    public void DoCameraShake(float time, float amplitude, float frequency)
    {
        StartCoroutine(CameraShakeCoroutine(time, amplitude, frequency));
    }


    public IEnumerator CameraShakeCoroutine(float time, float amplitude, float frequency)
    {
        float currentTime = 0f;
        float oldAmp = noise.m_AmplitudeGain;
        float oldFrequency = noise.m_FrequencyGain;

        noise.m_AmplitudeGain = amplitude;
        noise.m_FrequencyGain = frequency;
        while (currentTime < time) 
        { 
            currentTime += Time.deltaTime;
            yield return null;
        }
        noise.m_AmplitudeGain = oldAmp;
        noise.m_FrequencyGain = oldFrequency;

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
            //Switch to using player relative locomotion.
            up = transform.up;
            fwd = transform.forward;
            right = transform.right;

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

                /*if (transform.rotation != camRotation)
                {
                    rb.MoveRotation(shipController.transform.rotation * camRotation);
                }*/
            }
            else
            {
                rb.linearVelocity = Vector3.zero;
            }
        }
        else
        {
            //Make rotation match the ship's rotation.
            //camRotation = shipController.transform.rotation;
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
            if (!isControllingShip)
            {
                foreach (Light l in lights)
                {
                    l.enabled = false;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Oxygenated"))
        {
            isOxygenated = false;
            //Start losing oxygen.
            StartCoroutine(OxygenLossCoroutine());
            if (!isControllingShip)
            {
                foreach (Light l in lights)
                {
                    l.enabled = true;
                }
            }
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
        //make player invis;
        playerModel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //Make the player's position be
        //at the control console.
        //rb.MovePosition(shipController.transform.TransformPoint(playerControlPos.position));
        //rb.position = shipController.transform.TransformPoint(playerControlPos.localPosition);
        rb.rotation = shipController.transform.rotation;

        isControllingShip = true;
        shipController.enabled = true;
        //deactivate our cam so it switches to the ship cam.
        cam.SetActive(false);
        shipController.UnfreezeShip();
        //Lock the player to the ship.
        //shipController.fixedJoint.anchor = playerControlPos.localPosition;
        //shipController.fixedJoint.connectedAnchor = playerControlPos.localPosition;
        transform.position = shipController.transform.TransformPoint(playerControlPos.localPosition);
        fixedJoint = gameObject.AddComponent<FixedJoint>();
        fixedJoint.connectedBody = shipController.rb;
        //fixedJoint.connectedAnchor = Vector3.zero;
        //fixedJoint.anchor = playerControlPos.localPosition;
        ShipUI.SetActive(true);
        playerUI.SetActive(false);
        if (!isOxygenated)
        {
            foreach (Light l in lights)
            {
                l.enabled = false;
            }
        }
        
    }

    public void StopControllingShip()
    {
        //make player visible;
        playerModel.SetActive(true);
        rb.linearVelocity = Vector3.zero;
        rb.position = playerControlPos.position;
        rb.rotation = shipController.transform.rotation;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isControllingShip = false;
        shipController.enabled = false;
        cam.SetActive(true);
        shipController.FreezeShip();
        //unlock the player from the ship.
        Destroy(fixedJoint);
        fixedJoint = null;
        ShipUI.SetActive(false);
        playerUI.SetActive(true);


        if (!isOxygenated)
        {
            foreach (Light l in lights)
            {
                l.enabled = true;
            }
        }
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

    public void StopRepairingWin()
    {

        //Destroy the current repair object, effectively removing it from the list of 
        //repairables in the shipController. Thus "healing" it.
        Debug.Log("REPAIR WIN");
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

    public void StopRepairingLose()
    {

        //Destroy the current repair object, effectively removing it from the list of 
        //repairables in the shipController. Thus "healing" it.

        //Destroy the parent obj.
        //first remove it from the shipController list.
        //shipController.damageDecals.Remove(currentRepairObj.transform.parent.gameObject);
        //Then destroy the parent obj of the damage decal.
        //Destroy(currentRepairObj.transform.parent.gameObject); currentRepairObj = null;
        currentRepairObj = null;
        //disable repair panel so the minigame ends.
        RepairPanel.SetActive(false);
        //set isRepairing to false.
        isRepairing = false;
    }

    //This is where I do rotation
    IEnumerator ExecuteAfterFixedUpdateCoroutine()
    {
        YieldInstruction waitForFixedUpdate = new WaitForFixedUpdate();
        while (true)
        {
            yield return waitForFixedUpdate;

            if (!isControllingShip)
            {
/*
                //Combine all the rotations around their respective axes relative to the current directions
                //which are either the player's Right, Up, and Fwd or the ship's Right, Up, and Fwd.
                Quaternion newRotation = Quaternion.AngleAxis(camInput.y, up) * Quaternion.AngleAxis(-camInput.x, right) * Quaternion.AngleAxis(camInput.z, fwd);
                *//*Quaternion newRotation = Quaternion.AngleAxis(camInput.y, transform.up) * Quaternion.AngleAxis(-camInput.x, transform.right) * Quaternion.AngleAxis(camInput.z, transform.forward);*//*

                if (shouldAlignWithShip)
                    newRotation = Quaternion.AngleAxis(camInput.y, transform.up);
                camRotation *= newRotation;
                if (shouldAlignWithShip)
                {
                    //SUPER CLOSE TO WORKING
                    //The rotation required to get us from the 
                    //Current rotation to looking at the ship. 
                    //Quaternion wishRot = transform.rotation * newRotation;
                    //This is the rotation we'd need to add to our current rotation to be at 
                    //the ship's rotation. 
                    Quaternion diff = shipController.transform.rotation * Quaternion.Inverse(transform.rotation);
                    //The next line is the closest I can get to solving binding it to the proper axes.
                    //Quaternion diff = (shipController.transform.rotation * newRotation) * Quaternion.Inverse(transform.rotation);
                    //Quaternion diff = shipController.transform.rotation * Quaternion.Inverse(transform.rotation * Quaternion.Inverse(camRotation));
                    //Quaternion diff = Quaternion.Inverse(transform.rotation * camRotation) * shipController.transform.rotation;
                    //Quaternion diff2 = transform.rotation * Quaternion.Inverse(shipController.transform.rotation);

                    *//*                    Quaternion zRot = Quaternion.AngleAxis(camInput.z, fwd);
                                        newRotation = Quaternion.AngleAxis(camInput.y, up) * Quaternion.AngleAxis(-camInput.x, right);
                                        Quaternion tempRot = newRotation * rb.rotation;*/
                    /*newRotation = Quaternion.AngleAxis(camInput.y, up) * Quaternion.AngleAxis(-camInput.x, right) * Quaternion.AngleAxis(Quaternion.Angle(shipController.transform.rotation., ))*//*
                    newRotation = diff*//* * wishRot*//*;


                }*/
                //NEW ROTATION MUST BE FIRST BECAUSE QUATERNION MULTIPLICATION IS NOT COMMUNICATIVE.
                rb.MoveRotation(camRotation);
            }
            
            


        }
    }


    // define this function:
    public static float DampenedMovement(float value)
    {

        if (Mathf.Abs(value) > 1f)
        {
            // best value for dampenMouse is 0.5 but better make it user-adjustable
            return Mathf.Lerp(value, Mathf.Sign(value), /*dampenMouse*/0.5f);
        }
        return value;
    }

}
