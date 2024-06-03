using System.Collections;
using UnityEngine;

public class Sandal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroySelf());
    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }
}
