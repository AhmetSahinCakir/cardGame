using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "CardMatch/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("Level Settings")]
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
        return Mathf.Min(cardCount, maxCardCount);
    }
}

