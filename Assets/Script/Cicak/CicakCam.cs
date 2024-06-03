using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CicakCam : MonoBehaviour
{
    InputAction look;
    Camera cam;
    Coroutine camRoutine;
    [SerializeField] CicakMovement cm;
    const float lookSpeed = 0.5f;
    float verticalAngle;
    const float initCamFOV = 80, boostCamFOV = 100;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        verticalAngle = 0;
        cam = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        look = InputHandler.inputs.Player.Look;
        look.Enable();
    }

    private void OnDisable()
    {
        look.Disable();
    }

    public void BoostCam()
    {
        if (camRoutine != null)
        {
            StopCoroutine(camRoutine);
        }
        camRoutine = StartCoroutine(BoostCamRoutine());
    }

    public void ResetFOV()
    {
        if (camRoutine != null)
        {
            StopCoroutine(camRoutine);
        }
        camRoutine = StartCoroutine(ResetFOVRoutine());
    }

    private IEnumerator BoostCamRoutine()
    {
        while (cam.fieldOfView < boostCamFOV)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, boostCamFOV, 5 * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator ResetFOVRoutine()
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
        if (GameManager.isPaused || !cm.canMove)
        {
            return;
        }
        verticalAngle -= look.ReadValue<Vector2>().y * lookSpeed;
        verticalAngle = Mathf.Clamp(verticalAngle, -90, 90);
        transform.localEulerAngles = new Vector3(verticalAngle, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }
}
