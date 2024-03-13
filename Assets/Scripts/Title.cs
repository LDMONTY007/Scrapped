using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    public TextMeshProUGUI titleText;

    public Button StartButton;
    public Button QuitButton;
    public TextMeshProUGUI StartText;
    public TextMeshProUGUI QuitText;

    private void Awake()
    {
        titleText.alpha = 0f;
        Image startImage = StartButton.GetComponent<Image>();
        Image quitImage = QuitButton.GetComponent<Image>();
        startImage.color = new Color(startImage.color.r, startImage.color.g, startImage.color.b, 0f);
        quitImage.color = new Color(quitImage.color.r, quitImage.color.g, quitImage.color.b, 0f);
        QuitText.alpha = 0f;
        StartText.alpha = 0f;
        StartButton.interactable = false;
        QuitButton.interactable = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (Time.timeScale != 1f)
        {
            Time.timeScale = 1f;
        }

        StartCoroutine(LerpTextAlphaCoroutine(0.5f, titleText));
        StartCoroutine(delayCoroutine(0.5f, LerpTextAlphaCoroutine(0.5f, StartText)));
        StartCoroutine(delayCoroutine(0.5f, LerpTextAlphaCoroutine(0.5f, QuitText)));
        StartCoroutine(delayCoroutine(0.5f, LerpButtonAlphaCoroutine(0.5f, StartButton, StartButton.GetComponent<Image>())));
        StartCoroutine(delayCoroutine(0.5f, LerpButtonAlphaCoroutine(0.5f, QuitButton, QuitButton.GetComponent<Image>())));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScene(string scene)
    {
        //if the game was paused 
        //then swap over to unpaused
        //by setting time scale to 1.
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        Debug.Log("LOADING SCENE");
        SceneManager.LoadScene(scene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public IEnumerator LerpTextAlphaCoroutine(float totalTime, TextMeshProUGUI text)
    {
        float currentTime = 0f;
        Vector3 startPos = transform.position;
        while (currentTime < totalTime)
        {
            if (currentTime >= totalTime - 0.01f)
            {
                currentTime = totalTime;
                text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);
                break;
            }
            //Lerp color.
            text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Lerp(0f, 1f, currentTime / totalTime));
            currentTime += Time.deltaTime;
            yield return null;
        }
        //OnInteractEvent.Invoke();
        //Destroy(gameObject, 0.01f);

    }

    public IEnumerator LerpButtonAlphaCoroutine(float totalTime, Button button, Image image)
    {
        float currentTime = 0f;
        Vector3 startPos = transform.position;
        button.interactable = false;
        while (currentTime < totalTime)
        {
            if (currentTime >= totalTime - 0.01f)
            {
                currentTime = totalTime;
                image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
                break;
            }
            //Lerp color.
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(0f, 1f, currentTime / totalTime));
            currentTime += Time.deltaTime;
            yield return null;
        }
        button.interactable = true;
        //OnInteractEvent.Invoke();
        //Destroy(gameObject, 0.01f);

    }

    public IEnumerator delayCoroutine(float delay, IEnumerator coroutine)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(coroutine);
    }

}
