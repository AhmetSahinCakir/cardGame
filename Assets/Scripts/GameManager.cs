using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameSettings gameSettings;
    public ScoreManager scoreManager;
    public LevelManager levelManager;

    // Card prefab
    public GameObject cardPrefab;

    // Panel for card placement
    public Transform gridPanel;

    // Card images
    public List<Sprite> cardFrontSprites;

    // Number of cards in the game
    private int numberOfCards;

    // Selected cards
    private List<Card> selectedCards = new List<Card>();
    private List<int> shuffledValues = new List<int>();

    // UI Panels
    public GameObject levelCompletePanel;
    public GameObject mainMenuPanel;
    public GameObject gamePanel; // Game screen panel
    public TextMeshProUGUI gameScoreText;  // Score displayed during the game
    public TextMeshProUGUI finalScoreText; // Score displayed at the end
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
        Debug.Log($"GameSettings Loaded: Initial Card Count = {gameSettings.startingCardCount}, Max Cards = {gameSettings.maxCardCount}");
    }

    void Start()
    {
        scoreManager.OnScoreChanged += UpdateGameUI;
        ShowPanel(mainMenuPanel); // Start at the main menu
        numberOfCards = levelManager.GetCardCountForCurrentLevel();
        Debug.Log($"Starting Game with {numberOfCards} cards");
        scoreManager.ResetScore();
        UpdateGameUI();
        GenerateCards(numberOfCards);
    }

    public void StartGame()
    {
        ShowPanel(gamePanel);

        // Load selected level from PlayerPrefs
        int selectedLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
        Debug.Log($"Selected Level Loaded: {selectedLevel}");

        // Update LevelManager with selected level
        levelManager.SetCurrentLevel(selectedLevel);
        
        // Get updated card count based on the level
        numberOfCards = levelManager.GetCardCountForCurrentLevel();
        
        scoreManager.ResetScore();
        UpdateGameUI();
        GenerateCards(numberOfCards);
        
        Debug.Log($"Game Started! Level: {selectedLevel}, Card Count: {numberOfCards}");
    }

    void GenerateCards(int cardCount)
    {
        // Clear previous cards
        foreach (Transform child in gridPanel)
        {
            Destroy(child.gameObject);
        }

        shuffledValues.Clear();

        // Generate and shuffle card values
        for (int i = 0; i < cardCount / 2; i++)
        {
            shuffledValues.Add(i);
            shuffledValues.Add(i);
        }
        Shuffle(shuffledValues);

        // Instantiate new cards
        for (int i = 0; i < cardCount; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, gridPanel);
            Card card = newCard.GetComponent<Card>();

            int cardValue = shuffledValues[i];
            card.SetValue(cardValue, cardFrontSprites[cardValue]);
        }
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
            Debug.Log("Match Found! Score Updated.");
        }
        else
        {
            scoreManager.SubtractScore(3);
            Debug.Log("No Match. Points deducted.");
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

    public void ExitToMainMenu()
    {
        Debug.Log("Returning to Main Menu without saving progress...");

        // Reset the game state WITHOUT saving progress
        scoreManager.ResetScore(false); // Prevents saving the current score

        // Clear the game board to remove any active cards
        ClearGameBoard();

        // Ensure the UI updates properly
        UpdateGameUI();

        // Show the Main Menu panel
        ShowPanel(mainMenuPanel);
    }

    private void ClearGameBoard()
    {
        foreach (Transform child in gridPanel)
        {
            Destroy(child.gameObject); // Clears all cards from the board
        }
        selectedCards.Clear(); // Ensure selected cards list is emptied
        shuffledValues.Clear(); // Clear the shuffled values list
    }



}
