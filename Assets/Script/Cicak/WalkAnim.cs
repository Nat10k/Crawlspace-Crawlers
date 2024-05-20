using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class WalkAnim : MonoBehaviour
{
    [SerializeField] List<TwoBoneIKConstraint> leftConstraint, rightConstraint;
    [SerializeField] Transform tailTarget;
    [SerializeField] List<Transform> raySrcs;
    List<Transform> targets;
    List<Vector3> initPosition;
    List<Quaternion> initRotation;
    Vector3 initTailPos;
    float timer;

    private void Awake()
    {
        initTailPos = tailTarget.localPosition;
        targets = new List<Transform>(); initPosition = new List<Vector3>(); initRotation = new List<Quaternion>();
        foreach (TwoBoneIKConstraint c in leftConstraint)
        {
            targets.Add(c.data.target);
            initPosition.Add(c.data.target.localPosition);
            initRotation.Add(c.data.target.localRotation);
        }
        foreach (TwoBoneIKConstraint c in rightConstraint)
        {
            targets.Add(c.data.target);
            initPosition.Add(c.data.target.localPosition);
            initRotation.Add(c.data.target.localRotation);
        }
        foreach(Transform t in targets)
        {
            Debug.Log(t.gameObject.name);
        }
        timer = 0;
        StartCoroutine(WagTail());
    }

    private void OnEnable()
    {
        InputHandler.inputs.Player.Move.performed += MoveTargets;
        InputHandler.inputs.Player.Move.canceled += ResetTargets;
    }

    private void OnDisable()
    {
        InputHandler.inputs.Player.Move.performed -= MoveTargets;
        InputHandler.inputs.Player.Move.canceled -= ResetTargets;
    }

    void MoveTargets(InputAction.CallbackContext ctx)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            Vector2 readVal = ctx.ReadValue<Vector2>();
            float multiplier = 8;
            if (readVal.y != 0)
            {
                multiplier *= readVal.y;
            }
            targets[i].localPosition += Vector3.forward * multiplier;
        }
    }

    void ResetTargets(InputAction.CallbackContext ctx)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            targets[i].localPosition = initPosition[i];
            targets[i].localRotation = initRotation[i];
        }
    }

    private IEnumerator WagTail()
    {
        float tailTimer = 0;
        while (true)
        {
            while(tailTimer < 1)
            {
                tailTarget.localPosition = initTailPos + tailTimer * Vector3.right;
                tailTimer += Time.deltaTime;
                yield return null;
            }
            while (tailTimer > -1)
            {
                tailTarget.localPosition = initTailPos + tailTimer * Vector3.right;
                tailTimer -= Time.deltaTime;
                yield return null;
            }
        }
    }

    void Update()
    {
        Vector2 moveInput = InputHandler.inputs.Player.Move.ReadValue<Vector2>();
        RaycastHit hit;
        for (int i=0; i<raySrcs.Count; i++)
        {
            if (Physics.Raycast(raySrcs[i].position, raySrcs[i].forward, out hit, 0.06f) ||
                Physics.Raycast(raySrcs[i].position, -raySrcs[i].forward, out hit, 0.06f) ||
                Physics.Raycast(raySrcs[i].position, raySrcs[i].right, out hit, 0.06f) ||
                Physics.Raycast(raySrcs[i].position, -raySrcs[i].right, out hit, 0.06f))
            {
                targets[i].position = hit.point;
                targets[i].right = hit.normal;
            }
        }
        if (moveInput != Vector2.zero)
        {
            if (timer < 1)
            {
                timer += Time.deltaTime;
            } else
            {
                timer = 0;
            }
            if (timer < 0.5f)
            {
                foreach(var lCon in leftConstraint)
                {
                    lCon.data.targetPositionWeight = timer / 0.5f;
                }
                foreach(var rCon in rightConstraint)
                {
                    if (rCon.data.targetPositionWeight > 0)
                    {
                        rCon.data.targetPositionWeight -= Time.deltaTime;
                    }
                }
            } else
            {
                foreach (var lCon in leftConstraint)
                {
                    if (lCon.data.targetPositionWeight > 0)
                    {
                        lCon.data.targetPositionWeight -= Time.deltaTime;
                    }
                }
                foreach (var rCon in rightConstraint)
                {
                    rCon.data.targetPositionWeight = (timer-0.5f)/0.5f;
                }
            }
        }
    }
}
