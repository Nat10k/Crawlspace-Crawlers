using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Tongue : MonoBehaviour
{
    bool hitWall, hitObject, startedAnim;
    LineRenderer line;
    [SerializeField] Transform cicak, headPos;
    [SerializeField] CicakCam cicakCam;
    [SerializeField] Image cursor;
    CicakMovement cicakMove;
    Transform heldObj;
    Rigidbody cicakRB;
    Vector3 hitPos;
    Ray tongueRay;
    RaycastHit hit;
    bool hitSomething, isShooting;
    float maxTongueLength = 5 ,boostFactor = 2;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        cicakRB = cicak.GetComponent<Rigidbody>();
        cicakMove = cicak.GetComponent<CicakMovement>();
        hitWall = false;
        hitObject = false;
        startedAnim = false;
        isShooting = false;
    }

    private void Update()
    {
        tongueRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(tongueRay, out hit) && (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Movable")
            || hit.collider.CompareTag("Finish")) && Vector3.Distance(hit.point, headPos.position) < maxTongueLength)
        {
            hitSomething = true;
            cursor.color = Color.green;
        } else
        {
            hitSomething = false;
            cursor.color = Color.white;
        }
    }

    public IEnumerator ShootTongue()
    {
        isShooting = true;
        Vector3 dest;
        if (hitSomething)
        {
            dest = hit.point;
        } else
        {
            dest = tongueRay.origin + tongueRay.direction * maxTongueLength;
        }
        line.SetPosition(0, headPos.position);
        line.SetPosition(1, headPos.position);
        transform.position = headPos.position;
        while (true)
        {
            line.SetPosition(0, headPos.position);
            Vector3 tongueEndPos = line.GetPosition(1);
            if (Vector3.Distance(headPos.position, tongueEndPos) > maxTongueLength)
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
                    cicakRB.velocity = (hitPos - headPos.position).normalized * boostFactor;
                    if (!startedAnim)
                    {
                        Vector3 dir = (hitPos - headPos.position).normalized;
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
        isShooting = false;
        hitWall = false;
        hitObject = false;
        cicakCam.ResetFOV();
        while (line.GetPosition(1) != line.GetPosition(0))
        {
            line.SetPosition(0, headPos.position);
            line.SetPosition(1, Vector3.MoveTowards(line.GetPosition(1), headPos.position, 10 * Time.deltaTime));
            transform.position = line.GetPosition(1);
            yield return null;
        }
        boostFactor = 2;
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
        if (isShooting)
        {
            hitPos = other.ClosestPoint(line.GetPosition(1));
            if ((other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Floor")) && !hitObject)
            {
                hitWall = true;
                StartCoroutine(TongueBoost());
            }
            else if ((other.gameObject.CompareTag("Movable") || other.gameObject.CompareTag("Finish")) && !hitWall)
            {
                heldObj = other.transform;
                hitPos = heldObj.InverseTransformPoint(hitPos);
                line.SetPosition(1, heldObj.TransformPoint(hitPos));
                hitObject = true;
            }
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
