using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WalkAnim : MonoBehaviour
{
    [SerializeField] List<Transform> legEnds, targets;
    //[SerializeField] Transform tailTarget;
    //[SerializeField] List<Transform> raySrcs, targetAreaPointer;
    List<Tuple<float, bool>> legTimers;
    //List<Vector3> initPosition;
    //List<Quaternion> initRotation;
    //Vector3 initTailPos;
    float timer;
    //const float moveTargetMultiplier = 5f;

    private void Awake()
    {
        //initTailPos = tailTarget.localPosition;
        //targets = new List<Transform>(); // initPosition = new List<Vector3>(); initRotation = new List<Quaternion>();
        legTimers = new List<Tuple<float, bool>>
        {
            new Tuple<float, bool>(0f, false), // Front left
            new Tuple<float, bool>(0.1f, false), // Front right
            new Tuple<float, bool>(0.1f, false), // Back left
            new Tuple<float, bool>(0f, false) // Back right
        };
        timer = 0;
        //StartCoroutine(WagTail());
    }

    //private void OnEnable()
    //{
    //    InputHandler.inputs.Player.Move.performed += MoveTargets;
    //}

    //private void OnDisable()
    //{
    //    InputHandler.inputs.Player.Move.performed -= MoveTargets;
    //}

    //void MoveTargets(InputAction.CallbackContext ctx)
    //{
    //    for (int i = 0; i < targets.Count; i++)
    //    {
    //        Vector2 readVal = ctx.ReadValue<Vector2>();
    //        float multiplier = 8;
    //        if (readVal.y != 0)
    //        {
    //            multiplier *= readVal.y;
    //        }
    //        targetAreaPointer[i].localPosition += Vector3.forward * multiplier;
    //    }
    //}

    //void ResetTargets(InputAction.CallbackContext ctx)
    //{
    //    for (int i = 0; i < targets.Count; i++)
    //    {
    //        targets[i].localPosition = initPosition[i];
    //        targets[i].localRotation = initRotation[i];
    //    }
    //}

    //private IEnumerator WagTail()
    //{
    //    float tailTimer = 0;
    //    while (true)
    //    {
    //        while(tailTimer < 1)
    //        {
    //            tailTarget.localPosition = initTailPos + tailTimer * Vector3.right;
    //            tailTimer += Time.deltaTime;
    //            yield return null;
    //        }
    //        while (tailTimer > -1)
    //        {
    //            tailTarget.localPosition = initTailPos + tailTimer * Vector3.right;
    //            tailTimer -= Time.deltaTime;
    //            yield return null;
    //        }
    //    }
    //}

    void Update()
    {
        RaycastHit hit;
        timer += Time.deltaTime;
        if (timer >= 0.2f)
        {
            timer = 0;
            for (int i = 0; i < legTimers.Count; i++)
            {
                legTimers[i] = new Tuple<float, bool>(legTimers[i].Item1, false);
            }
        }
        for (int i=0; i<legTimers.Count; i++)
        {
            if (timer >= legTimers[i].Item1)
            {
                if (!legTimers[i].Item2)
                {
                    legTimers[i] = new Tuple<float, bool>(legTimers[i].Item1, true);
                    if (Physics.Raycast(legEnds[i].position, -transform.up, out hit, 0.1f))
                    {
                        targets[i].position = hit.point;
                    }
                }
            }
        }
    }
}
