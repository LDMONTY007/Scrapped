using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ZeusMonsterFlyBy : MonoBehaviour
{

    public UnityEvent OnMonsterBumpEvent;

    AudioSource audioSource;
    public AudioClip AudioClip;

    public Transform MonsterStartTransform;
    public Transform MonsterEndTransform;

    public Transform stationTransform;

    public GameObject monster;
    GameObject curMonster;

    public AudioClip metalClang;
    public AudioClip metalClang2;
    public AudioClip metalClang3;

    public AudioClip lostAntennaAudio;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DoMonsterFlyBy()
    {

        //Start coroutine for the monster lerping from one position to the next.
        StartCoroutine(delayCoroutine(1f, lerpCoroutine(1f, MonsterStartTransform, MonsterEndTransform)));
        //StartCoroutine(lerpCoroutine(1f, MonsterStartTransform, MonsterEndTransform));

        //Play some random metal sounds

        //Play some cinemachine camera shake

        
    }

    public IEnumerator delayCoroutine(float delay, IEnumerator coroutine)
    { 
        while (delay > 0f)
        {
            delay -= Time.deltaTime;
            yield return null;
        }
        StartCoroutine(coroutine);
    }



    public IEnumerator lerpCoroutine(float totalTime, Transform start, Transform end)
    {
        float currentTime = 0f;
        Vector3 startPos = start.position;
        bool didPlayClip = false;
        if (curMonster == null )
        {
            //make it and rotate it so that it is parellel with the rotation of the station.
            curMonster = GameObject.Instantiate(monster, startPos, stationTransform.transform.rotation);
        }
        while (currentTime < totalTime)
        {
            if (currentTime >= totalTime - 0.01f)
            {
                currentTime = totalTime;
            }
            if (!didPlayClip && currentTime <= totalTime / 2)
            {
                //Play the monster clip
                audioSource.PlayOneShot(metalClang);
                OnMonsterBumpEvent.Invoke();
                //PlayerController.instance.DoCameraShake(0.1f, 0.5f, 0.5f);
                didPlayClip = true;
            }
            curMonster.transform.position = (Vector3.Lerp(startPos, end.position, currentTime / totalTime));
            currentTime += Time.deltaTime;
            yield return null;
        }
        Destroy(curMonster);


        float currentTime1 = 0f;
        float totalTime1 = 1f;
        while (currentTime1 < totalTime1)
        {
            currentTime1 += Time.deltaTime;
            yield return null;
        }

        audioSource.PlayOneShot(metalClang2);
        OnMonsterBumpEvent.Invoke();
        float currentTime2 = 0f;
        float totalTime2 = 1f;
        while (currentTime2 < totalTime2)
        {
            currentTime2 += Time.deltaTime;
            yield return null;
        }

        audioSource.PlayOneShot(lostAntennaAudio);
        float currentTime3 = 0f;
        float totalTime3 = 7.56f;
        while (currentTime3 < totalTime3) 
        {
            currentTime3 += Time.deltaTime;
            yield return null;
        }
        audioSource.PlayOneShot(metalClang3);
        OnMonsterBumpEvent.Invoke();
        //Delete the antenna from the top of the ship.
        Destroy(ShipController.instance.antenna);
        // Destroy(gameObject, 0.01f);
    }


    
}
