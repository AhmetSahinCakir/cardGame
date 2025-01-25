using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public Toggle[] levelToggles; // Toggle grubu (Level1, Level2, ...)
    public TextMeshProUGUI highScoreText; // High Score gösterimi
    public TextMeshProUGUI lastScoreText; // Son skor gösterimi

    private int selectedLevel = 1; // Varsayılan olarak 1. seviye

    void Start()
    {
        // High Score ve Last Score bilgilerini göster
        highScoreText.text = $"High Score: {PlayerPrefs.GetInt("HighScore", 0)}";
        lastScoreText.text = $"Last Score: {PlayerPrefs.GetInt("LastScore", 0)}";

        // Varsayılan olarak ilk toggle'ı seçili yap
        levelToggles[0].isOn = true;
        selectedLevel = 1;
    }

    public void OnLevelToggleChanged(int levelIndex)
    {
        // Seçilen level'i güncelle
        selectedLevel = levelIndex + 1; // 0 tabanlı index + 1
        Debug.Log($"Selected Level: {selectedLevel}");
    }

    public void StartGame()
    {
        // Seçilen level'i kaydet ve oyun sahnesine geç
        PlayerPrefs.SetInt("SelectedLevel", selectedLevel);
        SceneManager.LoadScene("Game"); // Game sahnesine geç
    }
}
