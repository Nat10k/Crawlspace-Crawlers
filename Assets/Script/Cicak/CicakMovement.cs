using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CicakMovement : MonoBehaviour
{
    InputAction move, tongueAction, tailInput, look, rightClick;
    Rigidbody rb;
    const float lookSpeed = 1f, gravityForce = 17f, wallDetectDist = 0.2f;
    float moveSpeed = 2f, tailCooldown = 15f, scaleSpeed;
    bool justClimbed, hasTail, isGrounded, canMove, justHit;
    [SerializeField] Tongue tongue;
    [SerializeField] Transform tailObj, headBone, spine;
    [SerializeField] Material cicakMaterial;
    [SerializeField] TutorialTrigger trigger;
    CicakCam cicakCam;
    Coroutine tongueFire, rotateAnim, tailScaleAnim;
    Vector3 gravityDir, initTailPos, initTailScale;
    readonly Vector3[] allAxis = { Vector3.forward, Vector3.back, Vector3.right, Vector3.left, Vector3.up, Vector3.down };
    Quaternion initTailRot;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cicakCam = Camera.main.GetComponent<CicakCam>();

        gravityDir = transform.up * -1;
        justClimbed = false;
        justHit = false;
        hasTail = true;
        isGrounded = true;
        canMove = true;

        initTailPos = tailObj.localPosition;
        initTailScale = tailObj.localScale;
        initTailRot = tailObj.localRotation;

        scaleSpeed = initTailScale.magnitude / 2;
    }

    private void OnEnable()
    {
        move = InputHandler.inputs.Player.Move;
        tongueAction = InputHandler.inputs.Player.Fire;
        look = InputHandler.inputs.Player.Look;
        rightClick = InputHandler.inputs.Player.RightClick;
        tailInput = InputHandler.inputs.Player.Tail;

        rightClick.performed += LockCursor;
        rightClick.canceled += ReleaseCursor;
        tongueAction.performed += ShootTongue;
        tongueAction.canceled += ReleaseTongue;
        tailInput.performed += SeparateTail;
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
        tailInput.performed -= SeparateTail;
        move.Disable();
        tongueAction.Disable();
        look.Disable();
        rightClick.Disable();
        tailInput.Disable();
    }

    private void SeparateTail(InputAction.CallbackContext ctx)
    {
        if (hasTail)
        {
            if (tailScaleAnim != null)
            {
                StopCoroutine(tailScaleAnim);
                tailScaleAnim = null;
            }
            hasTail = false;
            tailObj.parent = null;
            StartCoroutine(TailBoost());
        }
    }

    IEnumerator TailBoost()
    {
        // Speed boost for 5 seconds after detaching tail
        cicakCam.BoostCam();
        moveSpeed *= 2;
        cicakMaterial.color = new Color(cicakMaterial.color.r, cicakMaterial.color.g, cicakMaterial.color.b, 125);
        yield return new WaitForSeconds(5);
        cicakCam.ResetFOV();
        moveSpeed /= 2;
        cicakMaterial.color = new Color(cicakMaterial.color.r, cicakMaterial.color.g, cicakMaterial.color.b, 255);
        tailScaleAnim = StartCoroutine(ScaleTail(Vector3.zero));
        yield return new WaitForSeconds(tailCooldown - 2);
        // Reattach tail after cooldown
        tailScaleAnim = StartCoroutine(ScaleTail(initTailScale));
        tailObj.parent = transform;
        tailObj.localPosition = initTailPos;
        tailObj.localRotation = initTailRot;
    }

    IEnumerator ScaleTail(Vector3 newScale)
    {
        while (Mathf.Abs(tailObj.localScale.magnitude - newScale.magnitude) > 0.1f)
        {
            tailObj.localScale = Vector3.MoveTowards(tailObj.localScale, newScale, scaleSpeed * Time.deltaTime);
            yield return null;
        }
        tailObj.localScale = newScale;
        tailScaleAnim = null;
        if (newScale != Vector3.zero)
            hasTail = true;
    }

    private void ShootTongue(InputAction.CallbackContext ctx)
    {
        tongueFire = StartCoroutine(tongue.ShootTongue());
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
        RaycastHit hit;
        if (canMove)
        {
            Vector2 moveInput = move.ReadValue<Vector2>() * moveSpeed;
            if (trigger != null && moveInput != Vector2.zero)
            {
                trigger.TriggerEvent();
                trigger = null;
            }

            if (justHit)
            {
                if (tongueFire != null && tongue.GetHitWall())
                {
                    StopCoroutine(tongueFire);
                    StartCoroutine(tongue.RetractTongue());
                }
            }

            if (!tongue.GetHitWall())
            {
                // Climb forward
                if (Physics.Raycast(transform.position, transform.forward, out hit, wallDetectDist))
                {
                    if (Vector3.Angle(transform.up, hit.normal) > 60 && (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Floor")))
                    {
                        justHit = true;
                        ClimbWall(hit.normal, transform.up);
                    }
                }
                else if (Physics.Raycast(transform.position, transform.right, out hit, wallDetectDist) ||
                    Physics.Raycast(transform.position, -transform.right, out hit, wallDetectDist) ||
                    Physics.Raycast(transform.position, transform.up, out hit, wallDetectDist)) 
                {
                    if (Vector3.Angle(transform.up, hit.normal) > 60 && (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Floor")))
                    {
                        justHit = true;
                        ClimbWall(hit.normal, transform.forward);
                    }
                }
                else if (Physics.Raycast(transform.position, -transform.forward, out hit, wallDetectDist + 0.1f))
                {
                    if (Vector3.Angle(transform.up, hit.normal) > 60 && (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Floor")))
                    {
                        justHit = true;
                        ClimbWall(hit.normal, -transform.up);
                    }
                }
                else
                {
                    justHit = false;
                    justClimbed = false;
                }
            }
            transform.Rotate(new Vector3(0, look.ReadValue<Vector2>().x * lookSpeed, 0));
            if (tongue.GetHitWall() || !isGrounded)
            {
                rb.velocity += transform.forward * moveInput.y + transform.right * moveInput.x;
            }
            else
            {
                rb.velocity = transform.forward * moveInput.y + transform.right * moveInput.x;
            }
        }
        if (Physics.Raycast(transform.position, -transform.up, out hit, wallDetectDist) && (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Floor")))
        {
            gravityDir = -hit.normal.normalized;
            if (hit.distance < wallDetectDist/5)
            {
                moveSpeed = 2f;
            }
        }
        else if (Physics.SphereCast(transform.position, 2, -transform.up, out hit) && (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Floor")))
        {
            gravityDir = -hit.normal.normalized;
            moveSpeed = 0.1f;
        }
        else if (!tongue.GetHitWall())
        {
            gravityDir = Vector3.down;
            moveSpeed = 2f;
        }
        rb.AddForce(gravityForce * gravityDir, ForceMode.Acceleration);
    }

    public void StopMove()
    {
        canMove = false;
    }

    private IEnumerator RotateAnim(Quaternion initRotation, Quaternion endRotation)
    {
        float prevMoveSpeed = moveSpeed;
        moveSpeed = 0.5f;
        float rotationProgress = 0;
        while (rotationProgress < 1)
        {
            rb.velocity *= 0.1f;
            rotationProgress += Time.fixedDeltaTime * 2;
            rb.MoveRotation(Quaternion.Lerp(initRotation, endRotation, rotationProgress));
            yield return new WaitForFixedUpdate();
        }
        moveSpeed = prevMoveSpeed;
        rotateAnim = null;
    }

    private Vector3 SearchAxis(Vector3 v)
    {
        // Search current rotation axis
        Vector3 axis = Vector3.up;
        float minAngle = Vector3.Angle(v, axis);
        foreach(Vector3 u in allAxis)
        {
            if (Vector3.Angle(v, u) < minAngle)
            {
                axis = u;
                minAngle = Vector3.Angle(v, u);
            }
        }
        return axis;
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

    private void OnCollisionEnter(Collision collision)
    {
        justHit = true;
    }

    private void OnCollisionStay(Collision collision)
    {
        justHit = false;
    }
}
