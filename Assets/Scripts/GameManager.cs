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

    // Kart görselleri
    public List<Sprite> cardFrontSprites; // Ön yüz görselleri

    // Kart sayısı (oyun zorluğuna göre ayarlanabilir)
    private int numberOfCards;

    // Seçilen kartları tutan liste
    private List<Card> selectedCards = new List<Card>();

    private List<int> shuffledValues = new List<int>(); // Karıştırılmış eşleşme değerleri

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

        shuffledValues.Clear();
        // Kart eşleşme değerlerini oluştur ve karıştır
        for (int i = 0; i < cardCount / 2; i++)
        {
            shuffledValues.Add(i);
            shuffledValues.Add(i); // Her değerden iki tane ekle (eşleşme için)
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
        if (selectedCards.Contains(card)) return; // Eğer kart zaten seçilmişse bir şey yapma

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
            // Puan ekle
            scoreManager.AddScore(gameSettings.pointsPerMatch);
            Debug.Log("Match Found! Score Updated.");
        }
        else
        {
            Debug.Log("No Match.");
            // Yanlış eşleşme durumunda kartları yeniden çevrilir hale getir
            card1.ResetCard();
            card2.ResetCard();
        }

        // Seçilen kartları temizle
        selectedCards.Clear();

        // Eğer eşleşmeler tamamlandıysa yeni seviyeye geç
        if (IsLevelComplete())
        {
            OnLevelComplete();
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

    // Listeyi karıştırmak için basit bir fonksiyon
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
}
