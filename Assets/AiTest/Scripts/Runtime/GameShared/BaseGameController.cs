using System;
using System.Threading.Tasks;
using Lumley.AiTest.ComponentUtilities;
using Lumley.AiTest.SceneManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Lumley.AiTest.GameShared
{
    public abstract class BaseGameController : MonoBehaviour
    {
        [Header("Base Game Settings")] private GameObject _winPanel;
        private GameObject _losePanel;
        private GameObject _pausePanel;
        
        [SerializeField]
        private TimelineController? _introductionTimeline;
        
        [SerializeField]
        private AssetReference _mainMenuScene = null!;
        
        protected float gameTimer = 0f; // TODO (slumley): When timer reaches max time, end the game (used from difficulty settongs)
        
        protected GameState State;
        
        protected async void Start()
        {
            try
            {
                State = GameState.IsInitializing;
                var currentGameInfoManager = Toolbox.Get<ICurrentGameInfoManager>();
                var currentGameDifficulty = currentGameInfoManager.CurrentGameDifficulty;
                await InitializeGameAsync(currentGameDifficulty);
                if (_introductionTimeline != null)
                {
                    await _introductionTimeline.PlayTimelineAsync();
                }
                State = GameState.IsPlaying;
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }

        protected virtual void Update()
        {
            if (State == GameState.IsPlaying)
            {
                gameTimer += Time.deltaTime;
                UpdateGameplay();
            }
        }

        protected abstract Task InitializeGameAsync(GameDifficulty difficulty);
        protected abstract void UpdateGameplay();
        protected abstract void HandleWin();
        protected abstract void HandleLose();

        protected void PauseGame()
        {
            State = GameState.IsPaused;
            _pausePanel.SetActive(true);
        }

        protected void ResumeGame()
        {
            State = GameState.IsPlaying;
            _pausePanel.SetActive(false);
        }

        protected void EndGame()
        {
            State = GameState.IsGameFinished;
        }

        protected async void RestartGame()
        {
            try
            {
                State = GameState.IsPaused;
                var currentGameInfoManager = Toolbox.Get<ICurrentGameInfoManager>();
                AssetReference currentGameAsset = _mainMenuScene;
                if (currentGameInfoManager.CurrentGameAsset != null)
                {
                    currentGameAsset = currentGameInfoManager.CurrentGameAsset;
                }
                var sceneTransitionManager = Toolbox.Get<ISceneTransitionManager>();
                await sceneTransitionManager.TransitionToSceneAsync(currentGameAsset);
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }
        
        protected enum GameState
        {
            IsInitializing,
            IsPlaying,
            IsPaused,
            IsGameFinished
        }
    }
}