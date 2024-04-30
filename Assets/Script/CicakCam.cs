using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CicakCam : MonoBehaviour
{
    PInput input;
    InputAction look, rightClick;
    Camera cam;
    const float lookSpeed = 0.5f;
    float verticalAngle;
    const float initCamFOV = 60, boostCamFOV = 80;

    private void Awake()
    {
        input = new PInput();
        verticalAngle = 0;
        cam = GetComponent<Camera>();
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

    public IEnumerator BoostCam()
    {
        while (cam.fieldOfView < boostCamFOV)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, boostCamFOV, 5 * Time.deltaTime);
            yield return null;
        }
    }

    public IEnumerator ResetFOV()
    {
        while (cam.fieldOfView > initCamFOV)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, initCamFOV, 5 * Time.deltaTime);
            yield return null;
        }
        cam.fieldOfView = initCamFOV;
    }

    private void Update()
    {
        if (rightClick.IsPressed())
        {
            verticalAngle -= look.ReadValue<Vector2>().y * lookSpeed;
            verticalAngle = Mathf.Clamp(verticalAngle, -90, 90);
            transform.localEulerAngles = new Vector3(verticalAngle, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }
    }
}
