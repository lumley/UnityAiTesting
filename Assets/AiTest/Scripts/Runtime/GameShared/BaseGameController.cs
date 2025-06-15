using System;
using System.Threading.Tasks;
using Lumley.AiTest.ComponentUtilities;
using Lumley.AiTest.SceneManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Lumley.AiTest.GameShared
{
    public abstract class BaseGameController : MonoBehaviour
    {
        [Header("Base Game Settings")][SerializeField] private GameObject _winPanel = null!;
        [SerializeField]
        private GameObject _losePanel = null!;
        [SerializeField]
        private GameObject _pausePanel = null!;
        
        [SerializeField] private Button _pauseButton = null!;
        [SerializeField] private Button _resumeButton = null!;
        [SerializeField] private Button _restartButton = null!;
        
        [SerializeField]
        private TimelineController? _introductionTimeline;
        
        [SerializeField]
        private AssetReference _mainMenuScene = null!;
        
        [Header("Cheats")] [SerializeField] private Button _autoWinButton = null!;
        
        protected GameState State;
        
        protected async void Start()
        {
            try
            {
#if ENABLE_CHEATS
                _autoWinButton.gameObject.SetActive(true);
                _autoWinButton.onClick.AddListener(HandleWin);
#else
                _autoWinButton.gameObject.SetActive(false);
#endif
                AddButtonListeners();
                _resumeButton.gameObject.SetActive(false);
                _pauseButton.gameObject.SetActive(true);
                _pauseButton.interactable = false;
                _restartButton.interactable = false;
                
                var currentSessionManager = Toolbox.Get<ICurrentSessionManager>();
                var seedForLastSavedRealtimeDay = currentSessionManager.SeedForLastSavedRealtimeDay;
                Random.InitState(seedForLastSavedRealtimeDay); // Here we use Unity's Random instead of System.Random because it uses a better algorithm for random number generation and generates more consistent playthroughs since external plugins also tend to use Unity's Random.
                
                State = GameState.IsInitializing;
                var currentGameInfoManager = Toolbox.Get<ICurrentGameInfoManager>();
                var currentGameDifficulty = currentGameInfoManager.CurrentGameDifficulty;
                await InitializeGameAsync(currentGameDifficulty);
                if (_introductionTimeline != null)
                {
                    await _introductionTimeline.PlayTimelineAsync();
                }
                State = GameState.IsPlaying;
                _pauseButton.interactable = true;
                _restartButton.interactable = true;
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
                UpdateGameplay();
            }
        }

        protected abstract Task InitializeGameAsync(GameDifficulty difficulty);
        protected abstract void UpdateGameplay();
        
        [ContextMenu("Win Game")]
        protected void HandleWin()
        {
            var currentSessionManager = Toolbox.Get<ICurrentSessionManager>();
            var currentGameInfoManager = Toolbox.Get<ICurrentGameInfoManager>();
            
            currentSessionManager.SetGameIndexCompleted(currentGameInfoManager.CurrentGameIndex);
            State = GameState.IsGameFinished;
            _winPanel.gameObject.SetActive(true);
        }
        
        [ContextMenu("Lose Game")]
        protected void HandleLose()
        {
            State = GameState.IsGameFinished;
            _losePanel.gameObject.SetActive(true);
        }

        protected void PauseGame()
        {
            State = GameState.IsPaused;
            _pauseButton.gameObject.SetActive(false);
            _resumeButton.gameObject.SetActive(true);
            _pausePanel.SetActive(true);
        }

        protected void ResumeGame()
        {
            State = GameState.IsPlaying;
            _pauseButton.gameObject.SetActive(true);
            _resumeButton.gameObject.SetActive(false);
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
        
        private void AddButtonListeners()
        {
            _pauseButton.onClick.AddListener(PauseGame);
            _resumeButton.onClick.AddListener(ResumeGame);
            _restartButton.onClick.AddListener(RestartGame);
        }
        
        private void RemoveButtonListeners()
        {
            _pauseButton.onClick.RemoveListener(PauseGame);
            _resumeButton.onClick.RemoveListener(ResumeGame);
            _restartButton.onClick.RemoveListener(RestartGame);
        }

        private void OnDestroy()
        {
            RemoveButtonListeners();
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