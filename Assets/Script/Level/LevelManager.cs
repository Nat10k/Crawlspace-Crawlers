
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] GameObject levelFinishedCanvas, gameOverCanvas;
    [SerializeField] Slider timerSlider;
    [SerializeField] List<Color> sliderColors;
    [SerializeField] Image timerSliderFill;
    [SerializeField] Transform propsParent;
    List<TargetObject> targetObjects;

    int objectCount;
    float timer, timeAddition;
    bool isInTutorial;
    const float maxTime = 60f;
    Listener finishListener, overListener, collectObjListener, tutorialFinishListener;
    private void Awake()
    {
        finishListener = new Listener();
        overListener = new Listener();
        collectObjListener = new Listener();
        tutorialFinishListener = new Listener();
        finishListener.invoke = LevelFinish;
        overListener.invoke = GameOver;
        collectObjListener.invoke = NextTarget;
        tutorialFinishListener.invoke = TutorialFinished;

        EventManagers.Register("Finish", finishListener);
        EventManagers.Register("GameOver", overListener);
        EventManagers.Register("CollectObj", collectObjListener);
        EventManagers.Register("TutorialFinish", tutorialFinishListener);

        // Setup timer
        timer = maxTime;
        timeAddition = 15f;

        isInTutorial = !PlayerPrefs.HasKey("TutorialFinish");
        if (isInTutorial)
        {
            timerSlider.gameObject.SetActive(false);
        }

        // Setup object counter
        objectCount = 0;

        // Setup target objects
        targetObjects = new List<TargetObject>(propsParent.GetComponentsInChildren<TargetObject>());
    }

    private void TutorialFinished()
    {
        isInTutorial = false;
        timerSlider.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        EventManagers.Unregister("Finish", finishListener);
        EventManagers.Unregister("GameOver", overListener);
        EventManagers.Unregister("CollectObj", collectObjListener);
        EventManagers.Unregister("TutorialFinish", tutorialFinishListener);
    }

    private void LevelFinish()
    {
        levelFinishedCanvas.SetActive(true);
        Time.timeScale = 0;
    }

    private void GameOver()
    {
        gameOverCanvas.SetActive(true);
        Time.timeScale = 0;
    }

    private void NextTarget()
    {
        targetObjects.RemoveAt(0);
        if (targetObjects.Count > 0)
        {
            timer += timeAddition;
            if (timer > maxTime)
            {
                timer = maxTime;
            }
            targetObjects[0].TurnOnTarget();
            timeAddition /= 2;
        } else
        {
            LevelFinish();
        }
    }

    private void Update()
    {
        if (!isInTutorial)
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
