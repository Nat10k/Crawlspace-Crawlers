
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    InputAction reset;
    [SerializeField] GameObject levelFinishedCanvas, gameOverCanvas;
    [SerializeField] Transform cicak;
    private void OnEnable()
    {
        reset = InputHandler.inputs.Player.Reset;
        reset.performed += ResetLevel;
        reset.Enable();
    }
    public void LevelFinish()
    {
        levelFinishedCanvas.SetActive(true);
        Time.timeScale = 0;
    }

    public void GameOver()
    {
        gameOverCanvas.SetActive(true);
        Time .timeScale = 0;
    }

    private void ResetLevel(InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
