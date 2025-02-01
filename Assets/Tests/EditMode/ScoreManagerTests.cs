using NUnit.Framework;
using UnityEngine;

public class ScoreManagerTests
{
    private ScoreManager scoreManager;

    [SetUp]
    public void Setup()
    {
        PlayerPrefs.DeleteAll(); 
        PlayerPrefs.Save();
        
        var gameObject = new GameObject();
        scoreManager = gameObject.AddComponent<ScoreManager>();
        PlayerPrefs.DeleteAll(); // PlayerPrefs verilerini temizler
    }

    [Test]
    public void ScoreManager_InitialScoreIsZero()
    {
        Assert.AreEqual(0, scoreManager.CurrentScore, "Initial score should be zero.");
    }

    [Test]
    public void AddScore_IncreasesScoreCorrectly()
    {
        scoreManager.AddScore(10);
        Assert.AreEqual(10, scoreManager.CurrentScore, "Score should increase by 10.");
    }

    [Test]
    public void SubtractScore_DecreasesScoreCorrectly()
    {
        scoreManager.AddScore(10);
        scoreManager.SubtractScore(5);
        Assert.AreEqual(5, scoreManager.CurrentScore, "Score should decrease by 5.");
    }

    [Test]
    public void SubtractScore_DoesNotGoBelowZero()
    {
        scoreManager.SubtractScore(5);
        Assert.AreEqual(0, scoreManager.CurrentScore, "Score should not go below zero.");
    }

    [Test]
    public void ResetScore_SetsScoreToZero()
    {
        scoreManager.AddScore(15);
        scoreManager.ResetScore();
        Assert.AreEqual(0, scoreManager.CurrentScore, "Score should reset to zero.");
    }

    [Test]
    public void IncrementMatches_IncreasesMatchCount()
    {
        scoreManager.IncrementMatches();
        Assert.AreEqual(1, scoreManager.Matches, "Match count should increment by 1.");
    }

    [Test]
    public void IncrementTurns_IncreasesTurnCount()
    {
        scoreManager.IncrementTurns();
        Assert.AreEqual(1, scoreManager.Turns, "Turn count should increment by 1.");
    }

    [Test]
    public void AddScore_UpdatesHighScoreCorrectly()
    {
        scoreManager.AddScore(20);
        Assert.AreEqual(20, scoreManager.HighScore, "High score should be updated to 20.");
    }
}
