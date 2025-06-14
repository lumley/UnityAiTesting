using System;
using System.Threading.Tasks;
using Lumley.AiTest.GameShared;
using Lumley.AiTest.Journey;
using Lumley.AiTest.SceneManagement;
using Lumley.AiTest.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Lumley.AiTest.MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("User interaction")]
        [SerializeField] private Button _continueButton = null!;
        [SerializeField] private Button _newGameButton = null!;
        [SerializeField] private Button _settingsButton = null!;

        [Header("Scene switching")]
        [SerializeField] private AssetReference _journeyScene = null!;
        [SerializeField] private LoadSceneMode _loadSceneMode = LoadSceneMode.Single;

        [Header("Game creation")] [SerializeField]
        private JourneyConfig _config = null!;

        private Button[] _allButtons = { };
        private SerializableSession? _lastSerializableSession;

        private void Start()
        {
            _allButtons = new[]
            {
                _continueButton, _newGameButton, _settingsButton
            };
            SetupButtons();
        }

        private void OnEnable()
        {
            _continueButton.gameObject.SetActive(false);
            CheckLastSavedGameInBackground();
        }

        private void SetupButtons()
        {
            _continueButton.onClick.AddListener(OnContinueClicked);
            _newGameButton.onClick.AddListener(OnNewGameClicked);
            _settingsButton.onClick.AddListener(OnSettingsClicked);
        }

        private async void CheckLastSavedGameInBackground()
        {
            try
            {
                var sessionPersistenceManager = Toolbox.Get<ISessionPersistenceManager>();
                var lastSession = await sessionPersistenceManager.LoadSessionAsync();
                _lastSerializableSession = lastSession;
                if (!SerializableSession.Null.Equals(lastSession))
                {
                    _continueButton.gameObject.SetActive(true);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }

        private void OnSettingsClicked()
        {
            // TODO (slumley): Open settings menu
        }

        private async void OnNewGameClicked()
        {
            try
            {
                SetButtonsInteractive(false);
                await CreateNewGameAsync();
                await SwitchToSceneAsync();
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }

        private async void OnContinueClicked()
        {
            try
            {
                SetButtonsInteractive(false);
                if (_lastSerializableSession != null && !SerializableSession.Null.Equals(_lastSerializableSession))
                {
                    var currentSessionManager = Toolbox.Get<ICurrentSessionManager>();
                    currentSessionManager.LoadSession(_lastSerializableSession.Value);
                }
                else // This is a fallback, but the continue button should not have been clickable
                {
                    await CreateNewGameAsync();
                }

                await SwitchToSceneAsync();
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }

        private async Task CreateNewGameAsync()
        {
            long startingDay = (long) DateTimeUtilities.GetCurrentTimeSinceEpoch().TotalDays;
            long baseSeed = SeedGenerationUtilities.GenerateBaseSeed();
            int journeyGameLength = _config.GetGameCountForDay(baseSeed, startingDay);

            var newSession = SerializableSession.CreateEmpty(startingDay, baseSeed, journeyGameLength);

            var currentSessionManager = Toolbox.Get<ICurrentSessionManager>();
            currentSessionManager.LoadSession(newSession);

            var sessionPersistenceManager = Toolbox.Get<ISessionPersistenceManager>();
            await sessionPersistenceManager.PersistSessionAsync(newSession);
        }

        private async Task SwitchToSceneAsync()
        {
            var sceneTransitionManager = Toolbox.Get<ISceneTransitionManager>();
            await sceneTransitionManager.TransitionToSceneAsync(_journeyScene, _loadSceneMode);
        }

        private void SetButtonsInteractive(bool shouldBeInteractive)
        {
            foreach (var button in _allButtons)
            {
                button.interactable = shouldBeInteractive;
            }
        }
    }
}