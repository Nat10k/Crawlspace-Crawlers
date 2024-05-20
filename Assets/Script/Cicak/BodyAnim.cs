using UnityEngine;

public class BodyAnim : MonoBehaviour
{
    [SerializeField] Transform target, bone;
    const float raycastDist = 0.2f;
    Quaternion initRot;
    Vector3 initPos;

    private void Awake()
    {
        initPos = target.localPosition;
        initRot = target.localRotation;
    }

    private void Update()
    {
        Ray frontRay = new(transform.position, transform.forward);
        Ray backRay = new(transform.position, -1 * transform.forward);
        Ray rightRay = new(transform.position, transform.right);
        Ray leftRay = new(transform.position, -1 * transform.right);
        Ray downRay = new(transform.position, -1 * transform.up);
        RaycastHit hit;

        if (Physics.Raycast(frontRay, out hit, raycastDist) || Physics.Raycast(backRay, out hit, raycastDist)
            || Physics.Raycast(rightRay, out hit, raycastDist) || Physics.Raycast(leftRay, out hit, raycastDist))
        {
            target.position = Vector3.MoveTowards(target.position, hit.point, Time.deltaTime);
            target.localPosition += new Vector3(0, (raycastDist - hit.distance) / raycastDist * 10);
        }
        else if (Physics.Raycast(downRay, out hit, raycastDist))
        {
            target.position = hit.point;
        }
        else
        {
            target.localPosition -= new Vector3(0, 10, 0);
        }
    }
}
