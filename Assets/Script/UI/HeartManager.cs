using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartManager : MonoBehaviour
{
    [SerializeField] List<Image> hearts;
    [SerializeField] Sprite damagedStill, fullStill;
    Listener damagedListener;
    const float InvulnerabilityPeriod = 3f;
    int currHealthyIdx;

    private void Awake()
    {
        currHealthyIdx = 0;
        damagedListener = new Listener();
        damagedListener.invoke = DamagedHeart;
        EventManagers.Register("Damaged", damagedListener);
    }

    private void OnDestroy()
    {
        EventManagers.Unregister("Damaged", damagedListener);
    }

    private void DamagedHeart()
    {
        StartCoroutine(DamageAnim());
    }

    private IEnumerator DamageAnim()
    {
        hearts[currHealthyIdx].sprite = damagedStill;
        int mult = -1;
        for (int i = 0; i < 6; i++)
        {
            foreach (var heart in hearts)
            {
                heart.color = new Color(heart.color.r, heart.color.g, heart.color.b, heart.color.a + mult * 127.5f);
            }
            mult *= -1;
            yield return new WaitForSeconds(InvulnerabilityPeriod / 6);
        }
        currHealthyIdx++;
    }
}
