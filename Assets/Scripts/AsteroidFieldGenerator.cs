using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidFieldGenerator : MonoBehaviour
{
    //the length of the sides of the field. (it's a cube).
    public int size;
    public int density;
    //public int width;
    //public int length;
    //public int height;

    public List<GameObject> asteroidPrefabs;
    public List<Collider> asteroidColliders;

    private List<GameObject> asteroids = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        //Found some helpful code here: https://forum.unity.com/threads/randomly-generate-objects-inside-of-a-box.95088/
        //Spawn asteroids.

        //Generate list of the asteroid colliders
        //from the list of asteroid prefabs.
        asteroidPrefabs.ForEach(obj => asteroidColliders.Add(obj.GetComponent<Collider>()));

        
        for (int i = 0; i < density; i++)
        {
            //Generate random index to select the prefab for use in this generation.
            int prefabIndex = Random.Range(0, asteroidPrefabs.Count - 1);
            Debug.Log(prefabIndex);

            Vector3 rndPos = new Vector3(Random.Range(-size, size), Random.Range(-size, size), Random.Range(-size, size));
            rndPos = transform.TransformPoint(rndPos * .5f);
            //Check if we are overlapping.
            while (Physics.BoxCast(rndPos, asteroidColliders[prefabIndex].bounds.size, Vector3.zero))
            {
                rndPos = new Vector3(Random.Range(-size, size), Random.Range(-size, size), Random.Range(-size, size));
                rndPos = transform.TransformPoint(rndPos * .5f);
            }
            //Instantiate random asteroid prefab at the already generated random position
            print(prefabIndex);
            asteroids.Add(Instantiate(asteroidPrefabs[prefabIndex], rndPos, Quaternion.identity));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(this.transform.position, new Vector3(size, size, size));
    }
}
