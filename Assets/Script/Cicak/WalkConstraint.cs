using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class WalkConstraint : MonoBehaviour
{
    [SerializeField] List<TwoBoneIKConstraint> leftConstraint, rightConstraint;
    List<Transform> targets;
    List<Vector3> initPosition;
    float timer;

    private void Awake()
    {
        targets = new List<Transform>(); initPosition = new List<Vector3>();
        foreach (TwoBoneIKConstraint c in leftConstraint)
        {
            targets.Add(c.data.target);
            initPosition.Add(c.data.target.localPosition);
        }
        foreach (TwoBoneIKConstraint c in rightConstraint)
        {
            targets.Add(c.data.target);
            initPosition.Add(c.data.target.localPosition);
        }
        timer = 0;
    }

    private void OnEnable()
    {
        InputHandler.inputs.Player.Move.performed += MoveTargets;
        //InputHandler.inputs.Player.Move.canceled += ResetTargets;
    }

    private void OnDisable()
    {
        InputHandler.inputs.Player.Move.performed -= MoveTargets;
        //InputHandler.inputs.Player.Move.canceled -= ResetTargets;
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
            targets[i].localPosition = initPosition[i] + Vector3.forward * multiplier;
        }
    }

    //void ResetTargets(InputAction.CallbackContext ctx)
    //{
    //    for (int i = 0; i < targets.Count; i++)
    //    {
    //        targets[i].localPosition = initPosition[i];
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        Vector2 moveInput = InputHandler.inputs.Player.Move.ReadValue<Vector2>();
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
