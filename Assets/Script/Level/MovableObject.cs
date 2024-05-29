using System.Collections;
using System.Collections.Generic;
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
            gameObject.layer = 2;
            tongueScript = other.gameObject.GetComponent<Tongue>();
            if (!tongueScript.GetHitWall())
            {
                joint = this.AddComponent<SpringJoint>();
                Transform cicak = tongueScript.GetCicak();
                joint.connectedBody = cicak.GetComponent<Rigidbody>();
                joint.spring = 30;
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
        if (other.gameObject.CompareTag("Tongue"))
        {
            gameObject.layer = 1;
            Destroy(joint);
        }
    }
}
