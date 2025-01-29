using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public Toggle[] levelToggles; // 6 adet toggle
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI lastScoreText;

    private const int TOTAL_LEVELS = 6;
    private int selectedLevel = 1;

    void Start()
    {
        InitializeLevelSelection();
        UpdateScores();
    }

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
            if (toggle != null)
            {
                toggle.isOn = false;
            }
        }

        levelToggles[selectedLevel - 1].isOn = true;

        ToggleGroup toggleGroup = levelToggles[0].group;
        if (toggleGroup != null)
        {
            toggleGroup.allowSwitchOff = false;
        }
    }

    public void OnLevelToggleChanged(int levelIndex)
    {
        if (!levelToggles[levelIndex].isOn)
            return;

        selectedLevel = levelIndex + 1;
        Debug.Log($"Selected Level: {selectedLevel}");

        PlayerPrefs.SetInt("SelectedLevel", selectedLevel);
        PlayerPrefs.Save();
    }

    public void StartGame()
    {
        PlayerPrefs.SetInt("SelectedLevel", selectedLevel);
        PlayerPrefs.Save();

        Debug.Log($"Selected Level Saved: {selectedLevel}");

        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.StartGame();
        }

        SceneManager.LoadScene("Game");
    }

    public void UpdateScores()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        int lastScore = PlayerPrefs.GetInt("LastScore", 0);

        if (highScoreText != null)
        {
            highScoreText.text = $"High Score: {highScore}";
        }

        if (lastScoreText != null)
        {
            lastScoreText.text = $"Last Score: {lastScore}";
        }

        Debug.Log($"MainMenu: Scores Loaded - Last Score: {lastScore}, High Score: {highScore}");
    }
}
