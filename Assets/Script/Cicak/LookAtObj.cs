using UnityEngine;
using UnityEngine.InputSystem;

public class LookAtObj : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            transform.position = hit.point;
        } else
        {
            transform.localPosition = new Vector3(0, 0, 10);
        }
    }
}
