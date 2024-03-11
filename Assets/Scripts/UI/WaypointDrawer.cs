using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//Gotten from here:https://forum.unity.com/threads/gui-markers-waypoints.477084/
public class WaypointDrawer : MonoBehaviour
{

    public bool useProximity = true;
    public float proxDistance = 15f;

    public Color waypointColor = Color.cyan;

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

        VerifyWaypoints();

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
        VerifyWaypoints();
        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] != null && waypointLoc[i] != null)
            {
                waypointLoc[i] = Camera.main.WorldToScreenPoint(waypoints[i].transform.position);
            }
            
        }


    }

    public void VerifyWaypoints()
    {
        List<int> indexesToRemove = new List<int>();

        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] == null)
            {
                //add the index that needs to be removed.
                //We'll remove it after we're done with this loop.
                indexesToRemove.Add(i);
            }

        }

        //remove any null waypoints.
        foreach (int i in indexesToRemove)
        {
            waypoints.RemoveAt(i);
            waypointLoc.RemoveAt(i);
        }
    }

    void OnGUI()
    {
        VerifyWaypoints() ;
        for (int i = 0; i < waypoints.Count; i++)
        {
            //if the waypoint is not behind the camera render it in the GUI.
            if (waypointLoc[i].z > 0)
            {
                if (useProximity)
                {

                    Debug.Log("Player:" + playerLoc.position + "Waypoint:" + waypoints[i].transform.position + "Dist:" + Vector3.Distance(playerLoc.position, waypoints[i].transform.position));
                    if (Vector3.Distance(playerLoc.position, waypoints[i].transform.position) <= proxDistance)
                    {
                        GUI.color = waypointColor;
                        GUI.Label(new Rect(waypointLoc[i].x + 6, Screen.height - waypointLoc[i].y, 100, 20), waypoints[i].name);
                        GUI.Label(new Rect(waypointLoc[i].x - 6, Screen.height - waypointLoc[i].y, 100, 20), "▲");
                    }
                    else
                    {
                        //Don't show them.
                    }
                }
                else
                {
                    GUI.color = waypointColor;
                    GUI.Label(new Rect(waypointLoc[i].x + 6, Screen.height - waypointLoc[i].y, 100, 20), waypoints[i].name);
                    GUI.Label(new Rect(waypointLoc[i].x - 6, Screen.height - waypointLoc[i].y, 100, 20), "▲");
                }
            }
        }
    }
}