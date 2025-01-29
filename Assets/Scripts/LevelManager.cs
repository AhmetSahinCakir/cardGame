using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameSettings gameSettings; 
    private int currentLevel; // Private olarak kalmalı

    void Start()
    {
        // Seçilen leveli PlayerPrefs’ten al
        currentLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
        Debug.Log($"LevelManager Yüklendi, Seçili Level: {currentLevel}");
    }

    // GETTER METODU EKLEYELİM!
    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public void SetCurrentLevel(int level)
    {
        currentLevel = level;
        Debug.Log($"LevelManager'da Güncellenmiş Level: {currentLevel}");
    }


    public int GetCardCountForCurrentLevel()
    {
        int cardCount = gameSettings.GetCardCountForLevel(currentLevel);
        Debug.Log($"🃏 Seçilen Level {currentLevel}, Kart Sayısı: {cardCount}");
        return cardCount;
    }
}

