using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public bool checkPlayer, checkTongue, isActive;

    public void TriggerEvent()
    {
        if (isActive)
        {
            EventManagers.InvokeEvent("Tutorial");
            Destroy(this);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (checkPlayer && collision.gameObject.CompareTag("Player"))
        {
            TriggerEvent();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (checkTongue && other.gameObject.CompareTag("Tongue"))
        {
            TriggerEvent();
        }
    }
}
