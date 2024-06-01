using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public bool checkPlayer, checkTongue, isActive, isFinish;

    public void TriggerEvent()
    {
        if (isActive)
        {
            EventManagers.InvokeEvent("Tutorial");
            if (!isFinish)
            {
                Destroy(this);
            } else
            {
                isFinish = false;
                checkTongue = false;
                checkPlayer = true;
            }
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
