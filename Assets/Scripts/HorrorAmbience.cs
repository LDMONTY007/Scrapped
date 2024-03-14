using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorrorAmbience : MonoBehaviour
{
    public float chanceToPlayAudio;
    public float audioChanceFactor;

    public AnimationCurve falloffCurve;

    Transform sourceTransform;
    AudioSource source;

    public List<AudioClip> clipList = new List<AudioClip>();

    public float sphereAmbienceRadius = 30f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(tryRandomAmbience());
    }

    // Update is called once per frame
    void Update()
    {
        chanceToPlayAudio += Time.deltaTime;


    }

    public IEnumerator tryRandomAmbience()
    {
        while (true)
        {
            if (Random.Range(0f, 1f) * falloffCurve.Evaluate(chanceToPlayAudio) > 0.9f)
            {
                if (sourceTransform == null)
                {
                    GameObject go = new GameObject();
                    sourceTransform = go.transform;
                    go.AddComponent<AudioSource>();
                    AudioSource source = go.AddComponent<AudioSource>();
                }
                if (sourceTransform && source)
                {
                    sourceTransform.position = PlayerController.instance.transform.position + Random.onUnitSphere * sphereAmbienceRadius;
                    source.PlayOneShot(clipList[Random.Range(0, clipList.Count)]);
                }
            }
            yield return new WaitForSeconds(Random.Range(15f, 30f));
        }
    }
}
