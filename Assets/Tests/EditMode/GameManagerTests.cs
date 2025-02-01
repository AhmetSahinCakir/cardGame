using NUnit.Framework;
using UnityEngine;

public class GameManagerTests
{
    private GameManager gameManager;
    private GameSettings gameSettings;
    private ScoreManager scoreManager;
    private LevelManager levelManager;

    [SetUp]
    public void Setup()
    {
        PlayerPrefs.DeleteAll(); 
        PlayerPrefs.Save();

        var gameObject = new GameObject();
        gameManager = gameObject.AddComponent<GameManager>();

        gameSettings = ScriptableObject.CreateInstance<GameSettings>();
        scoreManager = gameObject.AddComponent<ScoreManager>();
        levelManager = gameObject.AddComponent<LevelManager>();

        gameManager.Initialize(gameSettings, scoreManager, levelManager);
    }

    [Test]
    public void GameManager_InitializesDependenciesCorrectly()
    {
        Assert.IsNotNull(gameManager, "GameManager should be initialized.");
        Assert.IsNotNull(gameSettings, "GameSettings should be initialized.");
        Assert.IsNotNull(scoreManager, "ScoreManager should be initialized.");
        Assert.IsNotNull(levelManager, "LevelManager should be initialized.");
    }

    [Test]
    public void StartGame_SetsCorrectCardCount()
    {
        PlayerPrefs.SetInt("SelectedLevel", 1);

        // Private alanlara reflection ile erişim sağlama
        typeof(GameManager).GetField("gameSettings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(gameManager, gameSettings);
        
        typeof(GameManager).GetField("scoreManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(gameManager, scoreManager);
        
        typeof(GameManager).GetField("levelManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(gameManager, levelManager);

        gameManager.StartGame();

        Assert.Greater(levelManager.GetCardCountForCurrentLevel(), 0, "Card count should be greater than 0 after starting game.");
    }


}
