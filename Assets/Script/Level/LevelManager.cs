
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] GameObject levelFinishedCanvas, gameOverCanvas, statsCanvas;
    [SerializeField] Slider timerSlider;
    [SerializeField] List<Color> sliderColors;
    [SerializeField] Image timerSliderFill;
    [SerializeField] Transform propsParent;
    List<TargetObject> targetObjects;
    List<TMP_Text> statsText;

    int objectCount, score, livesLost, totalObjectsCount;
    const int objScoreMult = 100, livesLostMult = 200;
    float timer;
    bool isInTutorial, stopTimer;
    const float maxTime = 60f, timeAddition = 3f;
    Listener finishListener, overListener, collectObjListener, tutorialFinishListener, damagedListener;
    private void Awake()
    {
        // Register listeners
        finishListener = new Listener();
        overListener = new Listener();
        collectObjListener = new Listener();
        tutorialFinishListener = new Listener();
        damagedListener = new Listener();
        finishListener.invoke = LevelFinish;
        overListener.invoke = GameOver;
        collectObjListener.invoke = CollectedTarget;
        tutorialFinishListener.invoke = TutorialFinished;
        damagedListener.invoke = LifeLost;

        EventManagers.Register("Finish", finishListener);
        EventManagers.Register("GameOver", overListener);
        EventManagers.Register("CollectObj", collectObjListener);
        EventManagers.Register("TutorialFinish", tutorialFinishListener);
        EventManagers.Register("Damaged", damagedListener);

        // Setup timer
        timer = maxTime;
        stopTimer = false;

        // Setup object counter
        objectCount = 0;

        // Setup target objects
        targetObjects = new List<TargetObject>(propsParent.GetComponentsInChildren<TargetObject>());
        totalObjectsCount = targetObjects.Count;

        // Setup lives lost
        livesLost = 0;

        // Setup stats text
        statsText = new(statsCanvas.GetComponentsInChildren<TMP_Text>());

        isInTutorial = !PlayerPrefs.HasKey("TutorialFinish");
        if (isInTutorial)
        {
            stopTimer = true;
            timerSlider.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        if (!isInTutorial && targetObjects.Count > 0)
        {
            foreach (TargetObject targetObject in targetObjects)
            {
                targetObject.TurnOnTarget();
            }
        }
    }

    private void TutorialFinished()
    {
        isInTutorial = false;
        stopTimer = false;
        timerSlider.gameObject.SetActive(true);
        foreach (TargetObject targetObject in targetObjects)
        {
            targetObject.TurnOnTarget();
        }
    }

    private void LifeLost()
    {
        livesLost++;
    }

    private void OnDestroy()
    {
        EventManagers.Unregister("Finish", finishListener);
        EventManagers.Unregister("GameOver", overListener);
        EventManagers.Unregister("CollectObj", collectObjListener);
        EventManagers.Unregister("TutorialFinish", tutorialFinishListener);
        EventManagers.Unregister("Damaged", damagedListener);
    }

    private void LevelFinish()
    {
        stopTimer = true;
        EventManagers.InvokeEvent("StopMove");
        score = objectCount * objScoreMult - livesLost * livesLostMult;
        GameManager.UpdateStats(objectCount, livesLost, score);
        levelFinishedCanvas.SetActive(true);
        StartCoroutine(ShowStats());
    }

    private void GameOver()
    {
        stopTimer = true;
        EventManagers.InvokeEvent("StopMove");
        score = 0;
        GameManager.UpdateStats(objectCount, livesLost, score);
        gameOverCanvas.SetActive(true);
        StartCoroutine(ShowStats());
    }

    private void CollectedTarget()
    {
        objectCount++;
        targetObjects.RemoveAt(0);
        if (targetObjects.Count > 0)
        {
            timer += timeAddition;
            if (timer > maxTime)
            {
                timer = maxTime;
            }
        }
        else
        {
            LevelFinish();
        }
    }

    IEnumerator ShowStats()
    {
        statsText[1].text = "Items collected : " + objectCount.ToString();
        statsText[2].text = "Live lost : " + livesLost.ToString();
        statsText[3].text = "Score : " + score.ToString();
        string grade;
        if (score == 0)
        {
            grade = "F";
        }
        else if (score > totalObjectsCount * 0.85f * objScoreMult)
        {
            grade = "A";
        } else if ( score > totalObjectsCount * 0.7f * objScoreMult)
        {
            grade = "B";
        }
        else if (score > totalObjectsCount * 0.5f * objScoreMult)
        {
            grade = "C";
        } else
        {
            grade = "D";
        }
        statsText[4].text = "Grade : " + grade;
        yield return new WaitForSeconds(3);
        levelFinishedCanvas.SetActive(false);
        gameOverCanvas.SetActive(false);
        statsCanvas.SetActive(true);
    }

    private void Update()
    {
        if (!stopTimer)
        {
            // Reduce timer
            timer -= Time.deltaTime;
            timerSlider.value = timer;
            if (timer <= 0)
            {
                EventManagers.InvokeEvent("Finish");
            }
            else if (timer <= maxTime / 3)
            {
                timerSliderFill.color = sliderColors[0];
            }
            else if (timer <= maxTime / 2)
            {
                timerSliderFill.color = sliderColors[1];
            }
            else
            {
                timerSliderFill.color = sliderColors[2];
            }
        }
    }
}
