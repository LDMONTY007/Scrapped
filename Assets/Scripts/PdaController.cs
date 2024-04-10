using UnityEngine;
using System.Text.RegularExpressions;
using TMPro;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class PdaController : MonoBehaviour
{
    public bool isPDAOpen = false;

    public static PdaController Instance { get; private set; }

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI contentText;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!PlayerController.instance.isControllingShip)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        gameObject.SetActive(isPDAOpen);
    }

    // Update is called once per frame
    void Update()
    {
          
    }

    public void ClosePDA()
    {
        isPDAOpen=false;
        if (!PlayerController.instance.isControllingShip)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        gameObject.SetActive(isPDAOpen);
        //unpause time if so everything goes back to normal.
        Time.timeScale = 1f;
    }


    public void Hide(bool shouldReveal)
    {
        if (!shouldReveal)
        {
            GetComponent<Image>().enabled = false;
            foreach (Transform t in GetComponentsInChildren<Transform>())
            {
                t.gameObject.SetActive(false);
            }
        }
        else
        {
            GetComponent<Image>().enabled = true;
            foreach (Transform t in GetComponentsInChildren<Transform>())
            {
                t.gameObject.SetActive(true);
            }
        }

    }

    public void OpenPDA(bool isTextFromFile, string title, string content)
    {
        if (isTextFromFile)
        {
            //When loading things from a file the text formatting 
            //will keep escape characters instead of reading them as unicode
            //so we need to use Regex.Unescape to make them convert to unicode.
            //For Example, /n would literally just show up as /n and not a newline.
            //Found here: https://forum.unity.com/threads/programmatically-set-text-doesnt-format-by-itself.486198/#post-7328110
            titleText.text = Regex.Unescape(title);
            contentText.text = Regex.Unescape(content);
        }
        else
        {
            titleText.text = title;
            contentText.text = content;
        }
        isPDAOpen = true;
        if (!PlayerController.instance.isControllingShip)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        gameObject.SetActive(isPDAOpen);
        //Pause time so nothing happens while reading.
        Time.timeScale = 0f;
    }

    //https://stackoverflow.com/questions/67744910/importing-each-line-from-text-file-from-resources-in-unity-to-list-in-c-sharp
    //Line 0 is always the title.
    public void OpenPDA(string fileName)
    {
        string[] fileContent = File.ReadAllLines(Path.Combine(Application.streamingAssetsPath, fileName));
        string content = null;
        string title = null;
        for (int i = 0; i < fileContent.Length; i++)
        {
            if (i == 0)
            {
                title = fileContent[i];
            }
            else
            {
                content += fileContent[i] + "\n";
            }
        }
        titleText.text = Regex.Unescape(title);
        contentText.text = Regex.Unescape(content);
        isPDAOpen = true;
        if (!PlayerController.instance.isControllingShip)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        gameObject.SetActive(isPDAOpen);
        //Pause time so nothing happens while reading.
        Time.timeScale = 0f;
    }
    
}
