using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip introClip;
    public AudioClip testingClip;

    public UnityEvent onStartTestingEvent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(playClipsBackToBack());
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public IEnumerator playClipsBackToBack()
    {
        audioSource.PlayOneShot(introClip);
        while (audioSource.isPlaying)
        {
            yield return null;
        }
        onStartTestingEvent.Invoke();
        audioSource.PlayOneShot(testingClip);
        while (audioSource.isPlaying)
        {
            yield return null;
        }
        SceneManager.LoadScene("BaseScene");

    }

   
}
