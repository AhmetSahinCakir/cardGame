using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameSettings gameSettings;
    public ScoreManager scoreManager;
    public LevelManager levelManager;

    public GameObject cardPrefab;
    public Transform gridPanel;
    public List<Sprite> cardFrontSprites;

    private int numberOfCards;
    private List<Card> selectedCards = new List<Card>();
    private List<int> shuffledValues = new List<int>();

    public GameObject levelCompletePanel;
    public GameObject mainMenuPanel;
    public GameObject gamePanel;
    public TextMeshProUGUI gameScoreText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI matchesText;
    public TextMeshProUGUI turnsText;

    void Awake()
    {
        if (gameSettings == null)
        {
            Debug.LogWarning("GameSettings is missing, loading manually...");
            gameSettings = Resources.Load<GameSettings>("GameSettings");
        }
    }

    void Start()
    {
        scoreManager.OnScoreChanged += UpdateGameUI;
        ShowPanel(mainMenuPanel);
    }

    public void StartGame()
    {
        ShowPanel(gamePanel);

        int selectedLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
        levelManager.SetCurrentLevel(selectedLevel);
        numberOfCards = levelManager.GetCardCountForCurrentLevel();

        scoreManager.ResetScore();
        UpdateGameUI();

        StartCoroutine(DelayedGenerateCards());

        Debug.Log($"Game Started! Level: {selectedLevel}, Card Count: {numberOfCards}");
    }

    private IEnumerator DelayedGenerateCards()
    {
        yield return null;
        yield return new WaitForEndOfFrame();
        GenerateCards(numberOfCards);
    }

    void GenerateCards(int cardCount)
    {
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

        RectTransform gridRect = gridPanel.GetComponent<RectTransform>();
        float panelWidth = gridRect.rect.width;
        float panelHeight = gridRect.rect.height;

        int columns = Mathf.CeilToInt(Mathf.Sqrt(cardCount));
        int rows = Mathf.CeilToInt((float)cardCount / columns);

        GridLayoutGroup gridLayout = gridPanel.GetComponent<GridLayoutGroup>();
        if (gridLayout == null)
        {
            Debug.LogError("GridLayoutGroup component is missing on GridPanel!");
            return;
        }

        // Kenar Boşlukları
        float paddingX = 40f; // Sol ve sağ toplam boşluk
        float paddingY = 40f; // Üst ve alt toplam boşluk

        float availableWidth = panelWidth - paddingX;
        float availableHeight = panelHeight - paddingY;

        float cardWidth = (availableWidth - ((columns - 1) * gridLayout.spacing.x)) / columns;
        float cardHeight = (availableHeight - ((rows - 1) * gridLayout.spacing.y)) / rows;

        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;
        gridLayout.cellSize = new Vector2(cardWidth, cardHeight);

        for (int i = 0; i < cardCount; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, gridPanel);
            Card card = newCard.GetComponent<Card>();

            int cardValue = shuffledValues[i];
            card.SetValue(cardValue, cardFrontSprites[cardValue]);
        }

        Debug.Log($"Cards Generated: {cardCount}, Grid Size: {columns}x{rows}, Card Size: ({cardWidth:F2}, {cardHeight:F2})");
    }

    public void SelectCard(Card card)
    {
        if (selectedCards.Contains(card)) return;
        
        selectedCards.Add(card);

        if (selectedCards.Count == 2)
        {
            StartCoroutine(CheckMatch(selectedCards[0], selectedCards[1]));
        }
    }

    private IEnumerator CheckMatch(Card card1, Card card2)
    {
        scoreManager.IncrementTurns();
        yield return new WaitForSeconds(0.5f);

        if (card1.cardValue == card2.cardValue)
        {
            scoreManager.IncrementMatches();
            scoreManager.AddScore(gameSettings.pointsPerMatch);
        }
        else
        {
            scoreManager.SubtractScore(3);
            card1.ResetCard();
            card2.ResetCard();
        }
        
        selectedCards.Clear();
        UpdateGameUI();

        if (IsLevelComplete())
        {
            OnLevelComplete();
        }
    }

    public void OnLevelComplete()
    {
        ShowPanel(levelCompletePanel);
        
        if (finalScoreText != null)
        {
            finalScoreText.text = $"Final Score: {scoreManager.currentScore}";
        }
        if (highScoreText != null)
        {
            highScoreText.text = $"High Score: {scoreManager.highScore}";
        }
        
        PlayerPrefs.SetInt("LastScore", scoreManager.currentScore);
        if (scoreManager.currentScore > scoreManager.highScore)
        {
            PlayerPrefs.SetInt("HighScore", scoreManager.currentScore);
        }
        PlayerPrefs.Save();
    }

    public void ReturnToMainMenu()
    {
        ShowPanel(mainMenuPanel);
        MainMenuManager mainMenuManager = mainMenuPanel.GetComponent<MainMenuManager>();
        if (mainMenuManager != null)
        {
            mainMenuManager.UpdateScores();
        }
    }

    private bool IsLevelComplete()
    {
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
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);

        if (activePanel != null) activePanel.SetActive(true);
    }

    private void UpdateGameUI()
    {
        if (matchesText != null)
        {
            matchesText.text = $"Matches: {scoreManager.matches}";
        }

        if (turnsText != null)
        {
            turnsText.text = $"Turns: {scoreManager.turns}";
        }
        
        if (gameScoreText != null)
        {
            gameScoreText.text = $"Score: {scoreManager.currentScore}";
        }
    }
}
