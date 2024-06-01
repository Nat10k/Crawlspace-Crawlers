using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private List<string> instructions;
    [SerializeField] private List<Outline> targets;
    [SerializeField] private TMP_Text smallInstructionText, bigInstructionText;
    [SerializeField] private TargetObject finish;
    [SerializeField] private GameObject bigInstruction, mainHUD;
    [SerializeField] private List<bool> isBig;
    private TMP_Text currInstructionText;
    private List<TutorialTrigger> triggers;
    private const float instructionSpeed = 0.03f;
    private Listener tutorEventListener;
    private int currIdx;
    private Coroutine instructionCoroutine;
    private InputAction uiClick;

    private void Awake()
    {
        // Set up skip instruction
        uiClick = InputHandler.inputs.UI.Click;

        tutorEventListener = new Listener();
        tutorEventListener.invoke = NextInstruction;
        EventManagers.Register("Tutorial", tutorEventListener);
        triggers = new List<TutorialTrigger>();
        // Get all tutorial triggers
        foreach (Outline target in targets)
        {
            if (target != null)
            {
                TutorialTrigger currTrigger = target.gameObject.GetComponent<TutorialTrigger>();
                triggers.Add(currTrigger);
            } else
            {
                triggers.Add(null);
            }
        }
        currIdx = 0;
        currInstructionText = smallInstructionText;
        ChooseInstructionType();
        if (targets[currIdx] != null)
        {
            targets[currIdx].enabled = true;
            triggers[currIdx].isActive = true;
        }
        instructionCoroutine = StartCoroutine(PlayInstruction());
    }

    private void OnEnable()
    {
        uiClick.performed += SkipInstruction;
        uiClick.Enable();
    }

    private void OnDisable()
    {
        uiClick.performed -= SkipInstruction;
        uiClick.Disable();
    }

    private void OnDestroy()
    {
        EventManagers.Unregister("Tutorial", tutorEventListener);
    }

    public void NextInstruction()
    {
        if (instructionCoroutine != null)
        {
            StopAllCoroutines();
        }
        if (targets[currIdx] != null)
        {
            targets[currIdx].enabled = false;
        }
        currIdx++;
        ChooseInstructionType();
        instructionCoroutine = StartCoroutine(PlayInstruction());
        if (targets[currIdx] != null)
        {
            targets[currIdx].enabled = true;
            triggers[currIdx].isActive = true;
        }
        if (currIdx == instructions.Count-1)
        {
            finish.TurnOnTarget();
            // Disable big instruction and resume time
            bigInstruction.SetActive(false);
            GameManager.ResumeGame();
        }
    }

    private void ChooseInstructionType()
    {
        currInstructionText.text = "";
        if (isBig[currIdx])
        {
            bigInstruction.SetActive(true);
            currInstructionText = bigInstructionText;
            mainHUD.SetActive(false);
            GameManager.PauseGame();
        }
        else
        {
            bigInstruction.SetActive(false);
            currInstructionText = smallInstructionText;
            mainHUD.SetActive(true);
            GameManager.ResumeGame();
        }
        currInstructionText.text = "";
    }

    private void SkipInstruction(InputAction.CallbackContext ctx)
    {
        if (instructionCoroutine != null)
        {
            StopAllCoroutines();
            currInstructionText.text = instructions[currIdx];
            instructionCoroutine = null;
        } else if (isBig[currIdx])
        {
            NextInstruction();
        }
    }

    private IEnumerator PlayInstruction()
    {
        foreach (char c in instructions[currIdx].ToCharArray())
        {
            currInstructionText.text += c;
            yield return new WaitForSecondsRealtime(instructionSpeed);
        }
        instructionCoroutine = null;
    }
}
