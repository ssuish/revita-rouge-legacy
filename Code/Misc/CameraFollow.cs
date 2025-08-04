using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public Vector3 offset; // Offset between the player and the camera

    private void LateUpdate()
    {
        if (player != null)
        {
            // Set the camera's position to the player's position plus the offset
            transform.position = player.position + offset;
        }
    }
}
