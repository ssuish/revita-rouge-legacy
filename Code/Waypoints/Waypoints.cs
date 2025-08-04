using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    [Range(0f, 1f)] [SerializeField] private float waypointSize = 1f;

    [Header("Path Settings")]
    // If the path can loop back to the first waypoint
    [SerializeField]
    private bool canLoop = true;

    // If the path is moving forward or backward
    [SerializeField] private bool isMovingForward = true;
    [SerializeField] private bool canLoopOnLine = false;

    private void Start()
    {
        // Set the size of the waypoints
        foreach (Transform t in transform)
        {
            t.localScale = new Vector3(waypointSize, waypointSize, waypointSize);
        }
    }

    private void OnEnable()
    {
        GameEventsManagerSO.instance.miscEvents.onWayPointGateClosed += DestroyWaypointIndex;
    }

    private void OnDisable()
    {
        GameEventsManagerSO.instance.miscEvents.onWayPointGateClosed -= DestroyWaypointIndex;
    }

    private void OnDrawGizmos()
    {
        foreach (Transform t in transform)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(t.position, 0.5f);
        }

        Gizmos.color = Color.red;
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
        }

        // If the path can loop back to the first waypoint draw a line from the last to the first waypoint
        if (canLoop)
        {
            Gizmos.DrawLine(transform.GetChild(transform.childCount - 1).position, transform.GetChild(0).position);
        }
    }

    public Transform GetNearestWaypoint(Vector3 position)
    {
        Transform nearestWaypoint = null;
        float smallestDistance = float.MaxValue;

        foreach (Transform waypoint in transform)
        {
            float distance = Vector3.Distance(position, waypoint.position);
            Debug.Log($"Checking waypoint at {waypoint.position} with distance {distance}");

            if (distance < smallestDistance)
            {
                smallestDistance = distance;
                nearestWaypoint = waypoint;
                Debug.Log($"New nearest waypoint at {waypoint.position} with distance {smallestDistance}");
            }
        }

        if (nearestWaypoint == null)
        {
            Debug.LogError("No waypoints found.");
        }
        else
        {
            Debug.Log($"Nearest waypoint found at {nearestWaypoint.position} with distance {smallestDistance}");
        }

        return nearestWaypoint;
    }

    public Transform GetNextWaypoint(Transform currentWaypoint)
    {
        if (currentWaypoint == null)
        {
            return transform.GetChild(0);
        }

        // index of the current waypoint
        int currentIndex = currentWaypoint.GetSiblingIndex();
        // index of the next waypoint
        int nextIndex = currentIndex;

        if (isMovingForward)
        {
            nextIndex++;

            // If the next index is greater than the number of waypoints, set it to the first waypoint

            if (nextIndex >= transform.childCount)
            {
                if (canLoop)
                {
                    nextIndex = 0;
                }
                else if (canLoopOnLine)
                {
                    isMovingForward = false;
                    nextIndex = transform.childCount - 2;
                }
                else
                {
                    nextIndex--;
                }
            }
        }
        else
        {
            nextIndex--;

            // If the next index is less than 0, set it to the last waypoint

            if (nextIndex < 0)
            {
                if (canLoop)
                {
                    nextIndex = transform.childCount - 1;
                }
                else if (canLoopOnLine)
                {
                    isMovingForward = true;
                    nextIndex = 1;
                }
                else
                {
                    nextIndex++;
                }
            }
        }

        return transform.GetChild(nextIndex);
    }

    private void DestroyWaypointIndex(int index, bool isBefore, bool LineLoop, bool Looping)
    {
        if (index < 0 || index >= transform.childCount)
        {
            Debug.LogError("Index out of range.");
            return;
        }


        // Toggling waypoint loop config
        canLoop = Looping;
        canLoopOnLine = LineLoop;

        if (isBefore)
        {
            // Destroy all waypoints before the specified index
            for (int i = 0; i < index; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
        else
        {
            // Destroy all waypoints after the specified index
            for (int i = index + 1; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}