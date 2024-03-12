using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public bool canPause = true;
    public bool isPaused;

    public PlayerController playerController;
    public GameObject pausePanel;
    public GameObject playerControlPanel;
    public GameObject shipControlPanel;
    public GameObject playerPanel;
    public GameObject shipPanel;

    public void SetCanPause(bool value)
    {
        canPause = value;
    }

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
        if (canPause)
        {
            isPaused = !isPaused;
            if (isPaused)
            {
                AudioListener.pause = true;
                Time.timeScale = 0f;
                pausePanel.SetActive(true);
                playerControlPanel.SetActive(true);
                shipControlPanel.SetActive(true);

                if (playerController.isControllingShip)
                {
                    shipPanel.SetActive(false);
                }
                else
                {
                    playerPanel.SetActive(false);
                }
                if (playerController != null)
                {
                    if (!playerController.isControllingShip)
                    {
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                    }
                }
            }
            else
            {
                if (playerController.isControllingShip)
                {
                    shipPanel.SetActive(true);
                }
                else
                {
                    playerPanel.SetActive(true);
                }

                AudioListener.pause = false;
                Time.timeScale = 1f;
                pausePanel.SetActive(false);
                shipControlPanel.SetActive(false);
                playerControlPanel.SetActive(false);
                if (!playerController.isControllingShip)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }
    }
}
