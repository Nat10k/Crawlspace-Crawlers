using System.Collections;
using UnityEngine;

public class TargetObject : MonoBehaviour
{
    Outline outline;
    ParticleSystem particleSystem;
    Rigidbody rb;
    Animator animator;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        animator = GetComponent<Animator>();
        particleSystem = GetComponentInChildren<ParticleSystem>();
        rb = GetComponent<Rigidbody>();

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

    public void Shrink()
    {
        StartCoroutine(StartShrink());
    }

    private IEnumerator StartShrink()
    {
        float timer = 0;
        float initMagnitude = Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        rb.isKinematic = true;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            transform.Rotate(Vector3.down, 30);
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, initMagnitude/10);
            yield return null;
        }
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
