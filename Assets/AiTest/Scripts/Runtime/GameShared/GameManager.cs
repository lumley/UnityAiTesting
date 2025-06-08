using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lumley.AiTest.GameShared
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game Configuration")] public GameConfig gameConfig;

        public enum GameState
        {
            Menu,
            Playing,
            Paused,
            GameOver,
            Result
        }

        public enum MiniGameType
        {
            Tetris,
            Woodoku,
            BlockJam,
            ColorSort
        }

        public enum Difficulty
        {
            Easy,
            Medium,
            Hard,
            Impossible
        }

        public GameState CurrentState { get; private set; }
        public MiniGameType CurrentMiniGame { get; private set; }
        public Difficulty CurrentDifficulty { get; private set; }

        public event Action<GameState> OnStateChanged;
        public event Action<bool> OnGameCompleted; // true = win, false = fail

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void StartMiniGame(MiniGameType gameType, Difficulty difficulty)
        {
            CurrentMiniGame = gameType;
            CurrentDifficulty = difficulty;
            SetState(GameState.Playing);

            string sceneName = GetSceneName(gameType);
            SceneManager.LoadScene(sceneName);
        }

        public void SetState(GameState newState)
        {
            CurrentState = newState;
            OnStateChanged?.Invoke(newState);
        }

        public void CompleteGame(bool won)
        {
            SetState(GameState.Result);
            OnGameCompleted?.Invoke(won);
        }

        public void RestartCurrentGame()
        {
            StartMiniGame(CurrentMiniGame, CurrentDifficulty);
        }

        public void ReturnToMenu()
        {
            SetState(GameState.Menu);
            SceneManager.LoadScene("MainMenu");
        }

        private string GetSceneName(MiniGameType gameType)
        {
            return gameType switch
            {
                MiniGameType.Tetris => "TetrisScene",
                MiniGameType.Woodoku => "WoodokuScene",
                MiniGameType.BlockJam => "BlockJamScene",
                MiniGameType.ColorSort => "ColorSortScene",
                _ => "MainMenu"
            };
        }
    }
}