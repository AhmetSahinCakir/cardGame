using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private LevelManager levelManager;

    [Header("Card Setup")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform gridPanel;
    [SerializeField] private List<Sprite> cardFrontSprites;
    [SerializeField] private Sprite cardBackSprite;

    [Header("UI Panels")]
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private TextMeshProUGUI gameScoreText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI matchesText;
    [SerializeField] private TextMeshProUGUI turnsText;

    private int numberOfCards;
    private List<Card> selectedCards = new List<Card>();
    private List<int> shuffledValues = new List<int>();
    private Coroutine autoResetCoroutine; // 3. karta basılmazsa resetleme için

    private void Awake()
    {
        if (gameSettings == null)
            gameSettings = Resources.Load<GameSettings>("GameSettings");

        if (scoreManager == null)
            scoreManager = FindObjectOfType<ScoreManager>();

        if (levelManager == null)
            levelManager = FindObjectOfType<LevelManager>();

        if (gameSettings == null || scoreManager == null || levelManager == null || cardBackSprite == null)
        {
            Debug.LogError("GameManager dependencies are missing! Ensure all managers and Card Back Sprite exist in the scene.");
        }
    }

    public void StartGame()
    {
        if (gameSettings == null || scoreManager == null || levelManager == null || cardBackSprite == null)
        {
            Debug.LogError("Cannot start game! Missing dependencies.");
            return;
        }

        ShowPanel(gamePanel);
        int selectedLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
        levelManager.SetCurrentLevel(selectedLevel);
        numberOfCards = levelManager.GetCardCountForCurrentLevel();

        scoreManager.ResetScore();
        UpdateGameUI();

        StartCoroutine(DelayedGenerateCards());
    }

    public void Initialize(GameSettings settings, ScoreManager scoreMgr, LevelManager levelMgr)
    {
        gameSettings = settings;
        scoreManager = scoreMgr;
        levelManager = levelMgr;

        if (gameSettings == null || scoreManager == null || levelManager == null)
        {
            Debug.LogError("GameManager dependencies are NOT set correctly!");
        }
        else
        {
            Debug.Log("GameManager dependencies successfully initialized.");
        }
    }


    private IEnumerator DelayedGenerateCards()
    {
        yield return new WaitForEndOfFrame();
        GenerateCards(numberOfCards);
    }

    private void GenerateCards(int cardCount)
    {
        if (gridPanel == null)
        {
            Debug.LogError("GridPanel is not assigned!");
            return;
        }

        if (cardPrefab == null)
        {
            Debug.LogError("Card Prefab is missing!");
            return;
        }

        if (cardFrontSprites == null || cardFrontSprites.Count == 0)
        {
            Debug.LogError("Card Front Sprites are not assigned or empty!");
            return;
        }

        if (cardBackSprite == null)
        {
            Debug.LogError("CardBack sprite is missing! Assign a back image.");
            return;
        }

        // Önceki kartları temizle
        foreach (Transform child in gridPanel)
        {
            Destroy(child.gameObject);
        }

        shuffledValues.Clear();
        for (int i = 0; i < cardCount / 2; i++)
        {
            shuffledValues.Add(i);
            shuffledValues.Add(i);
        }
        Shuffle(shuffledValues);

        int columns = Mathf.CeilToInt(Mathf.Sqrt(cardCount));
        int rows = Mathf.CeilToInt((float)cardCount / columns);

        GridLayoutGroup gridLayout = gridPanel.GetComponent<GridLayoutGroup>();
        if (!gridLayout)
        {
            Debug.LogError("GridLayoutGroup component is missing on GridPanel!");
            return;
        }

        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;

        float panelWidth = gridPanel.GetComponent<RectTransform>().rect.width;
        float panelHeight = gridPanel.GetComponent<RectTransform>().rect.height;

        float cardWidth = (panelWidth - ((columns - 1) * gridLayout.spacing.x)) / columns;
        float cardHeight = (panelHeight - ((rows - 1) * gridLayout.spacing.y)) / rows;

        gridLayout.cellSize = new Vector2(cardWidth, cardHeight);

        for (int i = 0; i < cardCount; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, gridPanel);
            Card card = newCard.GetComponent<Card>();

            if (card != null)
            {
                int cardValue = shuffledValues[i];
                card.Initialize(this, cardFrontSprites[cardValue], cardBackSprite);
                card.SetValue(cardValue, cardFrontSprites[cardValue]);
                card.ResetCard();
            }
            else
            {
                Debug.LogError("Card component is missing on instantiated prefab!");
            }
        }

        Debug.Log($"Cards Generated: {cardCount}, Grid Size: {columns}x{rows}, Card Size: ({cardWidth:F2}, {cardHeight:F2})");
    }



    private void Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void SelectCard(Card card)
    {
        if (selectedCards.Contains(card)) return;

        if (selectedCards.Count == 2)
        {
            // Eğer 3. karta tıklanırsa, ilk iki kartı hemen kapat
            ResetSelectedCards();
        }

        selectedCards.Add(card);

        if (selectedCards.Count == 2)
        {
            // Eşleşme kontrolü başlat
            StartCoroutine(CheckMatch(selectedCards[0], selectedCards[1]));

            // Eğer 3. kart tıklanmazsa, belirli bir süre sonra kapanmaları için coroutine başlat
            if (autoResetCoroutine != null)
            {
                StopCoroutine(autoResetCoroutine); // Önceki bekleme sürecini iptal et
            }
            autoResetCoroutine = StartCoroutine(AutoResetCards());
        }
    }

    private IEnumerator AutoResetCards()
    {
        yield return new WaitForSeconds(1.5f); // 1.5 saniye bekle

        if (selectedCards.Count == 2)
        {
            ResetSelectedCards();
        }
    }

    private void ResetSelectedCards()
    {
        foreach (var card in selectedCards)
        {
            card.ResetCard(); // Kartları geri çevir
        }
        selectedCards.Clear();
    }

    private IEnumerator CheckMatch(Card card1, Card card2)
    {
        scoreManager.IncrementTurns();
        yield return new WaitForSeconds(0.5f);

        if (card1.cardValue == card2.cardValue)
        {
            // Kartlar eşleşti, açık bırak
            scoreManager.IncrementMatches();
            scoreManager.AddScore(gameSettings.PointsPerMatch);
            selectedCards.Clear(); // Kartları listeye almadan bırak
        }
        else
        {
            // Kartlar eşleşmedi, geri çevir
            scoreManager.SubtractScore(3);
            ResetSelectedCards();
        }

        UpdateGameUI();

        if (IsLevelComplete())
        {
            OnLevelComplete();
        }
    }

    private bool IsLevelComplete()
    {
        foreach (Transform child in gridPanel)
        {
            Card card = child.GetComponent<Card>();
            if (card && !card.IsFlipped())
            {
                return false;
            }
        }
        return true;
    }

    private void OnLevelComplete()
    {
        ShowPanel(levelCompletePanel);

        if (finalScoreText) finalScoreText.text = $"Final Score: {scoreManager.CurrentScore}";
        if (highScoreText) highScoreText.text = $"High Score: {scoreManager.HighScore}";

        PlayerPrefs.SetInt("LastScore", scoreManager.CurrentScore);
        if (scoreManager.CurrentScore > scoreManager.HighScore)
        {
            PlayerPrefs.SetInt("HighScore", scoreManager.CurrentScore);
        }
        PlayerPrefs.Save();

        Debug.Log("Level Completed! Scores updated.");
    }


    public void ReturnToMainMenu()
    {
        ShowPanel(mainMenuPanel);

        MainMenuManager mainMenuManager = mainMenuPanel.GetComponent<MainMenuManager>();
        if (mainMenuManager)
        {
            mainMenuManager.UpdateScores();
        }
    }


    private void ShowPanel(GameObject activePanel)
    {
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (gamePanel) gamePanel.SetActive(false);
        if (levelCompletePanel) levelCompletePanel.SetActive(false);

        if (activePanel) activePanel.SetActive(true);
    }

    private void UpdateGameUI()
    {
        if (matchesText) matchesText.text = $"Matches: {scoreManager.Matches}";
        if (turnsText) turnsText.text = $"Turns: {scoreManager.Turns}";
        if (gameScoreText) gameScoreText.text = $"Score: {scoreManager.CurrentScore}";
    }
}
