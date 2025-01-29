using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameSettings gameSettings; 
    private int currentLevel; // Should remain private

    void Start()
    {
        // Get selected level from PlayerPrefs
        currentLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
        Debug.Log($"LevelManager Loaded, Selected Level: {currentLevel}");
    }

    // GETTER METODU EKLEYELİM!
    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public void SetCurrentLevel(int level)
    {
        currentLevel = level;
        Debug.Log($"LevelManager Updated Level: {currentLevel}");
    }


    public int GetCardCountForCurrentLevel()
    {
        int cardCount = gameSettings.GetCardCountForLevel(currentLevel);
        Debug.Log($"Selected Level {currentLevel}, Card Count: {cardCount}");
        return cardCount;
    }
}

