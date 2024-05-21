using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CicakHealth : MonoBehaviour
{
    private int lives;
    public UnityEvent levelFinished, death;
    private bool isInvulnerable;
    private const float InvulnerabilityPeriod = 5f; // Invulnerability in seconds
    [SerializeField] private Material cicakMaterial;
    CicakMovement cm;
    private Color[] invulColors;

    private void Awake()
    {
        cm = GetComponent<CicakMovement>();

        isInvulnerable = false;
        lives = 3;
        invulColors = new Color[2];
        invulColors[0] = cicakMaterial.color;
        invulColors[1] = Color.red;
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
        cicakMaterial.color = invulColors[1];
        while (timer < InvulnerabilityPeriod)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        cicakMaterial.color = invulColors[0];
        isInvulnerable = false;
    }

    private IEnumerator Death()
    {
        cm.StopMove();
        Camera.main.transform.parent = null;
        while (transform.localScale.y > 0)
        {
            transform.localScale -= new Vector3(0, 0.005f * Time.deltaTime, 0);
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
