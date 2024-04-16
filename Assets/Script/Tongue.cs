using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tongue : MonoBehaviour
{
    bool hitWall, hitObject, startedAnim;
    LineRenderer line;
    InputAction tongueAction;
    PInput pInput;
    [SerializeField] Transform cicak;
    CicakMovement cicakMove;
    Transform heldObj;
    Rigidbody cicakRB;
    float maxTongueLength = 2;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        cicakRB = cicak.GetComponent<Rigidbody>();
        cicakMove = cicak.GetComponent<CicakMovement>();
        pInput = new PInput();
        startedAnim = false;
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
            if (Vector3.Distance(cicak.position, tongueEndPos) > maxTongueLength)
            {
                // Kalau lidah terlalu panjang, tarik kembali
                StartCoroutine(RetractTongue());
                yield break;
            }
            if (Vector3.Distance(tongueEndPos,dest) > 0.01f && !hitWall && !hitObject)
            {
                line.SetPosition(1, Vector3.MoveTowards(tongueEndPos, dest, 10 * Time.deltaTime));
            } else
            {
                if (hitWall)
                {
                    cicakRB.velocity = (dest - cicak.position).normalized * 2;
                    if (!startedAnim)
                    {
                        Vector3 dir = (dest - cicak.position).normalized;
                        Quaternion newRot = Quaternion.LookRotation(dir, cicak.up);
                        cicakMove.StartRotateAnim(cicakRB.rotation, newRot);
                        startedAnim = true;
                    }
                } else if (hitObject)
                {
                    line.SetPosition(1, heldObj.position);
                }
            }
            transform.position = line.GetPosition(1);
            yield return null;
        }
    }

    public IEnumerator RetractTongue()
    {
        while(line.GetPosition(1) != line.GetPosition(0))
        {
            line.SetPosition(0, cicak.position);
            line.SetPosition(1, Vector3.MoveTowards(line.GetPosition(1), cicak.position, 10 * Time.deltaTime));
            transform.position = line.GetPosition(1);
            yield return null;
        }
        gameObject.SetActive(false);
        enabled = false;
        startedAnim = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Floor")) && !hitObject)
        {
            hitWall = true;
        } else if (other.gameObject.CompareTag("Movable") && !hitWall)
        {
            heldObj = other.transform;
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

    public Transform GetCicak()
    {
        return cicak;
    }
}
