using NUnit.Framework;
using UnityEngine;

public class GameSettingsTests
{
    private GameSettings gameSettings;

    [SetUp]
    public void Setup()
    {
        PlayerPrefs.DeleteAll(); 
        PlayerPrefs.Save();
        
        gameSettings = ScriptableObject.CreateInstance<GameSettings>();
    }

    [Test]
    public void GameSettings_InitialValuesAreCorrect()
    {
        Assert.AreEqual(16, gameSettings.StartingCardCount, "Default StartingCardCount should be 16.");
        Assert.AreEqual(4, gameSettings.CardIncreasePerLevel, "CardIncreasePerLevel should be 4.");
        Assert.AreEqual(10, gameSettings.PointsPerMatch, "PointsPerMatch should be 10.");
        Assert.AreEqual(3, gameSettings.WrongSelectionPenalty, "WrongSelectionPenalty should be 3.");
    }

    [Test]
    public void GetCardCountForLevel_ReturnsCorrectCardCount()
    {
        int level = 3;
        int expectedCardCount = gameSettings.StartingCardCount + (level - 1) * gameSettings.CardIncreasePerLevel;
        Assert.AreEqual(expectedCardCount, gameSettings.GetCardCountForLevel(level));
    }

    [Test]
    public void GetScoreForMatch_ReturnsCorrectScore()
    {
        Assert.AreEqual(gameSettings.PointsPerMatch, gameSettings.GetScoreForMatch(true), "Points for correct match should match the configured value.");
        Assert.AreEqual(-gameSettings.WrongSelectionPenalty, gameSettings.GetScoreForMatch(false), "Penalty for wrong match should match the configured value.");
    }
}
