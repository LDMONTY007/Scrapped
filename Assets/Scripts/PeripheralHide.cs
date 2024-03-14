using UnityEngine;

public class PeripheralHide : MonoBehaviour
{
    Renderer renderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        if (Mathf.Abs(0.5f - pos.x) < 0.25f && Mathf.Abs(0.5f - pos.y) < 0.25f)
        {
            renderer.enabled = false;
        }
        else
        {
            renderer.enabled = true;
        }
    }
}
