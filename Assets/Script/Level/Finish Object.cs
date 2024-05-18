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
        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        while (true)
        {
            while (outline.OutlineColor.a > 0)
            {
                outline.OutlineColor -= new Color(0,0,0, Time.deltaTime);
                yield return null;
            }
            while (outline.OutlineColor.a < 1)
            {
                outline.OutlineColor += new Color(0, 0, 0, Time.deltaTime);
                yield return null;
            }
        }
    }
}
