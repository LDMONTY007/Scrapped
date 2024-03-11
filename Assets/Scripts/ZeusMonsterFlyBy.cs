using System.Collections;
using UnityEngine;

public class ZeusMonsterFlyBy : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip AudioClip;

    public Transform MonsterStartTransform;
    public Transform MonsterEndTransform;

    public Transform stationTransform;

    public GameObject monster;
    GameObject curMonster;

    public AudioClip metalClang;


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
                didPlayClip = true;
            }
            curMonster.transform.position = (Vector3.Lerp(startPos, end.position, currentTime / totalTime));
            currentTime += Time.deltaTime;
            yield return null;
        }
        Destroy(curMonster);
       // Destroy(gameObject, 0.01f);
    }
}
