using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartManager : MonoBehaviour
{
    [SerializeField] List<Image> hearts;
    [SerializeField] Sprite damagedGif, damagedStill, fullStill, fullGif;
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
        foreach (var heart in hearts)
        {
            if (heart.sprite == damagedStill)
            {
                heart.sprite = damagedGif;
            } else
            {
                heart.sprite = fullGif;
            }
        }
        yield return new WaitForSeconds(InvulnerabilityPeriod);
        foreach (var heart in hearts)
        {
            if (heart.sprite == damagedGif)
            {
                heart.sprite = damagedStill;
            }
            else
            {
                heart.sprite = fullStill;
            }
        }
        currHealthyIdx++;
    }
}
