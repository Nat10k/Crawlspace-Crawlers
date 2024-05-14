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
    [SerializeField] FinishObject finish;
    int currIdx;

    private void Awake()
    {
        currIdx = 0;
        if (targets[currIdx] != null)
        {
            targets[currIdx].enabled = true;
        }
        instructionText.text = instructions[currIdx];
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
        }
        if (currIdx == instructions.Count-1)
        {
            finish.TurnOnFinish();
        }
    }
}
