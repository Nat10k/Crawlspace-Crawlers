using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CicakCam : MonoBehaviour
{
    PInput input;
    InputAction look, rightClick;
    const float lookSpeed = 0.5f;
    float verticalAngle, horizontalAngle;

    private void Awake()
    {
        input = new PInput();
        verticalAngle = 0;
        horizontalAngle = 0;
    }

    private void OnEnable()
    {
        look = input.Player.Look;
        rightClick = input.Player.RightClick;
        rightClick.canceled += ResetHorizontalAngle;
        look.Enable();
        rightClick.Enable();
    }

    private void OnDisable()
    {
        look.Disable();
        rightClick.Disable();
    }

    private void ResetHorizontalAngle(InputAction.CallbackContext ctx)
    {
        horizontalAngle = 0;
        transform.localEulerAngles = new Vector3(verticalAngle, horizontalAngle, transform.localEulerAngles.z);
    }

    private void Update()
    {
        if (rightClick.IsPressed())
        {
            verticalAngle -= look.ReadValue<Vector2>().y * lookSpeed;
            horizontalAngle += look.ReadValue<Vector2>().x * lookSpeed;
            verticalAngle = Mathf.Clamp(verticalAngle, -90, 90);
            transform.localEulerAngles = new Vector3(verticalAngle, horizontalAngle, transform.localEulerAngles.z);
        }
    }
}
