using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovableObject : MonoBehaviour
{
    SpringJoint joint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Tongue"))
        {
            Tongue tongueScript = other.gameObject.GetComponent<Tongue>();
            if (!tongueScript.GetHitWall())
            {
                joint = this.AddComponent<SpringJoint>();
                Transform cicak = tongueScript.GetCicak();
                joint.connectedBody = cicak.GetComponent<Rigidbody>();
                joint.spring = 30;
                joint.tolerance = 0;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Tongue"))
        {
            Destroy(joint);
        }
    }
}
