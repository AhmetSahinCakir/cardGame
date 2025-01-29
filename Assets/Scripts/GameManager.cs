using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameSettings gameSettings; 
    public ScoreManager scoreManager;
    public LevelManager levelManager;

    // Kart prefab'i
    public GameObject cardPrefab;

    // Kartların yerleşeceği panel
    public Transform gridPanel;

    // Kart görselleri
    public List<Sprite> cardFrontSprites;

    // Kart sayısı
    private int numberOfCards;

    // Seçilen kartları tutan liste
    private List<Card> selectedCards = new List<Card>();
    private List<int> shuffledValues = new List<int>();

    // UI Panelleri
    public GameObject levelCompletePanel;
    public GameObject mainMenuPanel;
    public GameObject gamePanel; // Oyun alanı paneli
    public TextMeshProUGUI scoreText;  // Kazanılan puan metni
    public TextMeshProUGUI highScoreText; // Yüksek skor metni

    public TextMeshProUGUI matchesText; // Matches göstergesi
    public TextMeshProUGUI turnsText;   // Turns göstergesi

    void Awake()
    {
        if (gameSettings == null)
        {
            Debug.LogWarning("⚠️ GameSettings boş, manuel olarak yüklüyoruz...");
            gameSettings = Resources.Load<GameSettings>("GameSettings");
        }
        Debug.Log($"🛠️ GameSettings: Başlangıç Kart Sayısı = {gameSettings.startingCardCount}, Max Kart = {gameSettings.maxCardCount}");
    }

    void Start()
    {
        // Oyunu ana menüde başlat
        ShowPanel(mainMenuPanel);
        numberOfCards = levelManager.GetCardCountForCurrentLevel();
        Debug.Log($"Starting Game withh {numberOfCards} cards");
        scoreManager.ResetScore();
        UpdateMatchesAndTurnsUI();
        GenerateCards(numberOfCards);
        Debug.Log($"Game Started! Level: {levelManager.GetCardCountForCurrentLevel()} Cards: {numberOfCards}");

    }

    public void StartGame()
    {
        ShowPanel(gamePanel);

        // PlayerPrefs'ten en güncel leveli al
        int selectedLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
        Debug.Log($"📌 PlayerPrefs'ten Yüklenen Seçili Level: {selectedLevel}");

        // LevelManager'a yeni level bilgisini zorla güncelle
        levelManager.SetCurrentLevel(selectedLevel);

        // Güncellenmiş level bilgisiyle kart sayısını al
        numberOfCards = levelManager.GetCardCountForCurrentLevel();
        
        scoreManager.ResetScore();
        UpdateMatchesAndTurnsUI();
        GenerateCards(numberOfCards);
        
        Debug.Log($"Oyun Başladı! Güncellenmiş Level: {selectedLevel}, Kart Sayısı: {numberOfCards}");
    }

    public void UpdateLevelAndRestart()
    {
        numberOfCards = levelManager.GetCardCountForCurrentLevel();
        Debug.Log($"Yeni Level Güncellendi: {levelManager.GetCurrentLevel()}, Yeni Kart Sayısı: {numberOfCards}");

        // Mevcut kartları temizle ve yeniden oluştur
        GenerateCards(numberOfCards);
    }

    public void SetSelectedLevel(int level)
    {
        PlayerPrefs.SetInt("SelectedLevel", level);
        PlayerPrefs.Save();
        Debug.Log($"✅ GameManager Seçili Level Güncellendi: {level}");
    }


    void GenerateCards(int cardCount)
    {
        // Önceki kartları temizle
        foreach (Transform child in gridPanel)
        {
            Destroy(child.gameObject);
        }

        shuffledValues.Clear();

        // Kart eşleşme değerlerini oluştur ve karıştır
        for (int i = 0; i < cardCount / 2; i++)
        {
            shuffledValues.Add(i);
            shuffledValues.Add(i); // Her değerden iki tane ekle
        }
        Shuffle(shuffledValues);

        // Yeni kartları oluştur
        for (int i = 0; i < cardCount; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, gridPanel);
            Card card = newCard.GetComponent<Card>();

            // Kart değerini ve ön yüz görselini ata
            int cardValue = shuffledValues[i];
            card.SetValue(cardValue, cardFrontSprites[cardValue]);
        }
    }

    public void SelectCard(Card card)
    {
        if (selectedCards.Contains(card)) return;

        selectedCards.Add(card);

        if (selectedCards.Count == 2)
        {
            // İki kart seçildiğinde eşleşmeyi kontrol et
            StartCoroutine(CheckMatch(selectedCards[0], selectedCards[1]));
        }
    }

    private IEnumerator CheckMatch(Card card1, Card card2)
    {
        // Her deneme için turns'i artırın
        scoreManager.IncrementTurns();

        // İki kart seçildikten sonra 1 saniye bekle
        yield return new WaitForSeconds(0.5f);

        if (card1.cardValue == card2.cardValue)
        {
            // Doğru eşleşme: matches'i artırın ve puan ekleyin
            scoreManager.IncrementMatches();
            scoreManager.AddScore(gameSettings.pointsPerMatch);
            Debug.Log("Match Found! Score Updated.");
        }
        else
        {
            // Yanlış eşleşme: Puan düşür
            scoreManager.SubtractScore(3); // 3 puan düşür
            Debug.Log("No Match. Points deducted.");
            card1.ResetCard();
            card2.ResetCard();
        }

        // Seçilen kartları temizle
        selectedCards.Clear();
        UpdateMatchesAndTurnsUI();

        // Eğer eşleşmeler tamamlandıysa seviye tamamlama panelini göster
        if (IsLevelComplete())
        {
            OnLevelComplete();
        }
    }


    public void OnLevelComplete()
    {
        ShowPanel(levelCompletePanel); // LevelCompletePanel'i göster
        
        // Skorları güncelle
        if (levelCompletePanel != null)
        {
            scoreText.text = $"Score: {scoreManager.currentScore}";
            highScoreText.text = $"High Score: {scoreManager.highScore}";
        }
    }


    public void ReturnToMainMenu()
    {
        ShowPanel(mainMenuPanel); // Sadece ana menü panelini göster
        MainMenuManager mainMenuManager = mainMenuPanel.GetComponent<MainMenuManager>();
        if (mainMenuManager != null)
        {
            mainMenuManager.UpdateScores();
        }
    }

    private void UpdateMainMenuScores()
    {
        if (mainMenuPanel != null)
        {
            TextMeshProUGUI highScoreTextInMenu = mainMenuPanel.transform.Find("Panel/HighScore").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI lastScoreTextInMenu = mainMenuPanel.transform.Find("Panel/LastScore").GetComponent<TextMeshProUGUI>();

            if (highScoreTextInMenu != null) highScoreTextInMenu.text = $"High Score: {scoreManager.highScore}";
            if (lastScoreTextInMenu != null) lastScoreTextInMenu.text = $"Last Score: {scoreManager.lastScore}";
        }
    }

    private bool IsLevelComplete()
    {
        // GridPanel'deki tüm kartların açık olup olmadığını kontrol et
        foreach (Transform child in gridPanel)
        {
            Card card = child.GetComponent<Card>();
            if (card != null && !card.IsFlipped())
            {
                return false;
            }
        }
        return true;
    }

    void Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private void ShowPanel(GameObject activePanel)
    {
        // Tüm panelleri gizle
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);

        // Sadece aktif paneli göster
        if (activePanel != null) activePanel.SetActive(true);
    }

    private void UpdateMatchesAndTurnsUI()
    {
        if (matchesText != null)
        {
            matchesText.text = $"Matches: {scoreManager.matches}";
        }

        if (turnsText != null)
        {
            turnsText.text = $"Turns: {scoreManager.turns}";
        }
    }

}
