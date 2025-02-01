using NUnit.Framework;
using UnityEngine;

public class CardTests
{
    private Card card;

    [SetUp]
    public void Setup()
    {
        PlayerPrefs.DeleteAll(); 
        PlayerPrefs.Save();
        
        var gameObject = new GameObject();
        card = gameObject.AddComponent<Card>();
        var gameManager = new GameObject().AddComponent<GameManager>();
        card.Initialize(gameManager, null, null);
    }

    [Test]
    public void Card_InitializesCorrectly()
    {
        card.SetValue(1, null);
        Assert.AreEqual(1, card.cardValue);
    }

    [Test]
    public void Card_FlipsCorrectly()
    {
        card.OnCardClicked();
        Assert.IsTrue(card.IsFlipped(), "Card should be flipped after being clicked.");
    }


    [Test]
    public void Card_ResetsCorrectly()
    {
        card.OnCardClicked();
        card.ResetCard();
        Assert.IsFalse(card.IsFlipped(), "Card should be reset to unflipped state.");
    }
}
