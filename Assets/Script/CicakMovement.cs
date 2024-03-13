using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CicakMovement : MonoBehaviour
{
    PInput pInput;
    InputAction move, tongueAction, tail, look, rightClick;
    Rigidbody rb;
    const float moveSpeed = 0.5f, lookSpeed = 1f, gravityForce = 9.81f;
    float tongueLength = 40;
    [SerializeField] Tongue tongue;
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
        tongueAction = pInput.Player.Fire;
        look = pInput.Player.Look;
        rightClick = pInput.Player.RightClick;

        rightClick.performed += LockCursor;
        rightClick.canceled += ReleaseCursor;
        tongueAction.performed += ShootTongue;
        move.Enable();
        tongueAction.Enable();
        look.Enable();
        rightClick.Enable();
    }

    private void OnDisable()
    {
        rightClick.performed -= LockCursor;
        rightClick.canceled -= ReleaseCursor;
        tongueAction.performed -= ShootTongue;
        move.Disable();
        tongueAction.Disable();
        look.Disable();
        rightClick.Disable();
    }

    private void ShootTongue(InputAction.CallbackContext ctx)
    {
        StartCoroutine(ExtendTongue());
    }

    IEnumerator ExtendTongue()
    {
        tongue.gameObject.SetActive(true);
        tongue.enabled = true;
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Vector3 shootPos;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            shootPos = hit.point;
        } else
        {
            shootPos = ray.origin + ray.direction * tongueLength;
        }
        tongue.transform.LookAt(shootPos);
        while (tongueAction.IsPressed())
        {
            if (tongue.GetHitWall())
            {
                rb.velocity = tongue.transform.forward;
            } else
            {
                tongue.transform.localPosition += 2.5f * Time.deltaTime * transform.InverseTransformDirection(tongue.transform.forward);
                tongue.transform.localScale += new Vector3(0, 0, 5*Time.deltaTime);
            }
            yield return null;
        }
        tongue.ResetTongue();
        tongue.enabled = false;
        tongue.gameObject.SetActive(false);
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
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Floor"))
        {
            ContactPoint cp = collision.GetContact(0);
            transform.up = cp.normal;
            Debug.Log(transform.up);
            gravityDir = transform.up * -1;
        }
    }
}
