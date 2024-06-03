using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CicakMovement : MonoBehaviour
{
    InputAction move, tongueAction, tailInput, look, rightClick, reset;
    Rigidbody rb;
    const float lookSpeed = 1f, gravityForce = 15f, wallDetectDist = 0.2f, baseMoveSpeed = 2f, climbAngle = 50f;
    float moveSpeed = 2f, tailCooldown = 25f, scaleSpeed;
    bool justClimbed, hasTail, isGrounded, isNearSurface, justHit, justTongue;
    [HideInInspector] public bool canMove;
    [SerializeField] Tongue tongue;
    [SerializeField] Transform tailObj, tailParent, headBone, spine;
    [SerializeField] Material cicakMaterial;
    [SerializeField] TutorialTrigger trigger, tailTrigger;
    CicakCam cicakCam;
    Coroutine tongueFire, rotateAnim, tailScaleAnim;
    Vector3 gravityDir, initTailPos, initTailScale;
    readonly Vector3[] allAxis = { Vector3.forward, Vector3.back, Vector3.right, Vector3.left, Vector3.up, Vector3.down };
    Quaternion initTailRot;
    Listener stopMoveListener, resumeMoveListener;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cicakCam = Camera.main.GetComponent<CicakCam>();

        gravityDir = transform.up * -1;
        justClimbed = false;
        justTongue = false;
        justHit = false;
        hasTail = true;
        isGrounded = true;
        isNearSurface = true;
        canMove = true;

        initTailPos = tailObj.localPosition;
        initTailScale = tailObj.localScale;
        initTailRot = tailObj.localRotation;
        tailParent = tailObj.parent;

        scaleSpeed = initTailScale.magnitude / 2;

        stopMoveListener = new Listener();
        resumeMoveListener = new Listener();
        stopMoveListener.invoke = DisableMove;
        resumeMoveListener.invoke = EnableMove;
        EventManagers.Register("StopMove", stopMoveListener);
        EventManagers.Register("ResumeMove", resumeMoveListener);
    }

    private void OnEnable()
    {
        move = InputHandler.inputs.Player.Move;
        tongueAction = InputHandler.inputs.Player.Fire;
        look = InputHandler.inputs.Player.Look;
        rightClick = InputHandler.inputs.Player.RightClick;
        tailInput = InputHandler.inputs.Player.Tail;
        reset = InputHandler.inputs.Player.Reset;

        rightClick.performed += LockCursor;
        rightClick.canceled += ReleaseCursor;
        tongueAction.performed += ShootTongue;
        tongueAction.canceled += ReleaseTongue;
        tailInput.performed += SeparateTail;
        reset.performed += ResetRotation;
        move.Enable();
        tongueAction.Enable();
        look.Enable();
        rightClick.Enable();
        tailInput.Enable();
        reset.Enable();
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

    private void OnDestroy()
    {
        EventManagers.Unregister("StopMove", stopMoveListener);
        EventManagers.Unregister("ResumeMove", resumeMoveListener);
    }

    private void SeparateTail(InputAction.CallbackContext ctx)
    {
        if (hasTail)
        {
            if (tailTrigger != null)
            {
                tailTrigger.TriggerEvent();
                tailTrigger = null;
            }
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

    public void DisableMove()
    {
        canMove = false;
    }

    public void EnableMove()
    {
        canMove = true;
    }

    private void ResetRotation(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            ClimbWall(Vector3.up, Vector3.forward);
        }
    }

    IEnumerator TailBoost()
    {
        // Speed boost for 5 seconds after detaching tail
        cicakCam.BoostCam();
        moveSpeed *= 2;
        yield return new WaitForSeconds(5);
        cicakCam.ResetFOV();
        moveSpeed /= 2;
        tailScaleAnim = StartCoroutine(ScaleTail(Vector3.zero));
        yield return new WaitForSeconds(tailCooldown - 2);
        // Reattach tail after cooldown
        tailScaleAnim = StartCoroutine(ScaleTail(initTailScale));
        tailObj.parent = tailParent;
        tailObj.SetLocalPositionAndRotation(initTailPos, initTailRot);
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
        Vector2 moveInput = move.ReadValue<Vector2>() * moveSpeed;
        if (canMove)
        {
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
                    justTongue = true;
                }
            }

            if (!tongue.GetHitWall())
            {
                // Climb forward
                if (Physics.Raycast(transform.position, transform.forward, out hit, wallDetectDist) && (moveInput.y > 0 || justTongue))
                {
                    if (Vector3.Angle(transform.up, hit.normal) > climbAngle && (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Floor")))
                    {
                        justHit = true;
                        ClimbWall(hit.normal, transform.up);
                    }
                }
                else if ((Physics.Raycast(transform.position, transform.right, out hit, wallDetectDist) && (moveInput.x > 0 || justTongue)) ||
                    (Physics.Raycast(transform.position, -transform.right, out hit, wallDetectDist) && (moveInput.x < 0 || justTongue))) 
                {
                    if (Vector3.Angle(transform.up, hit.normal) > climbAngle && (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Floor")))
                    {
                        justHit = true;
                        ClimbWall(hit.normal, transform.forward);
                    }
                }
                else if (Physics.Raycast(transform.position, -transform.forward, out hit, wallDetectDist + 0.1f)
                    && (moveInput.y < 0 || justTongue))
                {
                    if (Vector3.Angle(transform.up, hit.normal) > climbAngle && (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Floor")))
                    {
                        justHit = true;
                        ClimbWall(hit.normal, -transform.up);
                    }
                } else if (Physics.Raycast(transform.position, transform.up, out hit, wallDetectDist) && justTongue)
                {
                    justHit = true;
                    ClimbWall(hit.normal, transform.forward);
                }
                else
                {
                    justHit = false;
                    justTongue = false;
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
        if (Physics.Raycast(transform.position, -transform.up, out hit, wallDetectDist) && 
            (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Floor")))
        {
            gravityDir = -hit.normal.normalized;
            isGrounded = true;
            if (moveSpeed < baseMoveSpeed && hit.distance < wallDetectDist/5)
            {
                moveSpeed = baseMoveSpeed;
            }
        }
        else
        {
            moveSpeed = 0.17f;
            if (isGrounded)
            {
                if (moveInput.y != 0)
                {
                    ClimbWall(transform.forward * moveInput.y, -transform.up * moveInput.y);
                }
                else if (moveInput.x != 0)
                {
                    ClimbWall(transform.right * moveInput.x, transform.forward);
                }
            }
            else if (!isNearSurface)
            {
                gravityDir = Vector3.down;
            }
            isGrounded = !Physics.SphereCast(transform.position, 3, -transform.up, out hit, wallDetectDist + 2);
        }
        //else if (Physics.SphereCast(transform.position, 3, -transform.up, out hit, 10) 
        //    && (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Floor")))
        //{
        //    gravi
        //    moveSpeed = 0.1f;
        //}
        //else if (!tongue.GetHitWall())
        //{
        //    gravityDir = Vector3.down;
        //    moveSpeed = 0.3f;
        //}
        rb.AddForce(gravityForce * gravityDir, ForceMode.Acceleration);
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

    //private Vector3 SearchAxis(Vector3 v)
    //{
    //    // Search current rotation axis
    //    Vector3 axis = Vector3.up;
    //    float minAngle = Vector3.Angle(v, axis);
    //    foreach(Vector3 u in allAxis)
    //    {
    //        if (Vector3.Angle(v, u) < minAngle)
    //        {
    //            axis = u;
    //            minAngle = Vector3.Angle(v, u);
    //        }
    //    }
    //    return axis;
    //}

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

    private void OnTriggerStay(Collider other)
    {
        isNearSurface = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isNearSurface = false;
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
