using System;
using Lumley.AiTest.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lumley.AiTest.GameShared
{
    [Obsolete("Merge what's useful into BaseGameController and remove this class")]
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

        public GameState CurrentState { get; private set; }
        public MiniGameType CurrentMiniGame { get; private set; }
        public GameDifficulty CurrentDifficulty { get; private set; }

        public event Action<GameState> OnStateChanged;
        public event Action<bool> OnGameCompleted; // true = win, false = fail

        public async void StartMiniGame(MiniGameType gameType, GameDifficulty difficulty)
        {
            CurrentMiniGame = gameType;
            CurrentDifficulty = difficulty;

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
    }
}