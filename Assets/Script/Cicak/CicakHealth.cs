using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CicakHealth : MonoBehaviour
{
    private int lives;
    private bool isInvulnerable;
    private const float InvulnerabilityPeriod = 3f, joltSpeed = 7f; // Invulnerability in seconds
    [SerializeField] private Material cicakMaterial;
    [SerializeField] private Transform model;
    CicakMovement cm;
    [SerializeField] private Color[] invulColors;
    Rigidbody rb;

    private void Awake()
    {
        cm = GetComponent<CicakMovement>();
        rb = GetComponent<Rigidbody>();
        cicakMaterial.color = invulColors[0];
        isInvulnerable = false;
        lives = 3;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && !isInvulnerable)
        {
            Damaged();
        }
    }

    private void Damaged()
    {
        lives--;
        EventManagers.InvokeEvent("Damaged");
        if (lives <= 0)
        {
            StartCoroutine(Death());
            return;
        }
        isInvulnerable = true;
        // Jolt when damaged
        cm.DisableMove();
        rb.velocity += (Vector3.up + Random.Range(-1, 1) * Vector3.right + Random.Range(-1, 1) * Vector3.forward) * joltSpeed;
        StartCoroutine(Invulnerability());
    }

    private IEnumerator Invulnerability()
    {
        int colorIdx = 1;
        for (int i=0; i<6; i++)
        {
            cicakMaterial.color = invulColors[colorIdx];
            colorIdx++;
            colorIdx %= invulColors.Length;
            yield return new WaitForSeconds(InvulnerabilityPeriod/6);
            cm.EnableMove();
        }
        cicakMaterial.color = invulColors[0];
        isInvulnerable = false;
    }

    private IEnumerator Death()
    {
        cm.StopMove();
        Camera.main.transform.parent = null;
        while (model.localScale.y > 0)
        {
            model.localScale -= new Vector3(0, Time.deltaTime, 0);
            yield return null;
        }
        EventManagers.InvokeEvent("GameOver");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            collision.gameObject.GetComponent<TargetObject>().Collect();
            EventManagers.InvokeEvent("CollectObj");
        } else if (collision.collider.CompareTag("Enemy") && !isInvulnerable)
        {
            Damaged();
        }
    }
}
