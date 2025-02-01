using NUnit.Framework;
using UnityEngine;
using UnityEditor;

public class LevelManagerTests
{
    private LevelManager levelManager;
    private GameSettings gameSettings;

    [SetUp]
    public void Setup()
    {
        PlayerPrefs.DeleteAll(); 
        PlayerPrefs.Save();

        var gameObject = new GameObject();
        levelManager = gameObject.AddComponent<LevelManager>();
        gameSettings = ScriptableObject.CreateInstance<GameSettings>();

        // GameSettings'i LevelManager'a atayalım
        levelManager.GetType().GetField("gameSettings", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(levelManager, gameSettings);
    }

    [Test]
    public void SetCurrentLevel_UpdatesLevelCorrectly()
    {
        levelManager.SetCurrentLevel(3);
        Assert.AreEqual(3, levelManager.GetCurrentLevel(), "Current level should be set to 3.");
    }

    [Test]
    public void GetCardCountForCurrentLevel_ReturnsCorrectCardCount()
    {
        levelManager.SetCurrentLevel(2);
        int expectedCardCount = gameSettings.GetCardCountForLevel(2);
        Assert.AreEqual(expectedCardCount, levelManager.GetCardCountForCurrentLevel(), "Card count should match GameSettings for level 2.");
    }
}
