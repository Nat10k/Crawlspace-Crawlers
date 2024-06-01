using UnityEngine;

public static class GameManager
{
    public static bool isPaused = false;

    public static void PauseGame()
    {
        Time.timeScale = 0;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public static void ResumeGame()
    {
        Time.timeScale = 1;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
