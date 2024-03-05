using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RepairMinigame : MonoBehaviour
{
    public Slider slider;

    public RectTransform hitAreaTransform;
    public RectTransform handleTransform;
    public Material sliderMaterial;

    public int min = 0;
    public int max = 500;

    public float totalTime = 1f;
    float currentTime = 0f;
    float currentSliderValue = 0f;
    bool doReverse = false;

    private float prevPos = 0;

    public int repairCount = 0;

    public PlayerController playerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
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


        if (Input.GetKeyDown(KeyCode.R) && RectTransformExtensions.Overlaps(handleTransform, hitAreaTransform))
        {
            repairCount++;
            StartCoroutine(ColorDelay());
        }

        /*if (Input.GetKeyDown(KeyCode.H) && 0.45f <= currentSliderValue && currentSliderValue <= 0.55f)
        {
            StartCoroutine(ColorDelay());
        }*/
    }

    public void SetRandomHitPos()
    {
        //TODO:
        //Do a random.range from the min + hitAreaWidth / 2, and max - hitAreaWidth / 2.
        //Then set the pos of the hitArea to be that random X position.
        //anchored position is the actual position relative to the anchor.
        float currentPos = Random.Range(min + hitAreaTransform.rect.width / 2, max - hitAreaTransform.rect.width / 2);

        //While the distance between the prevPos
        //is too small generate a new position
        //regenerate position.
        while (Mathf.Abs(prevPos - currentPos) < hitAreaTransform.rect.width * 2)
        {
            currentPos = Random.Range(min + hitAreaTransform.rect.width / 2, max - hitAreaTransform.rect.width / 2);
        }
        hitAreaTransform.anchoredPosition = new Vector2(currentPos, hitAreaTransform.anchoredPosition.y);

        prevPos = currentPos;
    }
    

    public IEnumerator ColorDelay()
    {
        //make the color cyan for 0.5f seconds.
        float time = 0.5f;
        while (time > 0)
        {
            hitAreaTransform.gameObject.GetComponent<Image>().color = Color.cyan;
            sliderMaterial.SetColor("_LineColor", Color.cyan);
            time -= Time.deltaTime;
            yield return null;
        }

        sliderMaterial.SetColor("_LineColor", Color.red);
        hitAreaTransform.gameObject.GetComponent<Image>().color = Color.red;
        if (repairCount >= 3)
        {
            repairCount = 0;
            //disable this object.
            //gameObject.SetActive(false);
            //Stop repairing.
            playerController.StopRepairing();
            //StopCoroutine(ColorDelay());
        }
        else
        {
            SetRandomHitPos();
        }
    }
}
