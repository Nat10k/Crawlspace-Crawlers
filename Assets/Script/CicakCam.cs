using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CicakCam : MonoBehaviour
{
    PInput input;
    InputAction look, rightClick;
    const float lookSpeed = 0.5f;
    float verticalAngle;

    private void Awake()
    {
        input = new PInput();
        verticalAngle = 0;
    }

    private void OnEnable()
    {
        look = input.Player.Look;
        rightClick = input.Player.RightClick;
        look.Enable();
        rightClick.Enable();
    }

    private void OnDisable()
    {
        look.Disable();
        rightClick.Disable();
    }

    private void Update()
    {
        if (rightClick.IsPressed())
        {
            verticalAngle -= look.ReadValue<Vector2>().y * lookSpeed;
            verticalAngle = Mathf.Clamp(verticalAngle, -30, 90);
            transform.localEulerAngles = new Vector3(verticalAngle, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }
    }
}
