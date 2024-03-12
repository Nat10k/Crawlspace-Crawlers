using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CicakMovement : MonoBehaviour
{
    PInput pInput;
    InputAction move, tongue, tail, look;
    Rigidbody rb;
    const float moveSpeed = 0.5f, lookSpeed = 0.5f, gravityForce = 9.81f;
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
        move.Enable();
        tongue.Enable();
        look.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        tongue.Disable();
        look.Disable();
    }

    private void FixedUpdate()
    {
        Vector2 moveInput = move.ReadValue<Vector2>() * moveSpeed;
        rb.MovePosition(transform.position + (transform.forward * moveInput.y + transform.right * moveInput.x) * Time.fixedDeltaTime);
        rb.MoveRotation(Quaternion.Euler(transform.rotation.eulerAngles +
            new Vector3(0, look.ReadValue<Vector2>().x, 0) * lookSpeed));
        rb.AddForce(gravityDir * gravityForce);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            ContactPoint cp = collision.GetContact(0);
            Debug.Log(cp.normal);
            transform.forward = transform.up + transform.forward;
            Debug.Log(transform.up);
            gravityDir = transform.up * -1;
        }
    }
}
