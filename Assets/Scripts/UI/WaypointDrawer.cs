using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//Gotten from here:https://forum.unity.com/threads/gui-markers-waypoints.477084/
public class WaypointDrawer : MonoBehaviour
{
    public List<string> tags = new List<string>();

    //Note for some reason in order to draw anything on the UI
    //the object for the waypoint needs some sort of component
    //but I've only seen it work with rendering based components
    //like mesh renderers or lights.
    public List<GameObject> waypoints = new List<GameObject>();
    public Transform playerLoc;

    public List<Vector3> waypointLoc = new List<Vector3>();



    void Start()
    {
        Debug.Log(gameObject.name);
        //Add each range with the tag we want. 
        Debug.Log("Tags:" + tags.Count);
        for (int i = 0; i < tags.Count; i++)
        {
            waypoints.AddRange(GameObject.FindGameObjectsWithTag(tags[i]));
        }
        Debug.Log("Waypoints" + waypoints.Count);
        //waypoints.AddRange(GameObject.FindGameObjectsWithTag("Waypoint"));


        //Create a list of the waypoint locations
        //and make sure that they are allocated for later use.
        waypointLoc.AddRange(new Vector3[waypoints.Count]);
    }

    void Update()
    {
        /*for (int i = 0; i < waypoints.Length; i++)
        {
            waypointLoc[i] = Camera.main.WorldToScreenPoint(waypoints[i].transform.position);
        }*/

        //O(n^2) really inefficient way to get all the newly spawned tags.
        //I don't have time to design an interface for waypoints with a callback
        //to add them.
        for (int i = 0; i < tags.Count; i++)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tags[i]);
            for (int j = 0; j < objects.Length; j++)
            {
                if (!waypoints.Contains(objects[j]))
                {
                    waypoints.Add(objects[j]);
                    waypointLoc.Add(Camera.main.WorldToScreenPoint(waypoints[i].transform.position));
                }
            }
        }
    }

    private void LateUpdate()
    {
        for (int i = 0; i < waypoints.Count; i++)
        {
            waypointLoc[i] = Camera.main.WorldToScreenPoint(waypoints[i].transform.position);
        }
    }

    void OnGUI()
    {

        for (int i = 0; i < waypoints.Count; i++)
        {
            //if the waypoint is not behind the camera render it in the GUI.
            if (waypointLoc[i].z > 0)
            {
                GUI.color = Color.cyan;
                GUI.Label(new Rect(waypointLoc[i].x + 6, Screen.height - waypointLoc[i].y, 100, 20), waypoints[i].name);
                GUI.Label(new Rect(waypointLoc[i].x - 6, Screen.height - waypointLoc[i].y, 100, 20), "▲");
            }
        }
    }
}