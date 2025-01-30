using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [Header("Card Settings")]
    public int cardValue;
    private bool isFlipped = false;

    [Header("Card Sprites")]
    private Sprite frontSprite;
    private Sprite backSprite;

    private Image cardImage;
    private GameManager gameManager;
    private Button button;

    /// Initializes the card with its dependencies.
    public void Initialize(GameManager manager, Sprite frontFace, Sprite backFace)
    {
        gameManager = manager;
        frontSprite = frontFace;
        backSprite = backFace;

        ResetCard(); // Başlangıçta kartı kapalı hale getir
    }

    private void Awake()
    {
        cardImage = GetComponent<Image>();
        button = GetComponent<Button>();

        if (button == null)
        {
            Debug.LogError("Card is missing a Button component! Add it in Unity.");
            return;
        }

        button.onClick.AddListener(OnCardClicked);
        ResetCard();
    }

    /// Assigns the value and front sprite to the card.
    public void SetValue(int value, Sprite frontImage)
    {
        cardValue = value;
        frontSprite = frontImage;
    }

    /// Handles card click event.
    public void OnCardClicked()
    {
        if (isFlipped || gameManager == null) return;

        isFlipped = true;
        button.interactable = false; // Kart açıldıktan sonra tıklanamaz hale getir

        // LeanTween ile döndürme animasyonu
        LeanTween.rotateY(gameObject, 90f, 0.15f).setOnComplete(() =>
        {
            cardImage.sprite = frontSprite; // Kartın ön yüzünü göster
            LeanTween.rotateY(gameObject, 0f, 0.15f); // Eski pozisyonuna geri döndür
        });

        Debug.Log($"Card {cardValue} clicked!");
        gameManager.SelectCard(this);
    }


    /// Returns whether the card is flipped.
    public bool IsFlipped() => isFlipped;

    /// Resets the card to its back face.
    public void ResetCard()
    {
        isFlipped = false;
        button.interactable = true;

        // LeanTween ile kapanma animasyonu
        LeanTween.rotateY(gameObject, 90f, 0.15f).setOnComplete(() =>
        {
            cardImage.sprite = backSprite; // Kartın arka yüzünü göster
            LeanTween.rotateY(gameObject, 0f, 0.15f); // Kartı geri döndür
        });
    }

}
