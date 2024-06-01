using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private Listener addObjListener;
    private float objectCount;
    private Animator animator;
    [SerializeField] private TMP_Text objCounterText;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        addObjListener = new Listener();
        addObjListener.invoke = AddObject;
        EventManagers.Register("CollectObj", addObjListener);

        objectCount = 0;
    }

    private void OnDestroy()
    {
        EventManagers.Unregister("CollectObj", addObjListener);
    }

    private void AddObject()
    {
        objectCount++;
        objCounterText.text = objectCount.ToString();
        animator.SetTrigger("Collect");
    }
}
