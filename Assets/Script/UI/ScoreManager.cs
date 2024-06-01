using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private Listener addObjListener;
    private float objectCount;
    [SerializeField] private TMP_Text objCounterText;

    private void Awake()
    {
        addObjListener = new Listener();
        addObjListener.invoke = AddObject;
        EventManagers.Register("AddObject", addObjListener);

        objectCount = 0;
    }

    private void OnDestroy()
    {
        EventManagers.Unregister("AddObject", addObjListener);
    }

    private void AddObject()
    {
        objectCount++;
        objCounterText.text = objectCount.ToString();
    }
}
