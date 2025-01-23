using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "CardMatch/GameSettings")]
public class GameSettings : ScriptableObject
{
    public int startingCardCount = 4; // İlk seviyede kaç kart olacak
    public int cardIncreasePerLevel = 2; // Her seviyede kaç kart artacak
    public int maxCardCount = 20; // Maksimum kart sayısı
    public int pointsPerMatch = 10; // Eşleşme başına kazanılacak puan
}

