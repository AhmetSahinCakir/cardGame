using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "CardMatch/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("Level Settings")]
    [SerializeField, Min(2)] private int startingCardCount = 16;   // Minimum 2 to maintain pairs
    [SerializeField, Min(2)] private int cardIncreasePerLevel = 4; // Minimum 2 for valid pairs
    [SerializeField, Min(2)] private int maxCardCount = 36;        // Prevents excessive cards

    [Header("Score Settings")]
    [SerializeField, Min(1)] private int pointsPerMatch = 10;      // Minimum 1 point per match
    [SerializeField, Min(1)] private int wrongSelectionPenalty = 3; // Minimum 1 penalty

    [Header("Difficulty Settings")]
    [SerializeField] private float difficultyMultiplier = 1.0f; // Can be used for scaling future mechanics

    // 🔹 Read-Only Properties for External Access
    public int StartingCardCount => startingCardCount;
    public int CardIncreasePerLevel => cardIncreasePerLevel;
    public int MaxCardCount => maxCardCount;
    public int PointsPerMatch => pointsPerMatch;
    public int WrongSelectionPenalty => wrongSelectionPenalty;
    public float DifficultyMultiplier => difficultyMultiplier;

    /// Returns the **valid** number of cards for the given level, ensuring **even pairs** and max limit.
    public int GetCardCountForLevel(int level)
    {
        int cardCount = startingCardCount + ((level - 1) * cardIncreasePerLevel);
        cardCount = Mathf.Min(cardCount, maxCardCount);

        // 🔹 Ensures card count is always even (pairs must match)
        if (cardCount % 2 != 0)
        {
            cardCount = Mathf.Max(2, cardCount - 1); 
        }

        return cardCount;
    }

    /// Returns the points gained or lost based on the match result.
    public int GetScoreForMatch(bool isCorrect)
    {
        return isCorrect ? pointsPerMatch : -wrongSelectionPenalty;
    }

    /// Adjust difficulty multiplier (e.g., for harder levels)
    public void SetDifficultyMultiplier(float newMultiplier)
    {
        difficultyMultiplier = Mathf.Max(0.1f, newMultiplier); // Prevents negative or zero scaling
    }
}
