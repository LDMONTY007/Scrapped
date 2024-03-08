using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
}
