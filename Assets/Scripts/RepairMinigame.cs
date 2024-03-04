using UnityEngine;
using UnityEngine.UI;

public class RepairMinigame : MonoBehaviour
{
    public Slider slider;

    float totalTime = 1f;
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
            if (currentTime > 0.99)
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
            if (currentTime < 0.01)
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
    }
}
