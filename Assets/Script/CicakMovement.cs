using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CicakMovement : MonoBehaviour
{
    PInput pInput;
    InputAction move, tongueAction, tail, look, rightClick;
    Rigidbody rb;
    const float moveSpeed = 1f, lookSpeed = 1f, gravityForce = 9.81f;
    float tongueLength = 40;
    [SerializeField] Tongue tongue;
    Coroutine tongueFire, climbAnim;
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
        tongueAction.canceled += ReleaseTongue;
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
        tongueAction.canceled -= ReleaseTongue;
        move.Disable();
        tongueAction.Disable();
        look.Disable();
        rightClick.Disable();
    }

    private void ShootTongue(InputAction.CallbackContext ctx)
    {
        tongue.gameObject.SetActive(true);
        tongue.enabled = true;
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Vector3 shootPos;
        if (Physics.Raycast(ray, out RaycastHit hit, tongueLength))
        {
            shootPos = hit.point;
        }
        else
        {
            shootPos = ray.origin + ray.direction * tongueLength;
        }
        tongueFire = StartCoroutine(tongue.ShootTongue(shootPos));
    }

    private void ReleaseTongue(InputAction.CallbackContext ctx)
    {
        if (tongueFire != null)
        {
            StopCoroutine(tongueFire);
            StartCoroutine(tongue.RetractTongue());
        }
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
        if (tongue.enabled && tongue.GetHitWall())
        {
            rb.velocity += transform.forward * moveInput.y + transform.right * moveInput.x;
        }
        else
        {
            rb.velocity = transform.forward * moveInput.y + transform.right * moveInput.x;
        }
        if (rightClick.IsPressed())
        {
            transform.Rotate(new Vector3(0, look.ReadValue<Vector2>().x * lookSpeed, 0));
        }
        rb.AddForce(gravityDir * gravityForce);
    }

    IEnumerator ClimbAnim(Vector3 newUp)
    {
        if (Vector3.Angle(transform.up, newUp) == 180)
        {
            while (Vector3.Angle(transform.up,newUp) > 90)
            {
                transform.up = Vector3.MoveTowards(transform.up, transform.right, 5 * Time.deltaTime);
                yield return null;
            }
            while (Vector3.Distance(transform.up, newUp) > 0.001f)
            {
                transform.up = Vector3.MoveTowards(transform.up, newUp / 2, 5 * Time.deltaTime);
                yield return null;
            }
        } else
        {
            while (Vector3.Distance(transform.up, newUp) > 0.001f)
            {
                transform.up = Vector3.MoveTowards(transform.up, newUp, 5 * Time.deltaTime);
                yield return null;
            }
        }
        transform.up = newUp;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Floor"))
        {
            if (tongueFire != null && tongue.GetHitWall())
            {
                StopCoroutine(tongueFire);
                StartCoroutine(tongue.RetractTongue());
            }
            ContactPoint cp = collision.GetContact(0);
            if (climbAnim != null)
            {
                StopCoroutine(climbAnim);
                climbAnim = null;
            }
            climbAnim = StartCoroutine(ClimbAnim(cp.normal));
            gravityDir = cp.normal * -1;
        }
    }
}
