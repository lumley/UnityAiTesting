using Lumley.AiTest.GameShared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lumley.AiTest.MainMenu
{
    public class GameUIController : MonoBehaviour
    {
        [Header("UI Elements")] public TextMeshProUGUI scoreText;
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI objectiveText;
        public Slider progressSlider;

        [Header("Control Buttons")] public Button pauseButton;
        public Button restartButton;
        public Button menuButton;

        [Header("Feedback")] public GameObject winFeedback;
        public GameObject loseFeedback;
        public TextMeshProUGUI feedbackText;

        private void Start()
        {
            SetupButtons();
            GameManager.Instance.OnGameCompleted += ShowFeedback;
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnGameCompleted -= ShowFeedback;
        }

        private void SetupButtons()
        {
            pauseButton?.onClick.AddListener(() => GameManager.Instance.SetState(GameManager.GameState.Paused));
            restartButton?.onClick.AddListener(() => GameManager.Instance.RestartCurrentGame());
            menuButton?.onClick.AddListener(() => GameManager.Instance.ReturnToMenu());
        }

        public void UpdateScore(int score)
        {
            if (scoreText != null)
                scoreText.text = $"Score: {score}";
        }

        public void UpdateTimer(float time)
        {
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(time / 60);
                int seconds = Mathf.FloorToInt(time % 60);
                timerText.text = $"{minutes:00}:{seconds:00}";
            }
        }

        public void UpdateObjective(string objective)
        {
            if (objectiveText != null)
                objectiveText.text = objective;
        }

        public void UpdateProgress(float progress)
        {
            if (progressSlider != null)
                progressSlider.value = progress;
        }

        private void ShowFeedback(bool won)
        {
            GameObject feedbackPanel = won ? winFeedback : loseFeedback;
            feedbackPanel?.SetActive(true);

            if (feedbackText != null)
                feedbackText.text = won ? "Victory!" : "Game Over";
        }
    }
}