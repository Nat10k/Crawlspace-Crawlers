using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CicakHealth : MonoBehaviour
{
    private int lives;
    public UnityEvent levelFinished, death;
    private bool isInvulnerable;
    private const float InvulnerabilityPeriod = 3f; // Invulnerability in seconds
    [SerializeField] private Material cicakMaterial;
    [SerializeField] private Transform model;
    CicakMovement cm;
    [SerializeField] private Color[] invulColors;

    private void Awake()
    {
        cm = GetComponent<CicakMovement>();
        cicakMaterial.color = invulColors[0];
        isInvulnerable = false;
        lives = 3;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && !isInvulnerable)
        {
            lives--;
            if (lives <= 0)
            {
                StartCoroutine(Death());
                return;
            }
            isInvulnerable = true;
            StartCoroutine(Invulnerability());
        }
    }

    private IEnumerator Invulnerability()
    {
        float timer = 0;
        int colorIdx = 1;
        for (int i=0; i<6; i++)
        {
            cicakMaterial.color = invulColors[colorIdx];
            colorIdx++;
            colorIdx %= invulColors.Length;
            yield return new WaitForSeconds(InvulnerabilityPeriod/6);
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
        death.Invoke();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            levelFinished.Invoke();
        }
    }
}
