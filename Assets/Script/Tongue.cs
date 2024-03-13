using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tongue : MonoBehaviour
{
    bool hitWall;
    Vector3 initSize;

    private void Awake()
    {
        initSize = transform.localScale;
    }

    private void OnDisable()
    {
        hitWall = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Floor"))
        {
            hitWall = true;
        }
    }

    public bool GetHitWall()
    {
        return hitWall;
    }

    public void ResetTongue()
    {
        transform.localScale = initSize;
        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector3.zero;
    }
}
