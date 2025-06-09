using System;
using Lumley.AiTest.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lumley.AiTest.GameShared
{
    public class GameManager : MonoBehaviour
    {
        [Obsolete("Use Toolbox instead")]
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
            Easy = 0,
            Medium = 1,
            Hard = 2,
            Impossible = 3
        }

        public GameState CurrentState { get; private set; }
        public MiniGameType CurrentMiniGame { get; private set; }
        public Difficulty CurrentDifficulty { get; private set; }

        public event Action<GameState> OnStateChanged;
        public event Action<bool> OnGameCompleted; // true = win, false = fail

        public async void StartMiniGame(MiniGameType gameType, Difficulty difficulty)
        {
            CurrentMiniGame = gameType;
            CurrentDifficulty = difficulty;

            string sceneName = GetSceneName(gameType);
            try
            {
                var sceneTransitionManager = Toolbox.Get<ISceneTransitionManager>();
                await sceneTransitionManager.TransitionToSceneAsync(null); // TODO (slumley): Get the correct reference to the scene
                SetState(GameState.Playing);
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
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