using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager
{
    private static GameManager _instance;

    [HideInInspector]
    public static GameManager instance
    {
        get
        {
            if (_instance == null)
                _instance = new GameManager();
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }


    public bool didGetToEnding = false;
    public bool didSaveAllAstronauts = false;
    public bool didCollectAllBeacons = false;
    public bool didSurvive = false;

    public void EndGame()
    {
        instance.didGetToEnding = true;
        Debug.Log(instance.didGetToEnding);
        SceneManager.LoadScene("TitleScene");
    }
}
