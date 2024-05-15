using UnityEngine;
using UnityEngine.Events;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent assignedEvent;
    [SerializeField] TutorialTrigger nextTrigger;
    public bool checkPlayer, checkTongue, isActive;

    public void TriggerEvent()
    {
        if (isActive)
        {
            assignedEvent.Invoke();
            if (nextTrigger != null)
            {
                nextTrigger.isActive = true;
            }
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
