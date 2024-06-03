using UnityEngine;

public static class GameManager
{
    public static bool isPaused = false;
    public static int totalItemsCollected = 0;
    public static int totalLivesLost = 0;
    public static int totalScore = 0;

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

    public static void UpdateStats(int itemsCol, int livesLost, int score)
    {
        totalItemsCollected += itemsCol;
        totalLivesLost += livesLost;
        totalScore += score;
    }
}
