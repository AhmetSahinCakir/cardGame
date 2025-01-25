using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int currentScore = 0; // Şu anki skor
    public int highScore = 0; // En yüksek skor
    public int lastScore = 0; // Son oynanan skor
    public int matches = 0; // Toplam doğru eşleşme sayısı
    public int turns = 0;   // Toplam deneme sayısı

    public void AddScore(int points)
    {
        currentScore += points;

        if (currentScore > highScore)
        {
            highScore = currentScore; // Eğer yeni puan, yüksek skordan büyükse highScore güncellenir
        }

        Debug.Log($"Current Score: {currentScore}, High Score: {highScore}");
    }

    public void SubtractScore(int points)
    {
        currentScore -= points;

        // Puan 0'ın altına düşemez
        if (currentScore < 0)
        {
            currentScore = 0;
        }

        Debug.Log($"Points deducted. Current Score: {currentScore}");
    }

    public void IncrementMatches()
    {
        matches++;
    }

    public void IncrementTurns()
    {
        turns++;
    }

    public void ResetScore()
    {
        // Oyun sonunda Last Score'u güncelle
        lastScore = currentScore;

        // Mevcut skoru sıfırla
        currentScore = 0;
        matches = 0;
        turns = 0;
    }
}

