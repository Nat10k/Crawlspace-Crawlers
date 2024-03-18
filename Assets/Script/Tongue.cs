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
    [SerializeField] Transform cicak;
    Rigidbody cicakRB;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        cc = GetComponent<CapsuleCollider>();
        cicakRB = cicak.GetComponent<Rigidbody>();
        pInput = new PInput();
    }

    private void OnEnable()
    {
        cc.enabled = true;
        tongueAction = pInput.Player.Fire;
        tongueAction.Enable();
    }

    private void OnDisable()
    {
        cc.enabled = false;
        hitWall = false;
        hitObject = false;
        tongueAction.Disable();
    }

    public IEnumerator ShootTongue(Vector3 dest)
    {
        line.SetPosition(0, cicak.position);
        line.SetPosition(1, cicak.position);
        transform.position = cicak.position;
        while (true)
        {
            line.SetPosition(0, cicak.position);
            Vector3 tongueEndPos = line.GetPosition(1);
            if (Vector3.Distance(tongueEndPos,dest) > 0.01f)
            {
                line.SetPosition(1, Vector3.MoveTowards(tongueEndPos, dest, 10 * Time.deltaTime));
                transform.position = line.GetPosition(1);
            } else
            {
                cicakRB.velocity = (dest-cicak.position).normalized * 2;
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
}
