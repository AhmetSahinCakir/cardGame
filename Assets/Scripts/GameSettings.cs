using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "CardMatch/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("Level Settings")]
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    public int startingCardCount = 16;   // Number of cards for Level 1 (8 pairs)
    public int cardIncreasePerLevel = 4; // Add 2 more pairs per level (4 cards)
    public int maxCardCount = 36;        // Maximum number of cards (18 pairs)
    
    [Header("Score Settings")]
    public int pointsPerMatch = 10;      // Points per match
    public int wrongSelectionPenalty = 3;


    // Calculate number of cards for each level
    public int GetCardCountForLevel(int level)
    {
        int cardCount = startingCardCount + ((level - 1) * cardIncreasePerLevel);
=======
=======
>>>>>>> Stashed changes
    public int startingCardCount = 16;   // Level 1 için kart sayısı (8 çift)
    public int cardIncreasePerLevel = 4; // Her levelde 2 çift daha ekle (4 kart)
    public int maxCardCount = 36;        // Maximum kart sayısı (18 çift)
    
    [Header("Score Settings")]
    public int pointsPerMatch = 10;      // Eşleşme başına puan

    // Her level için kart sayısını hesapla
    public int GetCardCountForLevel(int level)
    {
        int cardCount = startingCardCount + ((level - 1) * cardIncreasePerLevel);
        Debug.Log($"Level {level}: {cardCount} cards");
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        return Mathf.Min(cardCount, maxCardCount);
    }
}

