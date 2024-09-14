using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCheck : MonoBehaviour
{
    public Camera camera; // Gán Camera hoặc Cinemachine Virtual Camera vào đây

    void Start()
    {
        if (camera != null)
        {
            Debug.Log("Camera Size: " + camera.orthographicSize);
            Debug.Log("Camera Aspect Ratio: " + camera.aspect);
        }
    }
}
