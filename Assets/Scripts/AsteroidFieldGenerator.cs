using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidFieldGenerator : MonoBehaviour
{
    //the length of the sides of the field. (it's a cube).
    public int scaleMin;
    public int scaleMax;
    public int size;
    public int density;
    //public int width;
    //public int length;
    //public int height;

    public List<GameObject> asteroidPrefabs;
    private List<Collider> asteroidColliders = new List<Collider>();

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
            //Generate random index to select the prefab for use in this iteration.
            int prefabIndex = Random.Range(0, asteroidPrefabs.Count);
            Debug.Log(prefabIndex);

            Vector3 rndScale = Vector3.one * Random.Range(scaleMin, scaleMax);
            Vector3 rndPos = new Vector3(Random.Range(-size, size), Random.Range(-size, size), Random.Range(-size, size));
            rndPos = transform.TransformPoint(rndPos * .5f);
            //Check if we are overlapping.
            while (Physics.CheckBox(rndPos, rndScale/*asteroidColliders[prefabIndex].bounds.size*/))
            {
                rndPos = new Vector3(Random.Range(-size, size), Random.Range(-size, size), Random.Range(-size, size));
                rndPos = transform.TransformPoint(rndPos * .5f);
            }
            //Instantiate random asteroid prefab at the already generated random position
            GameObject go = Instantiate(asteroidPrefabs[prefabIndex], rndPos, Quaternion.identity);
            go.transform.localScale = rndScale;
            asteroids.Add(go);
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
