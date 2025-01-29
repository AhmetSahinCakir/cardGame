using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public Toggle[] levelToggles; // 6 adet toggle için
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
        // Toggle array kontrolü
        if (levelToggles == null || levelToggles.Length != TOTAL_LEVELS)
        {
            Debug.LogError($"Level toggles array must contain exactly {TOTAL_LEVELS} toggles!");
            return;
        }

        // Önceki seçili leveli yükle (varsayılan: 1)
        selectedLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
        
        // Seçili levelin geçerli aralıkta olduğundan emin ol
        selectedLevel = Mathf.Clamp(selectedLevel, 1, TOTAL_LEVELS);

        // Tüm toggle'ları kapat
        foreach (Toggle toggle in levelToggles)
        {
            if (toggle != null)
            {
                toggle.isOn = false;
            }
        }

        // Sadece seçili levelin toggle'ını aç
        levelToggles[selectedLevel - 1].isOn = true;

        // Toggle group'un sadece bir seçime izin verdiğinden emin ol
        ToggleGroup toggleGroup = levelToggles[0].group;
        if (toggleGroup != null)
        {
            toggleGroup.allowSwitchOff = false;
        }
    }

    public void OnLevelToggleChanged(int levelIndex)
    {
        // Toggle'ın açık olduğundan emin ol
        if (!levelToggles[levelIndex].isOn)
            return;

        selectedLevel = levelIndex + 1;
        Debug.Log($"Selected Level: {selectedLevel}");

        PlayerPrefs.SetInt("SelectedLevel", selectedLevel);
        PlayerPrefs.Save();
        Debug.Log($"Saved Selected Level: {selectedLevel}");

    }

    public void StartGame()
    {
        PlayerPrefs.SetInt("SelectedLevel", selectedLevel);
        PlayerPrefs.Save();
        
        Debug.Log($"📌 Seçili Level Kaydedildi: {selectedLevel}");

        // GameManager'a yeni level bilgisini zorla güncelle
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.StartGame();
        }

        // Oyun sahnesini yükle
        SceneManager.LoadScene("Game");
    }




    public void UpdateScores()
    {
        if (highScoreText != null)
        {
            highScoreText.text = $"High Score: {PlayerPrefs.GetInt("HighScore", 0)}";
        }
        
        if (lastScoreText != null)
        {
            lastScoreText.text = $"Last Score: {PlayerPrefs.GetInt("LastScore", 0)}";
        }
    }
}