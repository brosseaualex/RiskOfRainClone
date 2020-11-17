using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform target;
    public Transform player;
    public float camRotationSpeed = 1f;
    public float cameraMinimumY = -60f;
    public float cameraMaximumY = 75f;

    float mouseX = 90f;
    float mouseY;

    void LateUpdate()
    {
        if (!GameController.Instance.isPaused)
        {
            if (!GameController.Instance.hasLost && !GameController.Instance.hasWon)
            {
                ThirdPersonCamControl();
            }
        }
    }

    void ThirdPersonCamControl()
    {
        mouseX += Input.GetAxis("Mouse X") * camRotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * camRotationSpeed;

        mouseY = Mathf.Clamp(mouseY, cameraMinimumY, cameraMaximumY);

        target.rotation = Quaternion.Euler(mouseY, mouseX, 0);

        player.rotation = Quaternion.Euler(0, mouseX, 0);
    }
}
