using System.Collections;
using UnityEngine;

public class Tongue : MonoBehaviour
{
    bool hitWall, hitObject, startedAnim;
    LineRenderer line;
    [SerializeField] Transform cicak, headPos;
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
                    cicakRB.constraints = RigidbodyConstraints.FreezeRotation;
                    line.SetPosition(1, heldObj.TransformPoint(hitPos));
                }
            }
            transform.position = line.GetPosition(1);
            yield return null;
        }
    }

    public IEnumerator RetractTongue()
    {
        cicakRB.constraints = RigidbodyConstraints.None;
        while (line.GetPosition(1) != line.GetPosition(0))
        {
            line.SetPosition(0, headPos.position);
            line.SetPosition(1, Vector3.MoveTowards(line.GetPosition(1), headPos.position, 10 * Time.deltaTime));
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
