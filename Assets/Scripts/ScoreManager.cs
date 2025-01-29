using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    public int currentScore = 0; // Current score
    public int highScore = 0; // Highest score
    public int lastScore = 0; // Last played score
    public int matches = 0; // Total correct matches
    public int turns = 0; // Total number of attempts

    // Event to notify UI updates
    public event Action OnScoreChanged;

    void Start()
    {
        LoadScores(); // Load scores at start
    }

    public void AddScore(int points)
    {
        currentScore += points;

        if (currentScore > highScore)
        {
            highScore = currentScore; 
            PlayerPrefs.SetInt("HighScore", highScore);
        }

        SaveScores();
        OnScoreChanged?.Invoke(); // Notify UI

        Debug.Log($"Points Added! Current Score: {currentScore}, High Score: {highScore}");
    }

    public void SubtractScore(int points)
    {
        currentScore -= points;

        if (currentScore < 0)
            currentScore = 0;

        SaveScores();
        OnScoreChanged?.Invoke(); // Notify UI

        Debug.Log($"Points Deducted! Current Score: {currentScore}");
    }

    public void IncrementMatches()
    {
        matches++;
        OnScoreChanged?.Invoke(); // Notify UI
    }

    public void IncrementTurns()
    {
        turns++;
        OnScoreChanged?.Invoke(); // Notify UI
    }

    public void ResetScore(bool saveProgress = true)
    {
        lastScore = currentScore;
        currentScore = 0;
        matches = 0;
        turns = 0;

        if (saveProgress)
        {
            SaveScores();
            Debug.Log($"Scores Reset & Saved! Last Score: {lastScore}, High Score: {highScore}");
        }
        else
        {
            Debug.Log("Scores Reset without saving progress.");
        }

        OnScoreChanged?.Invoke(); // Update UI
    }


    private void LoadScores()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        lastScore = PlayerPrefs.GetInt("LastScore", 0);
        currentScore = PlayerPrefs.GetInt("CurrentScore", 0);
    }

    private void SaveScores()
    {
        PlayerPrefs.SetInt("LastScore", lastScore);
        PlayerPrefs.SetInt("CurrentScore", currentScore);
        PlayerPrefs.Save();
    }
}
