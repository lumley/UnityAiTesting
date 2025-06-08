using Lumley.AiTest.GameShared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lumley.AiTest.MainMenu
{
    public class ResultScreenController : MonoBehaviour
{
    [Header("Result UI")]
    public TextMeshProUGUI resultTitle;
    public TextMeshProUGUI gameTypeText;
    public TextMeshProUGUI difficultyText;
    public TextMeshProUGUI timeText;
    
    [Header("Buttons")]
    public Button playAgainButton;
    public Button nextDifficultyButton;
    public Button menuButton;
    
    private void Start()
    {
        SetupUI();
        SetupButtons();
    }
    
    private void SetupUI()
    {
        // Get result from GameManager
        bool won = true; // This would come from the game result
        
        resultTitle.text = won ? "VICTORY!" : "GAME OVER";
        resultTitle.color = won ? Color.green : Color.red;
        
        gameTypeText.text = $"Game: {GameManager.Instance.CurrentMiniGame}";
        difficultyText.text = $"Difficulty: {GameManager.Instance.CurrentDifficulty}";
        
        // Enable next difficulty button only if won and not on impossible
        nextDifficultyButton.interactable = won && 
            GameManager.Instance.CurrentDifficulty != GameManager.Difficulty.Impossible;
    }
    
    private void SetupButtons()
    {
        playAgainButton.onClick.AddListener(() => GameManager.Instance.RestartCurrentGame());
        menuButton.onClick.AddListener(() => GameManager.Instance.ReturnToMenu());
        
        nextDifficultyButton.onClick.AddListener(() => {
            GameManager.Difficulty nextDiff = GetNextDifficulty();
            GameManager.Instance.StartMiniGame(GameManager.Instance.CurrentMiniGame, nextDiff);
        });
    }
    
    private GameManager.Difficulty GetNextDifficulty()
    {
        return GameManager.Instance.CurrentDifficulty switch
        {
            GameManager.Difficulty.Easy => GameManager.Difficulty.Medium,
            GameManager.Difficulty.Medium => GameManager.Difficulty.Hard,
            GameManager.Difficulty.Hard => GameManager.Difficulty.Impossible,
            _ => GameManager.Difficulty.Easy
        };
    }
}
}