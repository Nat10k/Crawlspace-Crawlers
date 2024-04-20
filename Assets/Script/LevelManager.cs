
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    PInput pInput;
    InputAction reset;
    [SerializeField] GameObject levelFinishedCanvas;
    [SerializeField] Transform cicak;
    private void Awake()
    {
        pInput = new PInput();
    }
    private void OnEnable()
    {
        reset = pInput.Player.Reset;
        reset.performed += ResetLevel;
        reset.Enable();
    }
    public void LevelFinish()
    {
        levelFinishedCanvas.SetActive(true);
        Time.timeScale = 0;
    }

    private void ResetLevel(InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
