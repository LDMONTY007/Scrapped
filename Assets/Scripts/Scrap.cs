using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Scrap : MonoBehaviour, IInteractible
{
    public UnityEvent OnInteractEvent;

    public bool doRandomRotation = true;

    //This will be unused for now.
    public void OnFocus(PlayerController p)
    {
        //Debug.Log("On Focus!");
    }

    public void OnInteract(PlayerController p)
    {
        Debug.Log("On Interact!");
        p.scrapCount += 3;
        OnInteractEvent.Invoke();
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
        Destroy(gameObject, 0.01f);
    }
}
