using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public bool isPaused;

    public GameObject pausePanel;
    public GameObject playerControlPanel;
    public GameObject shipControlPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pausePanel.SetActive(false);
        playerControlPanel.SetActive(false);
        shipControlPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Pause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            AudioListener.pause = true;
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
            playerControlPanel.SetActive(true);
            shipControlPanel.SetActive(true);
        }
        else
        {
            AudioListener.pause = false;
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
            shipControlPanel.SetActive(false);
            playerControlPanel.SetActive(false);
        }
    }
}
