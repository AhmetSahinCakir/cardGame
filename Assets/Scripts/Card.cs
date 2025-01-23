using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public int cardValue; // Kartın eşleşme değeri
    private bool isFlipped = false; // Kartın çevrilip çevrilmediğini kontrol eder

    public void SetValue(int value)
    {
        cardValue = value;
    }

    public void OnCardClicked()
    {
        if (isFlipped) return; // Zaten çevrildiyse bir şey yapma

        isFlipped = true;
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
        isFlipped = false; // Kartı yeniden çevrilir hale getir
        Debug.Log($"Card {cardValue} reset.");
    }

}
