using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AnglerAttack : MonoBehaviour
{

    public Transform MonsterStartTransform;
    public Transform MonsterEndTransform;

    public Transform stationTransform;

    public GameObject monster;
    GameObject curMonster;


    public Transform CamTransform;

    public AudioClip monsterApproachSound;
    AudioSource source;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CamTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        DoAnglerAttack();
    }

    public void DoAnglerAttack()
    {

        //Start coroutine for the monster lerping from one position to the next.
        StartCoroutine(lerpCoroutine(1.5f, MonsterStartTransform, MonsterEndTransform));
        //StartCoroutine(lerpCoroutine(1f, MonsterStartTransform, MonsterEndTransform));

        //Play some random metal sounds

        //Play some cinemachine camera shake


    }

    public IEnumerator lerpCoroutine(float totalTime, Transform start, Transform end)
    {
        float currentTime = 0f;
        Vector3 startPos = CamTransform.position + (CamTransform.forward.normalized * 30f);
        bool didPlayClip = false;
        if (curMonster == null)
        {
            //make it and rotate it so that it is parellel with the rotation of the station.
            curMonster = GameObject.Instantiate(monster, startPos, stationTransform.transform.rotation);
            source = curMonster.GetComponent<AudioSource>();
        }

        if (source != null)
        {
            source.PlayOneShot(monsterApproachSound);
        }

        while (currentTime < totalTime)
        {
            startPos = CamTransform.position + (CamTransform.forward.normalized * 30f);
            if (curMonster != null) { 
                curMonster.transform.rotation = Quaternion.LookRotation(CamTransform.position - curMonster.transform.position);
                if (currentTime >= totalTime - 0.01f)
                {
                    currentTime = totalTime;
                }
                if (!didPlayClip && currentTime <= totalTime / 2)
                {
                    //Play the monster clip
                    //audioSource.PlayOneShot(metalClang);
                    //OnMonsterBumpEvent.Invoke();
                    //PlayerController.instance.DoCameraShake(0.1f, 0.5f, 0.5f);
                    didPlayClip = true;
                }
                curMonster.transform.position = (Vector3.Lerp(startPos, CamTransform.position /*+ (curMonster.transform.up.normalized * 2f) + (PlayerController.instance.transform.forward.normalized * 5f)*/, currentTime / totalTime));
            }
            currentTime += Time.deltaTime;
            yield return null;
        }
        /*Destroy(curMonster);
        curMonster = null;*/



        //For now end the game on the angler attack.
        GameManager.instance.EndGame();


/*        float currentTime1 = 0f;
        float totalTime1 = 1f;
        while (currentTime1 < totalTime1)
        {
            currentTime1 += Time.deltaTime;
            yield return null;
        }

        //audioSource.PlayOneShot(metalClang2);
        //OnMonsterBumpEvent.Invoke();
        float currentTime2 = 0f;
        float totalTime2 = 1f;
        while (currentTime2 < totalTime2)
        {
            currentTime2 += Time.deltaTime;
            yield return null;
        }

        //audioSource.PlayOneShot(lostAntennaAudio);
        float currentTime3 = 0f;
        float totalTime3 = 7.56f;
        while (currentTime3 < totalTime3)
        {
            currentTime3 += Time.deltaTime;
            yield return null;
        }
        //audioSource.PlayOneShot(metalClang3);
        //OnMonsterBumpEvent.Invoke();
        //Delete the antenna from the top of the ship.
        Destroy(ShipController.instance.antenna);
        // Destroy(gameObject, 0.01f);*/
    }
}
