using System.Collections;
using UnityEngine;

public class TargetObject : MonoBehaviour
{
    Outline outline;
    ParticleSystem particleSystem;
    Animator animator;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        animator = GetComponent<Animator>();
        particleSystem = GetComponentInChildren<ParticleSystem>();

        outline.enabled = false;
        particleSystem.enableEmission = false;
    }
    
    public void TurnOnTarget()
    {
        gameObject.tag = "Target";
        outline.enabled = true;
        particleSystem.enableEmission = true;
        StartCoroutine(Blink());
    }

    public void Collect()
    {
        gameObject.tag = "Untagged";
        outline.enabled = false;
        particleSystem.enableEmission = false;
        StopAllCoroutines();
        animator.SetTrigger("Collected");
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
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
