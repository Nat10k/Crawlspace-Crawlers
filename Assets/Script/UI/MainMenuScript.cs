using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    bool isInTutorial;
    SceneLoader loader;

    private void Awake()
    {
        isInTutorial = !PlayerPrefs.HasKey("TutorialFinish");
        loader = GameObject.Find("SceneLoader").GetComponent<SceneLoader>();
    }

    public void Play()
    {
        if (isInTutorial)
        {
            StartCoroutine(loader.LoadNewScene("Toilet Tutorial"));
        } else
        {
            StartCoroutine(loader.LoadNewScene("Dapur"));
        }
    }

    public void Exit()
    {
        Application.Quit();
    }
}
