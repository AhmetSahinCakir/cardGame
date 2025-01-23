using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int currentScore = 0; // Şu anki skor
    public int highScore = 0; // En yüksek skor

    public void AddScore(int points)
    {
        currentScore += points;

        if (currentScore > highScore)
        {
            highScore = currentScore;
        }

        Debug.Log($"Current Score: {currentScore}, High Score: {highScore}");
    }

    public void ResetScore()
    {
        currentScore = 0;
    }
}

