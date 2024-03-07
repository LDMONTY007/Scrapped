using UnityEngine;

public class RedGem : MonoBehaviour, IInteractible
{
    public void OnFocus(PlayerController p)
    {
        Debug.Log("On Focus!");
    }

    public void OnInteract(PlayerController p)
    {
        Debug.Log("On Interact!");
    }

    public void OnLostFocus(PlayerController p)
    {
        Debug.Log("Lost Focus!");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
