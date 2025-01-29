using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int CurrentScore { get; private set; } = 0;
    public int HighScore { get; private set; } = 0;
    public int LastScore { get; private set; } = 0;
    public int Matches { get; private set; } = 0;
    public int Turns { get; private set; } = 0;

    public event Action OnScoreChanged; // Event for UI updates

    private void Awake()
    {
        LoadScores(); // Load high score and last session score
    }

    /// Adds score and updates high score if needed
    public void AddScore(int points)
    {
        if (points <= 0) return;

        CurrentScore += points;
        UpdateHighScore(); // Check if high score needs an update

        SaveScores();
        OnScoreChanged?.Invoke();

        Debug.Log($"Score Updated! Current: {CurrentScore}, High Score: {HighScore}");
    }

    /// Deducts score but ensures it never goes below 0
    public void SubtractScore(int points)
    {
        if (points <= 0) return;

        CurrentScore = Mathf.Max(0, CurrentScore - points);
        SaveScores();
        OnScoreChanged?.Invoke();

        Debug.Log($"Score Deducted! Current: {CurrentScore}");
    }

    /// Increments match count and updates UI
    public void IncrementMatches()
    {
        Matches++;
        OnScoreChanged?.Invoke();
    }

    /// Increments turn count and updates UI
    public void IncrementTurns()
    {
        Turns++;
        OnScoreChanged?.Invoke();
    }

    /// Resets the score and optionally saves progress
    public void ResetScore(bool saveProgress = true)
    {
        LastScore = CurrentScore;
        CurrentScore = 0;
        Matches = 0;
        Turns = 0;

        if (saveProgress)
        {
            SaveScores();
            Debug.Log($"Score Reset & Saved! Last: {LastScore}, High: {HighScore}");
        }
        else
        {
            Debug.Log("Score Reset without saving progress.");
        }

        OnScoreChanged?.Invoke();
    }

    /// Loads high score and last session score from `PlayerPrefs`
    private void LoadScores()
    {
        HighScore = PlayerPrefs.GetInt("HighScore", 0);
        LastScore = PlayerPrefs.GetInt("LastScore", 0);
        CurrentScore = 0; // Always reset at session start
    }

    /// Saves current, last, and high scores to `PlayerPrefs`
    private void SaveScores()
    {
        PlayerPrefs.SetInt("LastScore", LastScore);
        PlayerPrefs.SetInt("CurrentScore", CurrentScore);
        PlayerPrefs.SetInt("HighScore", HighScore);
        PlayerPrefs.Save();
    }

    /// Updates the high score only if needed
    private void UpdateHighScore()
    {
        if (CurrentScore > HighScore)
        {
            HighScore = CurrentScore;
            PlayerPrefs.SetInt("HighScore", HighScore);
            PlayerPrefs.Save();
            Debug.Log($"New High Score: {HighScore}");
        }
    }
}
