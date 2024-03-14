using System.Collections;
using UnityEngine;

public class ApollyonEchoes : MonoBehaviour
{
    public Light light;
    public Rigidbody rb;

    public bool shouldDoLightFollow = false;

    PlayerController playerController;
    Transform playerTransform;

    public float moveSpeed = 5f;

    public float maxSpeed = 10f;


    public AudioClip glassBang = null;
    public AudioClip rumble = null;

    public AudioSource audioSource = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerController = PlayerController.instance;
        playerTransform = playerController.transform;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartLightFollow()
    {
        StartCoroutine(LightFollowCoroutine());
    }

    public bool didDie = false;

    public IEnumerator LightFollowCoroutine()
    {
        float currentTime = 0f;
        float totalTime = 10f;
        light.enabled = true;
        while (currentTime < totalTime)
        {
            
/*            if ((int)currentTime % 3 == 0)
            {
                audioSource.PlayOneShot(glassBang);
            }*/
            audioSource.PlayOneShot(glassBang);

            if (currentTime > 2f * totalTime / 3f && !didDie)
            {
                //go oposite of player.
                shouldDoLightFollow = false;
                rb.linearVelocity = Vector3.zero;
                yield return new WaitForFixedUpdate();
                rb.AddForce((transform.position - playerTransform.position).normalized * moveSpeed, ForceMode.Impulse);
                didDie = true;
            }
            else
            {
                shouldDoLightFollow = true;
            }
            currentTime += Time.deltaTime;
            yield return null;
        }
        shouldDoLightFollow = false;
        light.enabled = false;
    }

/*    public IEnumerator fadeLight(float time, float start, float end)
    {

    }*/

    private void FixedUpdate()
    {
        if (shouldDoLightFollow)
        {
            //rb.AddForce((transform.position - (playerTransform.position + playerTransform.forward * 20f)) * moveSpeed);

            Vector3.Angle(transform.position, playerTransform.position + Camera.main.transform.forward * 10f);
            //rb.AddForce(((playerTransform.position + Camera.main.transform.forward * 10f) - transform.position).normalized * moveSpeed);
            rb.AddForce((playerTransform.position - transform.position).normalized * moveSpeed);
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed);
            }
        }
    }



}
