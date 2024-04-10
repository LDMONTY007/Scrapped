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
        if (playerControlPanel && shipControlPanel)
        {
            playerControlPanel.SetActive(false);
            shipControlPanel.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerControlPanel)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Pause();
            }
        }
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

                if (playerControlPanel && shipControlPanel)
                {
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
                if (playerControlPanel && shipControlPanel)
                {
                    if (playerController.isControllingShip)
                    {
                        shipPanel.SetActive(true);
                    }
                    else
                    {
                        playerPanel.SetActive(true);
                    }
                }


                //Only set time scale back to 1 if the
                //PDA is not open. 
                //This is because the PDA also freezes 
                //Time while we are reading it.
                if (!PdaController.Instance.isPDAOpen)
                {
                    AudioListener.pause = false;
                    Time.timeScale = 1f;
                }

                //Turn off the UI for the pause panel.
                pausePanel.SetActive(false);
                if (playerControlPanel && shipControlPanel)
                {
                    shipControlPanel.SetActive(false);
                    playerControlPanel.SetActive(false);
                    //Only hide the cursor if we are not controlling the ship or not in a UI menu like the PDA
                    if (!playerController.isControllingShip && !PdaController.Instance.isPDAOpen)
                    {
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                    }
                }
            }
        }
    }
}
