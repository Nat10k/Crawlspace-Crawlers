using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResetTutorial : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.f7Key.wasPressedThisFrame)
        {
            PlayerPrefs.DeleteKey("TutorialFinish");
        }
    }
}
