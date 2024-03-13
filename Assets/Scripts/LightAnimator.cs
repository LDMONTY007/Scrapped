using System.Collections;
using UnityEngine;

public class LightAnimator : MonoBehaviour 
{
    public Light curLight = null;
    public float startValue;
    public float endValue;
    public float totalTime = 1f;

    public AudioSource source;
    public AudioClip clip;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        StartCoroutine(AnimateLightCoroutine());
    }
    
    public IEnumerator AnimateLightCoroutine()
    {
        float currentTime = 0f;
        bool doBackwards = false;
        while (true)
        {
            if (!doBackwards && currentTime >= totalTime - 0.01f)
            {
                source.PlayOneShot(clip);
                doBackwards = true;
                currentTime = totalTime;
            }
            if (doBackwards && currentTime <= 0f)
            {
                doBackwards = false;
                currentTime = 0f;
            }
            currentTime += doBackwards ? -Time.deltaTime : Time.deltaTime;
            curLight.intensity = Mathf.Lerp(startValue, endValue, currentTime / totalTime);
            yield return null;
        }
    }

}
