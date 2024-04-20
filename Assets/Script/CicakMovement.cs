using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CicakMovement : MonoBehaviour
{
    PInput pInput;
    InputAction move, tongueAction, tailInput, look, rightClick;
    Rigidbody rb;
    Ray frontRay, backRay, upRay, downRay;
    const float lookSpeed = 1f, gravityForce = 9.81f, wallDetectDist = 0.1f, scaleSpeed = 0.5f;
    float shootLength = 40, moveSpeed = 2f, tailCooldown = 15f;
    bool justClimbed, hasTail, isGrounded;
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
        justClimbed = false;
        hasTail = true;
        isGrounded = true;
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
        if (Physics.Raycast(ray, out RaycastHit hit, shootLength))
        {
            shootPos = hit.point;
        }
        else
        {
            shootPos = ray.origin + ray.direction * shootLength;
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
        downRay = new Ray(transform.position, transform.up * -1);
        bool justHit = false;
        RaycastHit hit;
        if (Physics.Raycast(frontRay, out hit, wallDetectDist))
        {
            if (hit.transform.CompareTag("Wall") || hit.transform.CompareTag("Floor"))
            {
                justHit = true;
                ClimbWall(hit.normal, transform.up);
            }
        }
        else if (Physics.Raycast(backRay, out hit, wallDetectDist + 0.02f))
        {
            if (hit.transform.CompareTag("Wall") || hit.transform.CompareTag("Floor"))
            {
                justHit = true;
                ClimbWall(hit.normal, transform.up * -1);
            }
        }
        else if (Physics.Raycast(upRay, out hit, wallDetectDist))
        {
            if (hit.transform.CompareTag("Wall") || hit.transform.CompareTag("Floor"))
            {
                justHit = true;
                ClimbWall(hit.normal, transform.forward);
            }
        } else
        {
            justClimbed = false;
        }

        if (!Physics.Raycast(downRay, out hit, wallDetectDist)) // Doesn't hit ground
        {
            if (transform.eulerAngles.x != 0 && !tongue.GetHitWall())
            {
                ClimbWall(Vector3.up, transform.up * -1);
            }
            else
            {
                justClimbed = false;
            }
            isGrounded = false;
            moveSpeed = 1f;
        } else
        {
            isGrounded = true;
            moveSpeed = 2f;
            justClimbed = false;
        } 
        if (justHit)
        {
            if (tongueFire != null && tongue.GetHitWall())
            {
                StopCoroutine(tongueFire);
                StartCoroutine(tongue.RetractTongue());
            }
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
}
