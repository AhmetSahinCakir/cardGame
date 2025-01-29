using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Level Selection")]
    [SerializeField] private Toggle[] levelToggles; // Level selection toggles
    [SerializeField] private ToggleGroup toggleGroup;

    [Header("Score UI")]
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI lastScoreText;

    [Header("Dependencies")]
    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private GameManager gameManager;

    private const int TOTAL_LEVELS = 6;
    private int selectedLevel = 1;

    private void Awake()
    {
        EnsureDependencies();
    }

    private void Start()
    {
        InitializeLevelSelection();
        UpdateScores();
    }

    /// Ensures all required dependencies are assigned.
    private void EnsureDependencies()
    {
        bool allDependenciesLoaded = true;

        if (!gameSettings)
        {
            gameSettings = Resources.Load<GameSettings>("GameSettings");
            if (!gameSettings)
            {
                Debug.LogError("GameSettings asset is missing in Resources folder!");
                allDependenciesLoaded = false;
            }
        }

        if (!scoreManager)
        {
            scoreManager = FindObjectOfType<ScoreManager>();
            if (!scoreManager)
            {
                Debug.LogError("ScoreManager is missing in the scene!");
                allDependenciesLoaded = false;
            }
        }

        if (!levelManager)
        {
            levelManager = FindObjectOfType<LevelManager>();
            if (!levelManager)
            {
                Debug.LogError("LevelManager is missing in the scene!");
                allDependenciesLoaded = false;
            }
        }

        if (!gameManager)
        {
            gameManager = FindObjectOfType<GameManager>();
            if (!gameManager)
            {
                Debug.LogError("GameManager is missing in the scene!");
                allDependenciesLoaded = false;
            }
        }

        if (allDependenciesLoaded)
        {
            gameManager.Initialize(gameSettings, scoreManager, levelManager);
            Debug.Log("Dependencies successfully initialized in MainMenuManager.");
        }
        else
        {
            Debug.LogError("Some dependencies are missing in MainMenuManager! Check scene objects.");
        }
    }


    /// Initializes level selection UI from PlayerPrefs.
    private void InitializeLevelSelection()
    {
        if (levelToggles == null || levelToggles.Length != TOTAL_LEVELS)
        {
            Debug.LogError($"Level toggles array must contain exactly {TOTAL_LEVELS} toggles!");
            return;
        }

        selectedLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
        selectedLevel = Mathf.Clamp(selectedLevel, 1, TOTAL_LEVELS);

        foreach (Toggle toggle in levelToggles)
        {
            if (toggle != null) toggle.isOn = false;
        }

        levelToggles[selectedLevel - 1].isOn = true;

        if (toggleGroup != null)
        {
            toggleGroup.allowSwitchOff = false;
        }
    }

    /// Updates selected level based on toggle change.
    public void OnLevelToggleChanged(int levelIndex)
    {
        if (!levelToggles[levelIndex].isOn) return;

        selectedLevel = levelIndex + 1;
        Debug.Log($"Selected Level: {selectedLevel}");

        PlayerPrefs.SetInt("SelectedLevel", selectedLevel);
        PlayerPrefs.Save();
    }

    /// Starts the game, ensuring dependencies are properly initialized.
    public void StartGame()
    {
        if (!gameManager || !scoreManager || !levelManager)
        {
            Debug.LogError("Cannot start game! Some dependencies are missing.");
            return;
        }

        PlayerPrefs.SetInt("SelectedLevel", selectedLevel);
        PlayerPrefs.Save();

        Debug.Log($"Starting Game! Selected Level: {selectedLevel}");

        gameManager.StartGame();
        SceneManager.LoadScene("Game");
    }

    /// Updates high score and last score in UI.
    public void UpdateScores()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        int lastScore = PlayerPrefs.GetInt("LastScore", 0);

        if (highScoreText) highScoreText.text = $"High Score: {highScore}";
        if (lastScoreText) lastScoreText.text = $"Last Score: {lastScore}";

        Debug.Log($"Scores Loaded - Last: {lastScore}, High: {highScore}");
    }
}
