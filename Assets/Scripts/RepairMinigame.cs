using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RepairMinigame : MonoBehaviour
{
    public Slider slider;

    public Material sliderMaterial;

    public float leftBound = 0.45f;
    public float rightBound = 0.55f;

    public float totalTime = 1f;
    float currentTime = 0f;
    float currentSliderValue = 0f;
    bool doReverse = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float timeIncrement = Time.deltaTime;

        if (!doReverse)
        {
            if (currentSliderValue > 0.99)
            {
                currentSliderValue = 1f;
                doReverse = true;
            }
            else
            {
                currentSliderValue = Mathf.Lerp(0f, 1f, currentTime / totalTime);
            }
        }
        else
        {
            //inverse the time increment so that we are decreasing
            timeIncrement *= -1f;
            if (currentSliderValue < 0.01)
            {
                currentSliderValue = 0f;
                doReverse = false;
            }
            else
            {
                currentSliderValue = Mathf.Lerp(0f, 1f, currentTime / totalTime);
            }
        }


        slider.value = currentSliderValue;
        currentTime += timeIncrement;


        if (Input.GetKeyDown(KeyCode.H) && 0.45f <= currentSliderValue && currentSliderValue <= 0.55f)
        {
            StartCoroutine(ColorDelay());
        }
    }

    public IEnumerator ColorDelay()
    {
        //make the color cyan for 0.5f seconds.
        float time = 0.5f;
        while (time > 0)
        {
            sliderMaterial.SetColor("_LineColor", Color.cyan);
            time -= Time.deltaTime;
            yield return null;
        }
        sliderMaterial.SetColor("_LineColor", Color.red);
    }
}
