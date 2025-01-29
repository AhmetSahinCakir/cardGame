using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameSettings gameSettings; // Injected via Unity Inspector
    private int currentLevel;

    public event Action<int> OnLevelChanged; // Event to notify UI

    private void Awake()
    {
        if (!gameSettings)
        {
            gameSettings = Resources.Load<GameSettings>("GameSettings");
            Debug.LogWarning("GameSettings not assigned! Loading from Resources...");
        }

        LoadLevelFromPrefs();
    }

    /// Loads the selected level from PlayerPrefs, ensuring a valid level.
    private void LoadLevelFromPrefs()
    {
        currentLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
        currentLevel = Mathf.Max(1, currentLevel); // 🔹 Ensures level is at least 1
        Debug.Log($"LevelManager Loaded - Current Level: {currentLevel}");
    }

    /// Returns the **current** level.
    public int GetCurrentLevel() => currentLevel;

    /// Updates the level and saves to PlayerPrefs.
    public void SetCurrentLevel(int level)
    {
        if (level < 1)
        {
            Debug.LogError($"Invalid level {level}! Level must be at least 1.");
            return;
        }

        currentLevel = level;
        PlayerPrefs.SetInt("SelectedLevel", currentLevel);
        PlayerPrefs.Save();

        OnLevelChanged?.Invoke(currentLevel); // 🔹 Notify UI dynamically
        Debug.Log($"LevelManager Updated - New Level: {currentLevel}");
    }

    /// Gets the **number of cards** based on the current level, using `GameSettings`.
    public int GetCardCountForCurrentLevel()
    {
        if (!gameSettings)
        {
            Debug.LogError("GameSettings is missing! Cannot get card count.");
            return 0;
        }

        int cardCount = gameSettings.GetCardCountForLevel(currentLevel);
        Debug.Log($"Level {currentLevel} - Card Count: {cardCount}");
        return cardCount;
    }
}
