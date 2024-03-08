using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Gotten from here:https://forum.unity.com/threads/gui-markers-waypoints.477084/
public class WaypointDrawer : MonoBehaviour
{

    public GameObject[] waypoints;
    public Transform playerLoc;

    public Vector3[] waypointLoc;

    void Start()
    {
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        waypointLoc = new Vector3[waypoints.Length];
    }

    void Update()
    {
        /*for (int i = 0; i < waypoints.Length; i++)
        {
            waypointLoc[i] = Camera.main.WorldToScreenPoint(waypoints[i].transform.position);
        }*/
    }

    private void LateUpdate()
    {
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypointLoc[i] = Camera.main.WorldToScreenPoint(waypoints[i].transform.position);
        }
    }

    void OnGUI()
    {

        for (int i = 0; i < waypoints.Length; i++)
        {
            //if the waypoint is not behind the camera render it in the GUI.
            if (waypointLoc[i].z > 0)
            {
                GUI.color = Color.cyan;
                GUI.Label(new Rect(waypointLoc[i].x + 6, Screen.height - waypointLoc[i].y, 100, 20), "Waypoint");
                GUI.Label(new Rect(waypointLoc[i].x - 6, Screen.height - waypointLoc[i].y, 100, 20), "▲");
            }
        }
    }
}