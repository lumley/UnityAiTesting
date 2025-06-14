using UnityEngine;

namespace Lumley.AiTest.GameShared
{
    public abstract class BaseGameController : MonoBehaviour
    {
        [Header("Base Game Settings")] public GameObject winPanel;
        public GameObject losePanel;
        public GameObject pausePanel;

        protected bool isGameActive = false;
        protected float gameTimer = 0f; // TODO (slumley): When timer reaches max time, end the game (used from difficulty settongs)
        
        protected virtual void Start()
        {
            var currentGameDifficulty = Toolbox.Get<ICurrentGameInfoManager>().CurrentGameDifficulty;
            InitializeGame(currentGameDifficulty);
            GameManager.Instance.OnStateChanged += HandleStateChange;
        }

        protected virtual void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged -= HandleStateChange;
        }

        protected virtual void Update()
        {
            if (isGameActive)
            {
                gameTimer += Time.deltaTime;
                UpdateGameplay();
            }
        }

        protected abstract void InitializeGame(GameManager.Difficulty difficulty);
        protected abstract void UpdateGameplay();
        protected abstract void HandleWin();
        protected abstract void HandleLose();

        protected virtual void HandleStateChange(GameManager.GameState newState)
        {
            isGameActive = newState == GameManager.GameState.Playing;

            switch (newState)
            {
                case GameManager.GameState.Playing:
                    ResumeGame();
                    break;
                case GameManager.GameState.Paused:
                    PauseGame();
                    break;
                case GameManager.GameState.Result:
                    EndGame();
                    break;
            }
        }

        protected virtual void PauseGame()
        {
            Time.timeScale = 0f;
            pausePanel?.SetActive(true);
        }

        protected virtual void ResumeGame()
        {
            Time.timeScale = 1f;
            pausePanel?.SetActive(false);
        }

        protected virtual void EndGame()
        {
            isGameActive = false;
        }

        public virtual void RestartGame()
        {
            GameManager.Instance.RestartCurrentGame();
        }

        public virtual void ReturnToMenu()
        {
            GameManager.Instance.ReturnToMenu();
        }
    }
}