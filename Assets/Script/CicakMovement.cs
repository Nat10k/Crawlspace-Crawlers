using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CicakMovement : MonoBehaviour
{
    PInput pInput;
    InputAction move, tongueAction, tailInput, look, rightClick;
    Rigidbody rb;
    Ray frontRay, backRay, upRay;
    const float lookSpeed = 1f, gravityForce = 9.81f, wallDetectDist = 0.15f, scaleSpeed = 0.5f;
    float tongueLength = 40, moveSpeed = 1f, tailCooldown = 15f;
    bool justClimbed, hasTail;
    [SerializeField] Tongue tongue;
    [SerializeField] Transform tailObj;
    [SerializeField] Material cicakMaterial;
    FixedJoint tailJoint;
    Coroutine tongueFire, rotateAnim;
    Vector3 gravityDir, initTailPos, initTailScale;
    Quaternion initTailRot;
    private void Awake()
    {
        pInput = new PInput();
        rb = GetComponent<Rigidbody>();
        tailJoint = GetComponent<FixedJoint>();
        gravityDir = transform.up * -1;
        justClimbed = true;
        hasTail = true;
        initTailPos = tailObj.localPosition;
        initTailScale = tailObj.localScale;
        initTailRot = tailObj.localRotation;
    }

    private void OnEnable()
    {
        move = pInput.Player.Move;
        tongueAction = pInput.Player.Fire;
        look = pInput.Player.Look;
        rightClick = pInput.Player.RightClick;
        tailInput = pInput.Player.Tail;

        rightClick.performed += LockCursor;
        rightClick.canceled += ReleaseCursor;
        tongueAction.performed += ShootTongue;
        tongueAction.canceled += ReleaseTongue;
        tailInput.performed += SeperateTail;
        move.Enable();
        tongueAction.Enable();
        look.Enable();
        rightClick.Enable();
        tailInput.Enable();
    }

    private void OnDisable()
    {
        rightClick.performed -= LockCursor;
        rightClick.canceled -= ReleaseCursor;
        tongueAction.performed -= ShootTongue;
        tongueAction.canceled -= ReleaseTongue;
        tailInput.performed -= SeperateTail;
        move.Disable();
        tongueAction.Disable();
        look.Disable();
        rightClick.Disable();
        tailInput.Disable();
    }

    private void SeperateTail(InputAction.CallbackContext ctx)
    {
        if (hasTail)
        {
            hasTail = false;
            Destroy(tailJoint);
            tailObj.parent = null;
            StartCoroutine(TailBoost());
        }
    }

    IEnumerator TailBoost()
    {
        // Speed boost for 3 seconds after deattaching tail
        moveSpeed *= 2;
        cicakMaterial.color = new Color(cicakMaterial.color.r, cicakMaterial.color.g, cicakMaterial.color.b, 125);
        yield return new WaitForSeconds(3);
        moveSpeed /= 2;
        cicakMaterial.color = new Color(cicakMaterial.color.r, cicakMaterial.color.g, cicakMaterial.color.b, 255);
        StartCoroutine(ScaleTail(Vector3.zero));
        yield return new WaitForSeconds(tailCooldown);
        // Reattach tail after cooldown
        StartCoroutine(ScaleTail(initTailScale));
        tailObj.parent = transform;
        tailObj.localPosition = initTailPos;
        tailObj.localRotation = initTailRot;
        tailJoint = gameObject.AddComponent(typeof(FixedJoint)) as FixedJoint;
        tailJoint.connectedBody = tailObj.GetComponent<Rigidbody>();
        hasTail = true;
    }

    IEnumerator ScaleTail(Vector3 newScale)
    {
        while (tailObj.localScale != newScale)
        {
            tailObj.localScale = Vector3.MoveTowards(tailObj.localScale, newScale, scaleSpeed * Time.deltaTime);
            yield return null;
        }
        tailObj.localScale = newScale;
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
        frontRay = new Ray(transform.position, transform.forward);
        backRay = new Ray(transform.position, transform.forward * -1);
        upRay = new Ray(transform.position, transform.up);
        RaycastHit hit;
        if (Physics.Raycast(frontRay, out hit, wallDetectDist))
        {
            if (hit.transform.CompareTag("Wall") || hit.transform.CompareTag("Floor"))
            {
                ClimbWall(hit.normal, transform.up);
            }
        }
        else if (Physics.Raycast(backRay, out hit, wallDetectDist))
        {
            if (hit.transform.CompareTag("Wall") || hit.transform.CompareTag("Floor"))
            {
                ClimbWall(hit.normal, transform.up * -1);
            }
        }
        else if (Physics.Raycast(upRay, out hit, wallDetectDist))
        {
            if (hit.transform.CompareTag("Wall") || hit.transform.CompareTag("Floor"))
            {
                ClimbWall(hit.normal, new Vector3(0, transform.forward.z));
            }
        } else
        {
            justClimbed = false;
        }
        Vector2 moveInput = move.ReadValue<Vector2>() * moveSpeed;
        transform.Rotate(new Vector3(0, moveInput.x * lookSpeed * 2, 0));
        if (tongue.enabled && tongue.GetHitWall())
        {
            rb.velocity += transform.forward * moveInput.y + transform.right * moveInput.x;
        }
        else
        {
            rb.velocity = transform.forward * moveInput.y;
        }
        rb.AddForce(gravityDir * gravityForce);
    }

    //IEnumerator ClimbAnim(Vector3 hitNormal, Vector3 newForward)
    //{
    //    // Rotate the character to look at the wall while maintaining upwards direction
    //    Quaternion newRot = Quaternion.LookRotation(newForward, hitNormal);
    //    while (Quaternion.Angle(rb.rotation, newRot) > 10f)
    //    {
    //        rb.MoveRotation(Quaternion.Lerp(rb.rotation, newRot, Time.fixedDeltaTime * 5));
    //        yield return new WaitForFixedUpdate();
    //    }
    //    rb.MoveRotation(newRot);
    //    rotateAnim = null;
    //    //if (Vector3.Angle(transform.up, newUp) == 180)
    //    //{
    //    //    while (Vector3.Angle(transform.up,newUp) > 90)
    //    //    {
    //    //        transform.up = Vector3.MoveTowards(transform.up, transform.right, 5 * Time.deltaTime);
    //    //        yield return null;
    //    //    }
    //    //    while (Vector3.Distance(transform.up, newUp) > 0.001f)
    //    //    {
    //    //        transform.up = Vector3.MoveTowards(transform.up, newUp / 2, 5 * Time.deltaTime);
    //    //        yield return null;
    //    //    }
    //    //} else
    //    //{
    //    //    while (Vector3.Distance(transform.up, newUp) > 0.001f)
    //    //    {
    //    //        transform.up = Vector3.MoveTowards(transform.up, newUp / 2, 5 * Time.deltaTime);
    //    //        yield return null;
    //    //    }
    //    //}
    //    //transform.up = newUp;
    //}

    private IEnumerator RotateAnim(Quaternion initRotation, Quaternion endRotation)
    {
        float rotationProgress = 0;
        while (rotationProgress < 1)
        {
            rotationProgress += Time.fixedDeltaTime * 2;
            rb.MoveRotation(Quaternion.Lerp(initRotation, endRotation, rotationProgress));
            yield return new WaitForFixedUpdate();
        }
        rotateAnim = null;
    }

    public void StartRotateAnim(Quaternion initRotation, Quaternion endRotation)
    {
        if (rotateAnim != null)
        {
            return;
        }
        rotateAnim = StartCoroutine(RotateAnim(initRotation, endRotation));
    }

    private void ClimbWall(Vector3 hitNormal, Vector3 newForward)
    {
        if (rotateAnim != null || justClimbed)
        {
            return;
        }
        justClimbed = true;
        Quaternion newRot = Quaternion.LookRotation(newForward, hitNormal);
        rotateAnim = StartCoroutine(RotateAnim(rb.rotation, newRot));
        gravityDir = hitNormal * -1;
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Floor"))
    //    {
    //        if (tongueFire != null && tongue.GetHitWall())
    //        {
    //            StopCoroutine(tongueFire);
    //            StartCoroutine(tongue.RetractTongue());
    //        }
    //        ContactPoint cp = collision.GetContact(0);
    //        if (climbAnim != null)
    //        {
    //            StopCoroutine(climbAnim);
    //            climbAnim = null;
    //        }
    //        climbAnim = StartCoroutine(ClimbAnim(cp.normal));
    //        gravityDir = cp.normal * -1;
    //    }
    //}
}
