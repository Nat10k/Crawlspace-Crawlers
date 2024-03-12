using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class CicakCam : MonoBehaviour
{
    PInput input;
    InputAction look;
    const float lookSpeed = 0.5f;
    float verticalAngle;

    private void Awake()
    {
        input = new PInput();
        Cursor.lockState = CursorLockMode.Locked;
        verticalAngle = 0;
    }

    private void OnEnable()
    {
        look = input.Player.Look;
        look.Enable();
    }

    private void OnDisable()
    {
        look.Disable();
    }

    private void Update()
    {
        verticalAngle -= look.ReadValue<Vector2>().y * lookSpeed;
        verticalAngle = Mathf.Clamp(verticalAngle, -90, 90);
        transform.localEulerAngles = new Vector3(verticalAngle, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }
}
