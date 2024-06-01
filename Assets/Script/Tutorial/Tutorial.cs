using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Tutorial : MonoBehaviour
{
    [SerializeField] List<string> instructions;
    [SerializeField] List<Outline> targets;
    [SerializeField] TMP_Text instructionText;
    [SerializeField] TargetObject finish;
    private List<TutorialTrigger> triggers;
    Listener tutorEventListener;
    int currIdx;

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
        instructionText.text = instructions[currIdx];
    }

    private void OnDestroy()
    {
        EventManagers.Unregister("Tutorial", tutorEventListener);
    }

    public void NextInstruction()
    {
        if (targets[currIdx] != null)
        {
            targets[currIdx].enabled = false;
        }
        currIdx++;
        instructionText.text = instructions[currIdx];
        if (targets[currIdx] != null)
        {
            targets[currIdx].enabled = true;
            triggers[currIdx].isActive = true;
        }
        if (currIdx == instructions.Count-1)
        {
            finish.TurnOnFinish();
        }
    }
}
