using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tongue : MonoBehaviour
{
    bool hitWall, hitObject;
    LineRenderer line;
    CapsuleCollider cc;
    InputAction tongueAction;
    PInput pInput;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        cc = GetComponent<CapsuleCollider>();
        cc.radius = line.startWidth;
        pInput = new PInput();
    }

    private void OnEnable()
    {
        tongueAction = pInput.Player.Fire;
        tongueAction.Enable();
    }

    private void OnDisable()
    {
        hitWall = false;
        hitObject = false;
        tongueAction.Disable();
        ResetTongue();
    }

    public IEnumerator ShootTongue(Vector3 dest)
    {
        line.SetPosition(0, transform.parent.position);
        line.SetPosition(1, transform.parent.position);
        while (true)
        {
            line.SetPosition(0, transform.parent.position);
            Vector3 tongueEndPos = line.GetPosition(1);
            if (Vector3.Distance(tongueEndPos,dest) > 0.01f)
            {
                line.SetPosition(1, Vector3.MoveTowards(tongueEndPos,dest,10*Time.deltaTime));
                transform.LookAt(tongueEndPos);
                cc.transform.localPosition = tongueEndPos / 2;
                cc.height = Vector3.Distance(transform.parent.position, tongueEndPos);
            }
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Floor"))
        {
            hitWall = true;
        } else if (other.gameObject.CompareTag("Movable"))
        {
            hitObject = true;
        }
    }

    public bool GetHitWall()
    {
        return hitWall;
    }

    public bool GetHitObject()
    {
        return hitObject;
    }

    public void ResetTongue()
    {
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        cc.height = 0;
    }
}
