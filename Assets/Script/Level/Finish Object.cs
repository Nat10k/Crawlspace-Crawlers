using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishObject : MonoBehaviour
{
    Outline outline;
    ParticleSystem particleSystem;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        particleSystem = GetComponentInChildren<ParticleSystem>();

        outline.enabled = false;
        particleSystem.enableEmission = false;
    }
    
    public void TurnOnFinish()
    {
        gameObject.tag = "Finish";
        outline.enabled = true;
        particleSystem.enableEmission = true;
    }
}
