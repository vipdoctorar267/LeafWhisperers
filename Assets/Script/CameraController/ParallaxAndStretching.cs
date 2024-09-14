using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxAndStretching : MonoBehaviour
{
    public Transform cameraTransform; // Camera mà background sẽ theo dõi
    public Transform backgroundTransform; // Background cần điều chỉnh
    public BoxCollider2D cameraBounds; // Collider giới hạn di chuyển của camera
    public float parallaxEffectMultiplier = 0.5f; // Hệ số parallax

    private Vector3 lastCameraPosition;
    private float backgroundStartX, backgroundStartY;

    private void Start()
    {
        lastCameraPosition = cameraTransform.position;
        backgroundStartX = backgroundTransform.position.x;
        backgroundStartY = backgroundTransform.position.y;
    }

    private void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

        // Hiệu ứng Parallax
        float parallaxX = deltaMovement.x * parallaxEffectMultiplier;
        float parallaxY = deltaMovement.y * parallaxEffectMultiplier;

        // Giới hạn di chuyển của background trong collider
        float newBackgroundX = Mathf.Clamp(backgroundTransform.position.x + parallaxX,
                                            cameraBounds.bounds.min.x,
                                            cameraBounds.bounds.max.x);
        float newBackgroundY = Mathf.Clamp(backgroundTransform.position.y + parallaxY,
                                            cameraBounds.bounds.min.y,
                                            cameraBounds.bounds.max.y);

        backgroundTransform.position = new Vector3(newBackgroundX, newBackgroundY, backgroundTransform.position.z);

        lastCameraPosition = cameraTransform.position;
    }
}
