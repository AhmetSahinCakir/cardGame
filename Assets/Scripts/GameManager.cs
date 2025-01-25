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

    void Start()
    {
        // Oyunu ana menüde başlat
        ShowPanel(mainMenuPanel);
    }

    public void StartGame()
    {
        ShowPanel(gamePanel); // Sadece oyun panelini göster
        numberOfCards = levelManager.GetCardCountForCurrentLevel();
        GenerateCards(numberOfCards);
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
        // İki kart seçildikten sonra 1 saniye bekle
        yield return new WaitForSeconds(1f);

        if (card1.cardValue == card2.cardValue)
        {
            // Doğru eşleşme: Puan ekle
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

        // Eğer eşleşmeler tamamlandıysa seviye tamamlama panelini göster
        if (IsLevelComplete())
        {
            OnLevelComplete();
        }
    }

    public void OnLevelComplete()
    {
        ShowPanel(levelCompletePanel); // Sadece seviye tamamlama panelini göster
        if (levelCompletePanel != null)
        {
            // Kazanılan puanı ve yüksek skoru göster
            scoreText.text = $"Score: {scoreManager.currentScore}";
            highScoreText.text = $"High Score: {scoreManager.highScore}";
        }
    }

    public void ReturnToMainMenu()
    {
        ShowPanel(mainMenuPanel); // Sadece ana menü panelini göster
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
}
