using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tongue : MonoBehaviour
{
    bool hitWall, hitObject, startedAnim;
    LineRenderer line;
    [SerializeField] Transform cicak;
    [SerializeField] CicakCam cicakCam;
    CicakMovement cicakMove;
    Transform heldObj;
    Rigidbody cicakRB;
    Vector3 hitPos;
    float maxTongueLength = 5 ,boostFactor = 2;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        cicakRB = cicak.GetComponent<Rigidbody>();
        cicakMove = cicak.GetComponent<CicakMovement>();
        startedAnim = false;
    }

    private void OnDisable()
    {
        hitWall = false;
        hitObject = false;
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
                    cicakRB.velocity = (hitPos - cicak.position).normalized * boostFactor;
                    if (!startedAnim)
                    {
                        Vector3 dir = (hitPos - cicak.position).normalized;
                        dir = Vector3.ProjectOnPlane(dir, cicak.up);
                        Quaternion newRot = Quaternion.LookRotation(dir, cicak.up);
                        cicakMove.StartRotateAnim(cicakRB.rotation, newRot);
                        startedAnim = true;
                    }
                } else if (hitObject)
                {
                    line.SetPosition(1, heldObj.TransformPoint(hitPos));
                }
            }
            transform.position = line.GetPosition(1);
            yield return null;
        }
    }

    public IEnumerator RetractTongue()
    {
        while (line.GetPosition(1) != line.GetPosition(0))
        {
            line.SetPosition(0, cicak.position);
            line.SetPosition(1, Vector3.MoveTowards(line.GetPosition(1), cicak.position, 10 * Time.deltaTime));
            transform.position = line.GetPosition(1);
            cicakCam.ResetFOV();
            yield return null;
        }
        boostFactor = 2;
        gameObject.SetActive(false);
        enabled = false;
        startedAnim = false;
    }

    IEnumerator TongueBoost()
    {
        yield return new WaitForSeconds(0.2f);
        boostFactor *= 2;
        cicakCam.BoostCam();
    }

    private void OnTriggerEnter(Collider other)
    {
        hitPos = other.ClosestPoint(line.GetPosition(1));
        if ((other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Floor")) && !hitObject)
        {
            hitWall = true;
            StartCoroutine(TongueBoost());
        } else if (other.gameObject.CompareTag("Movable") && !hitWall)
        {
            heldObj = other.transform;
            hitPos = heldObj.InverseTransformPoint(hitPos);
            line.SetPosition(1, heldObj.TransformPoint(hitPos));
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
