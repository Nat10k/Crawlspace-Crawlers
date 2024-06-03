using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private List<string> instructions;
    [SerializeField] private List<GameObject> targets;
    [SerializeField] private TMP_Text smallInstructionText, bigInstructionText;
    [SerializeField] private TargetObject finish;
    [SerializeField] private GameObject bigInstruction, mainHUD;
    [SerializeField] private List<bool> isBig;
    private TMP_Text currInstructionText;
    private List<TutorialTrigger> triggers;
    private List<Outline> outlines;
    private const float instructionSpeed = 0.03f;
    private Listener tutorEventListener;
    private int currIdx;
    private Coroutine instructionCoroutine;
    private InputAction leftClick;

    private void Awake()
    {
        // Set up skip instruction
        leftClick = InputHandler.inputs.Player.Fire;
        if (PlayerPrefs.HasKey("TutorialFinish"))
        {
            enabled = false;
            return;
        }

        mainHUD.SetActive(false);
        tutorEventListener = new Listener();
        tutorEventListener.invoke = NextInstruction;
        EventManagers.Register("Tutorial", tutorEventListener);
        triggers = new List<TutorialTrigger>();
        outlines = new List<Outline>();
        // Get all tutorial triggers
        foreach (GameObject target in targets)
        {
            if (target != null)
            {
                triggers.Add(target.GetComponent<TutorialTrigger>());
                outlines.Add(target.GetComponent<Outline>());
            } else
            {
                outlines.Add(null);
                triggers.Add(null);
            }
        }
        currIdx = 0;
        currInstructionText = smallInstructionText;
        ChooseInstructionType();
        if (targets[currIdx] != null)
        {
            targets[currIdx].SetActive(true);
            if (outlines[currIdx] != null)
                outlines[currIdx].enabled = true;
            if (triggers[currIdx] != null)
                triggers[currIdx].isActive = true;
        }
        instructionCoroutine = StartCoroutine(PlayInstruction());
    }

    private void OnEnable()
    {
        leftClick.performed += SkipInstruction;
    }

    private void OnDisable()
    {
        leftClick.performed -= SkipInstruction;
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
        if (outlines[currIdx] != null && !triggers[currIdx].isFinish)
        {
            outlines[currIdx].enabled = false;
        }
        currIdx++;
        if (currIdx < instructions.Count)
        {
            ChooseInstructionType();
            instructionCoroutine = StartCoroutine(PlayInstruction());
            if (targets[currIdx] != null)
            {
                targets[currIdx].SetActive(true);
                if (outlines[currIdx] != null)
                    outlines[currIdx].enabled = true;
                if (triggers[currIdx] != null)
                    triggers[currIdx].isActive = true;
            }
            if (triggers[currIdx] != null && triggers[currIdx].isFinish)
            {
                finish.TurnOnTarget();
            }
        } else
        {
            // Disable big instruction and resume time
            bigInstruction.SetActive(false);
            EventManagers.InvokeEvent("TutorialFinish");
            PlayerPrefs.SetInt("TutorialFinish", 1);
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
        if (ctx.performed)
        {
            if (instructionCoroutine != null)
            {
                StopAllCoroutines();
                currInstructionText.text = instructions[currIdx];
                instructionCoroutine = null;
            }
            else if (currIdx < instructions.Count && isBig[currIdx])
            {
                NextInstruction();
            }
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
