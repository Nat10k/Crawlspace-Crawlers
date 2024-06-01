
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
    const float maxTime = 60f;
    Listener finishListener, overListener, collectObjListener;
    private void Awake()
    {
        finishListener = new Listener();
        overListener = new Listener();
        collectObjListener = new Listener();
        finishListener.invoke = LevelFinish;
        overListener.invoke = GameOver;
        collectObjListener.invoke = NextTarget;

        EventManagers.Register("Finish", finishListener);
        EventManagers.Register("GameOver", overListener);
        EventManagers.Register("CollectObj", collectObjListener);

        // Setup timer
        timer = maxTime;
        timeAddition = 15f;

        // Setup object counter
        objectCount = 0;

        // Setup target objects
        targetObjects = new List<TargetObject>(propsParent.GetComponentsInChildren<TargetObject>());
    }

    private void OnDestroy()
    {
        EventManagers.Unregister("Finish", finishListener);
        EventManagers.Unregister("GameOver", overListener);
    }

    private void LevelFinish()
    {
        levelFinishedCanvas.SetActive(true);
        Time.timeScale = 0;
    }

    private void GameOver()
    {
        gameOverCanvas.SetActive(true);
        Time .timeScale = 0;
    }

    private void NextTarget()
    {
        targetObjects.RemoveAt(0);
        if (targetObjects.Count > 0)
        {
            timer += timeAddition;
            targetObjects[0].TurnOnTarget();
            timeAddition /= 2;
        } else
        {
            LevelFinish();
        }
    }

    private void Update()
    {
        // Reduce timer
        timer -= Time.deltaTime;
        timerSlider.value = timer;
        if (timer <= 0)
        {
            EventManagers.InvokeEvent("Finish");
        } else if (timer <= maxTime/3)
        {
            timerSliderFill.color = sliderColors[0];
        } else if (timer <= maxTime/2)
        {
            timerSliderFill.color = sliderColors[1];
        } else
        {
            timerSliderFill.color = sliderColors[2];
        }
    }
}
