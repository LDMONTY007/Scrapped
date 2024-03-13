using UnityEngine;

public class CollisionIdentifier : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.LogWarning(collision.gameObject.name);
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.LogWarning("Particle: " + other.name);
    }
}
