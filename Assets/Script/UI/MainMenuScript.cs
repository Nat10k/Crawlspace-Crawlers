using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    bool isInTutorial;
    private void Awake()
    {
        isInTutorial = !PlayerPrefs.HasKey("TutorialFinish");
    }

    public void Play()
    {
        if (isInTutorial)
        {
            SceneManager.LoadSceneAsync("Toilet Tutorial");
        } else
        {
            SceneManager.LoadSceneAsync("Dapur");
        }
    }

    public void Exit()
    {
        Application.Quit();
    }
}
