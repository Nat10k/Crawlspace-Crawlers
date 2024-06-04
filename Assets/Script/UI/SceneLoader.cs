using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    private static GameObject instance;
    [SerializeField] GameObject loadingScreen;
    [SerializeField] Image loadingImage;
    [SerializeField] List<Sprite> loadSequence;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(loadingScreen);
            instance = gameObject;
        }
        else
        {
            Destroy(loadingScreen);
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += DeactivateLoadScreen;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= DeactivateLoadScreen;
    }

    private void DeactivateLoadScreen(Scene scene, LoadSceneMode mode)
    {
        loadingScreen.SetActive(false);
    }

    public IEnumerator LoadNewScene(string sceneName)
    {
        Time.timeScale = 1;
        AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(sceneName);
        loadingScreen.SetActive(true);
        int currIdx = 0;
        while (!sceneLoad.isDone)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            currIdx++;
            currIdx %= loadSequence.Count;
            loadingImage.sprite = loadSequence[currIdx];
        }
    }
}
