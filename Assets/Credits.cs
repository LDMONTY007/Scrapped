using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    [TextArea]
    public string content = string.Empty;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI contentText;

    public Scrollbar verticalScrollbar;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (Time.timeScale != 1f)
        {
            Time.timeScale = 1f;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator LerpCoroutine(float totalTime)
    {
        float currentTime = 0f;
        Vector3 startPos = transform.position;
        while (currentTime < totalTime)
        {
            if (currentTime >= totalTime - 0.01f)
            {
                currentTime = totalTime;
            }
            verticalScrollbar.value = Mathf.Lerp(0f, 1f, currentTime / totalTime);
            currentTime += Time.deltaTime;
            yield return null;
        }
        //OnInteractEvent.Invoke();
        //Destroy(gameObject, 0.01f);

    }

}
