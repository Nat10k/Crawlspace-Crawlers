using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private List<string> instructions;
    [SerializeField] private List<Outline> targets;
    [SerializeField] private TMP_Text instructionText;
    [SerializeField] private TargetObject finish;
    private List<TutorialTrigger> triggers;
    private const float instructionSpeed = 0.03f;
    private Listener tutorEventListener;
    private int currIdx;
    private Coroutine instructionCoroutine;

    private void Awake()
    {
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
        if (targets[currIdx] != null)
        {
            targets[currIdx].enabled = true;
            triggers[currIdx].isActive = true;
        }
        instructionCoroutine = StartCoroutine(PlayInstruction());
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
        instructionCoroutine = StartCoroutine(PlayInstruction());
        if (targets[currIdx] != null)
        {
            targets[currIdx].enabled = true;
            triggers[currIdx].isActive = true;
        }
        if (currIdx == instructions.Count-1)
        {
            finish.TurnOnTarget();
        }
    }

    private void WholeInstruction()
    {
        //if (instructionCoroutine != null)
        //{
        //    StopAllCoroutines();
        //    instructionText.text = instructions[currIdx];
        //    dialogueCoroutine = null;
        //}
    }

    private IEnumerator PlayInstruction()
    {
        instructionText.text = "";
        foreach (char c in instructions[currIdx].ToCharArray())
        {
            instructionText.text += c;
            yield return new WaitForSecondsRealtime(instructionSpeed);
        }
        instructionCoroutine = null;
    }
}
