using System.Collections;
using UnityEngine;
using UnityEngine.Events;



//I HAVE YET OT ACTUALLY CODE THE PDA.
//I NEED TO DO SO BUT LATER.
public class Astronaut : MonoBehaviour, IInteractible
{
    public Gradient promptGradient = new Gradient();

    public AstronautType astronautType;

    public AudioClip savedClip;

    public AudioSource source;

    public enum AstronautType
    {
        Zeus,
        Apollyon,
        Atlas
    }

    public UnityEvent OnInteractEvent;

    public bool doRandomRotation = true;

    

    //public string fileName = null;

    //This will be unused for now.
    public void OnFocus(PlayerController p)
    {
        //Debug.Log("On Focus!");
    }

    public void OnInteract(PlayerController p)
    {
        Debug.Log("On Interact!");
        source.PlayOneShot(savedClip);
        PlayerController.instance.PlayPromptText(3f, "Astronaut Saved", promptGradient);
        switch (astronautType)
        {
            case AstronautType.Zeus:
                p.hasApollyonAstronaut = true;
                break;
            case AstronautType.Apollyon:
                p.hasApollyonAstronaut = true;
                break;
            case AstronautType.Atlas:
                p.hasAtlas = true;
                break;
        }
        StartCoroutine(PickupCoroutine(1f, p.transform));   
    }


    //This will also be unused for now.
    public void OnLostFocus(PlayerController p)
    {
        //Debug.Log("Lost Focus!");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //source = GetComponent<AudioSource>();
        if (doRandomRotation)
        {
            //randomize rotation so it actually looks good.
            transform.rotation = Random.rotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator PickupCoroutine(float totalTime, Transform playerTransform)
    {
        float currentTime = 0f;
        Vector3 startPos = transform.position;
        while (currentTime < totalTime)
        {
            if (currentTime >= totalTime - 0.01f)
            {
                currentTime = totalTime;
            }
            transform.position = (Vector3.Lerp(startPos, playerTransform.position, currentTime / totalTime));
            currentTime += Time.deltaTime;
            yield return null;
        }
        OnInteractEvent.Invoke();
        PlayerController.instance.hasApollyonAstronaut = true;

        //Let the audio play out before we destroy the game object.
        //also make it invisible so it isn't just sitting there.
        while (source.isPlaying)
        {
            foreach(Renderer r in source.GetComponentsInChildren<Renderer>())
            {
                r.enabled = false;
            }
            yield return null;
        }
        Destroy(gameObject, 0.01f);
    }
}
