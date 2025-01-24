using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int cardValue; // Kartın eşleşme değeri
    private bool isFlipped = false; // Kartın çevrilip çevrilmediğini kontrol eder
    public Sprite frontSprite; // Ön yüz görseli
    public Sprite backSprite; // Arka yüz görseli
    private Image cardImage; // Kartın görselini göstermek için Image bileşeni

    void Awake()
    {
        cardImage = GetComponent<Image>();
        ResetCard(); // Başlangıçta kartı arka yüz olarak ayarla
    }

    public void SetValue(int value, Sprite frontImage)
    {
        cardValue = value;
        frontSprite = frontImage; // Ön yüz görselini ata
    }

    public void OnCardClicked()
    {
        if (isFlipped) return; // Zaten çevrildiyse bir şey yapma

        isFlipped = true;
        cardImage.sprite = frontSprite; // Kartın ön yüzünü göster

        Debug.Log($"Card {cardValue} clicked!");

        // GameManager'a bildirerek seçilen kartı gönder
        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.SelectCard(this);
    }

    public bool IsFlipped()
    {
        return isFlipped;
    }

    public void ResetCard()
    {
        isFlipped = false;
        cardImage.sprite = backSprite; // Kartı arka yüze çevir
        Debug.Log($"Card {cardValue} reset.");
    }
}
