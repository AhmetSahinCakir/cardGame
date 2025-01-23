using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameSettings gameSettings; // Oyunun ayarları
    public int currentLevel = 1; // Mevcut seviye

    public int GetCardCountForCurrentLevel()
    {
        int cardCount = gameSettings.startingCardCount +
                        (currentLevel - 1) * gameSettings.cardIncreasePerLevel;

        return Mathf.Min(cardCount, gameSettings.maxCardCount);
    }

    public void NextLevel()
    {
        currentLevel++;
        Debug.Log($"Level: {currentLevel}");
    }
}

