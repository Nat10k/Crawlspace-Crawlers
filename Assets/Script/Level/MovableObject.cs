using Unity.VisualScripting;
using UnityEngine;

public class MovableObject : MonoBehaviour
{
    SpringJoint joint;
    Tongue tongueScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Tongue"))
        {
            if (joint != null)
            {
                return;
            }
            gameObject.layer = 2;
            tongueScript = other.gameObject.GetComponent<Tongue>();
            if (!tongueScript.GetHitWall())
            {
                joint = this.AddComponent<SpringJoint>();
                Transform cicak = tongueScript.GetCicak();
                joint.connectedBody = cicak.GetComponent<Rigidbody>();
                joint.spring = 60;
                joint.tolerance = 0;
                joint.connectedMassScale = 0.0001f;
            }
        }
    }

    private void Update()
    {
        if (tongueScript != null && !tongueScript.GetHitObject())
        {
            if (joint != null)
            {
                Destroy(joint);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        gameObject.layer = 3;
        if (other.gameObject.CompareTag("Tongue"))
        {
            Destroy(joint);
        }
    }
}
