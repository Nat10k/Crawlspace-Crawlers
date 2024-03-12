using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CicakMovement : MonoBehaviour
{
    PInput pInput;
    InputAction move, tongue, tail, look, rightClick;
    Rigidbody rb;
    const float moveSpeed = 0.5f, lookSpeed = 1f, gravityForce = 9.81f;
    Vector3 gravityDir;
    private void Awake()
    {
        pInput = new PInput();
        rb = GetComponent<Rigidbody>();
        gravityDir = transform.up * -1;
    }

    private void OnEnable()
    {
        move = pInput.Player.Move;
        tongue = pInput.Player.Fire;
        look = pInput.Player.Look;
        rightClick = pInput.Player.RightClick;
        rightClick.performed += LockCursor;
        rightClick.canceled += ReleaseCursor;
        move.Enable();
        tongue.Enable();
        look.Enable();
        rightClick.Enable();
    }

    private void OnDisable()
    {
        rightClick.performed -= LockCursor;
        rightClick.canceled -= ReleaseCursor;
        move.Disable();
        tongue.Disable();
        look.Disable();
        rightClick.Disable();
    }

    private void LockCursor(InputAction.CallbackContext ctx)
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void ReleaseCursor(InputAction.CallbackContext ctx)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void FixedUpdate()
    {
        Vector2 moveInput = move.ReadValue<Vector2>() * moveSpeed;
        rb.MovePosition(transform.position + (transform.forward * moveInput.y + transform.right * moveInput.x) * Time.fixedDeltaTime);
        if (rightClick.IsPressed())
        {
            transform.Rotate(new Vector3(0, look.ReadValue<Vector2>().x * lookSpeed, 0));
        }
        rb.AddForce(gravityDir * gravityForce);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            ContactPoint cp = collision.GetContact(0);
            transform.up = cp.normal;
            Debug.Log(transform.up);
            gravityDir = transform.up * -1;
        }
    }
}
