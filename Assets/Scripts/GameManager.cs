using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameSettings gameSettings; 
    public ScoreManager scoreManager;
    public LevelManager levelManager; // LevelManager referansı

    // Kart prefab'i
    public GameObject cardPrefab;

    // Kartların yerleşeceği panel
    public Transform gridPanel;

    // Kart sayısı (oyun zorluğuna göre ayarlanabilir)
    private int numberOfCards;

    // Seçilen kartları tutan liste
    private List<Card> selectedCards = new List<Card>();

    void Start()
    {
        // Mevcut seviyeye göre kart sayısını LevelManager'dan alıyoruz
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

        // Yeni kartları oluştur
        for (int i = 0; i < cardCount; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, gridPanel);
            newCard.GetComponent<Card>().SetValue(i / 2); // Eşleşme için çift değerler
        }
    }

    public void SelectCard(Card card)
    {
        if (selectedCards.Contains(card)) return; // Eğer kart zaten seçilmişse bir şey yapma

        selectedCards.Add(card);

        if (selectedCards.Count == 2)
        {
            CheckMatch(selectedCards[0], selectedCards[1]);
            selectedCards.Clear(); // Karşılaştırma sonrası listeyi temizle

            // Eğer eşleşmeler tamamlandıysa, yeni seviyeye geç
            if (IsLevelComplete())
            {
                OnLevelComplete();
            }
        }
    }

    public void CheckMatch(Card card1, Card card2)
    {
        if (card1.cardValue == card2.cardValue)
        {
            // Puan ekle
            scoreManager.AddScore(gameSettings.pointsPerMatch);
            Debug.Log("Match Found! Score Updated.");
        }
        else
        {
            Debug.Log("No Match.");
            // Yanlış eşleşme durumunda kartları yeniden çevrilebilir yapın
            card1.ResetCard();
            card2.ResetCard();
        }
    }

    public void OnLevelComplete()
    {
        // Seviye ilerlet ve yeni kartları oluştur
        levelManager.NextLevel();
        numberOfCards = levelManager.GetCardCountForCurrentLevel();
        GenerateCards(numberOfCards);

        Debug.Log($"Level Complete! Proceeding to Level {levelManager.currentLevel}");
    }

    private bool IsLevelComplete()
    {
        // GridPanel'deki tüm kartların açık olup olmadığını kontrol et
        foreach (Transform child in gridPanel)
        {
            Card card = child.GetComponent<Card>();
            if (card != null && !card.IsFlipped())
            {
                return false; // Bir kart hala kapalıysa seviye tamamlanmamış demektir
            }
        }
        return true;
    }
}
