using UnityEngine;
using UnityEngine.Events;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent assignedEvent;
    public bool checkPlayer, checkTongue;

    public void TriggerEvent()
    {
        assignedEvent.Invoke();
        Destroy(this);
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
