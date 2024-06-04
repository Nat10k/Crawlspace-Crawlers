using UnityEngine;

public class EmakModel : MonoBehaviour
{
    Emak parent;
    private void Awake()
    {
        parent = transform.parent.GetComponent<Emak>();
    }

    public void ThrowSandal()
    {
        parent.ThrowSandal();
    }
}
