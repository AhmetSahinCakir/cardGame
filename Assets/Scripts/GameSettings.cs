using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "CardMatch/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("Level Settings")]
    public int startingCardCount = 16;   // Level 1 için kart sayısı (8 çift)
    public int cardIncreasePerLevel = 4; // Her levelde 2 çift daha ekle (4 kart)
    public int maxCardCount = 36;        // Maximum kart sayısı (18 çift)
    
    [Header("Score Settings")]
    public int pointsPerMatch = 10;      // Eşleşme başına puan

    // Her level için kart sayısını hesapla
    public int GetCardCountForLevel(int level)
    {
        int cardCount = startingCardCount + ((level - 1) * cardIncreasePerLevel);
        return Mathf.Min(cardCount, maxCardCount);
    }
}

