using System;
using System.Collections.Generic;
using DG.Tweening;
using Lumley.AiTest.GameShared;
using Lumley.AiTest.SceneManagement;
using Lumley.AiTest.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Lumley.AiTest.Journey
{
    /// <summary>
    /// Controls the current journey day view
    /// </summary>
    public sealed class JourneyController : MonoBehaviour
    {
        [Header("Introduction")] [SerializeField]
        private GameObject _introductionRoot = null!;
        
        [SerializeField]
        private TMP_Text _journeyDayText = null!;
        
        [SerializeField]
        private string _journeyDayTextFormat = "Day {0} of your journey";
        
        [SerializeField]
        private TMP_Text _journeyHoursLeftText = null!;
        
        [SerializeField]
        private string _journeyHoursLeftTextFormat = "You have {0}h {1}m left to complete it";

        [Header("Streak Lost")] [SerializeField]
        private GameObject _streakLostRoot = null!;

        [SerializeField] private TMP_Text _streakLostText = null!;
        [SerializeField] private string _streakLostTextFormat = "You lost your streak of {0} days! Try again!";

        [Header("Configuration")] [SerializeField]
        private JourneyConfig _journeyConfig = null!;
        
        [SerializeField, Tooltip("Load mode of the scene when starting a game from the journey")]
        private LoadSceneMode _loadSceneMode = LoadSceneMode.Single;

        [Header("Journey view")]
        [SerializeField, Tooltip("Finds all steps inside this root, they will be set up in order from top to bottom")]
        private Transform _journeyStepRoot = null!;

        [SerializeField] private GameObject _journeyStepPrefab = null!;
        
        [Header("Cheats")] [SerializeField] private Button _advanceDayButton = null!;
        
        private float _nextTextUpdateTimestamp = 0f;

        private void Start()
        {
            _advanceDayButton.onClick.AddListener(AdvanceDayCheatPressed);
        }

        private void OnEnable()
        {
            InitializeJourney();
        }

        private void InitializeJourney()
        {
            var currentSessionManager = Toolbox.Get<ICurrentSessionManager>();
            long currentDayEpoch = (long)DateTimeUtilities.GetCurrentTimeSinceEpoch().TotalDays;
            var gameCompletionOnLastKnownSessionList = currentSessionManager.GameCompletionList;
            var lastSavedRealtimeDay = currentSessionManager.LastSavedRealtimeDay;
            var baseSeed = currentSessionManager.BaseSeed;
            var sessionRealtimeResult = currentSessionManager.SetRealtimeDay(currentDayEpoch);
            _journeyDayText.text = string.Format(_journeyDayTextFormat, currentSessionManager.PlayerGameStreak);
            if (sessionRealtimeResult == ICurrentSessionManager.SessionRealtimeResult.StreakBroken)
            {
                DisplayGameOver();
            }
            else if (currentSessionManager.PlayerGameStreak == 0 && currentSessionManager.CompletedGameCount == 0)
            {
                DisplayIntroduction();
            }

            var gamesForDay = _journeyConfig.GetGamesForDay(baseSeed, lastSavedRealtimeDay);
            int maxGameCount = Mathf.Min(gameCompletionOnLastKnownSessionList.Count, gamesForDay.Length);
            for (int i = 0; i < maxGameCount; i++)
            {
                var game = gamesForDay[i];
                game.IsCompleted = gameCompletionOnLastKnownSessionList[i];
            }

            DisplayJourneyMap(gamesForDay);
        }

        private void Update()
        {
            if (Time.time < _nextTextUpdateTimestamp)
            {
                return;
            }
            
            DateTime now = DateTime.Now;
            DateTime tomorrow = now.Date.AddDays(1);
            TimeSpan timeLeft = tomorrow - now;

            _journeyHoursLeftText.text = string.Format(_journeyHoursLeftTextFormat, (int)timeLeft.TotalHours, timeLeft.Minutes);
        }

        private void DisplayJourneyMap(IReadOnlyList<GameJourney> gamesForDay)
        {
            // Here we could use pooling but this view doesn't usually refresh while open, so in terms of overall memory, it is more convenient to instantiate and destroy on demand.
            var rootChildCount = _journeyStepRoot.childCount;
            var stepRoots = new Transform[rootChildCount];
            for (int i = 0; i < rootChildCount; i++)
            {
                var child = _journeyStepRoot.GetChild(i);
                stepRoots[i] = child;
                // Clear the child game objects, we will instantiate new ones
                var childCountOfStep = child.childCount;
                for (int j = childCountOfStep - 1; j >= 0; j--)
                {
                    Destroy(child.GetChild(j).gameObject);
                }
            }

            var gameIndex = 0;
            int maxGameIndex = Mathf.Min(gamesForDay.Count, rootChildCount);
            if (rootChildCount < gamesForDay.Count) // We will still display the games that fit in the journey step root
            {
                Debug.LogError(
                    $"Not enough steps in the journey step root to display all games for the day. Expected at least {gamesForDay.Count} but found only {rootChildCount}. Please add more steps to the journey step root.",
                    this);
            }

            for (; gameIndex < maxGameIndex; gameIndex++)
            {
                var gameJourney = gamesForDay[gameIndex];
                var stepRoot = stepRoots[gameIndex];
                var instance = Instantiate(_journeyStepPrefab, stepRoot);
                var journeyVisibleStep = instance.GetComponent<JourneyVisibleStep>();
                if (journeyVisibleStep != null)
                {
                    journeyVisibleStep.Setup(gameJourney, OnGameJourneyTapped);
                }
                else
                {
                    Debug.LogError($"Instantiated journey step prefab does not have a {nameof(JourneyVisibleStep)} component attached. Please add it to the prefab.", instance);
                }
            }
        }

        private async void OnGameJourneyTapped(GameJourney gameJourney)
        {
            try
            {
                var gameInfoManager = Toolbox.Get<ICurrentGameInfoManager>();
                gameInfoManager.CurrentGameDifficulty = gameJourney.Difficulty;
                gameInfoManager.CurrentGameIndex = gameJourney.GameIndex;
                gameInfoManager.CurrentGameAsset = gameJourney.GameInfo.Scene;

                var sceneTransitionManager = Toolbox.Get<ISceneTransitionManager>();
                await sceneTransitionManager.TransitionToSceneAsync(gameJourney.GameInfo.Scene, _loadSceneMode);
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }
        
        private async void AdvanceDayCheatPressed()
        {
            try
            {
                _advanceDayButton.interactable = false;
                var currentSessionManager = Toolbox.Get<ICurrentSessionManager>();
                var sessionPersistenceManager = Toolbox.Get<ISessionPersistenceManager>();

                var serializableSession = currentSessionManager.ExportSession();
                serializableSession.StartingDayEpoch -= 1;
                
                await sessionPersistenceManager.PersistSessionAsync(serializableSession);
                currentSessionManager.LoadSession(serializableSession);
                InitializeJourney();
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
            finally
            {
                _advanceDayButton.interactable = true;
            }
        }

        private async void DisplayGameOver()
        {
            try
            {
                var currentSessionManager = Toolbox.Get<ICurrentSessionManager>();
                var playerGameStreak = currentSessionManager.PlayerGameStreak;
                _streakLostText.text = string.Format(_streakLostTextFormat, playerGameStreak);

                _streakLostRoot.gameObject.SetActive(true);

                // Erase the current session
                var sessionPersistenceManager = Toolbox.Get<ISessionPersistenceManager>();
                await sessionPersistenceManager.PersistSessionAsync(SerializableSession.Null);
                currentSessionManager.LoadSession(SerializableSession.Null);
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }

        private void DisplayIntroduction()
        {
            // TODO (slumley): Show the introduction screen, ideally with a timeline and await it
            _introductionRoot.gameObject.SetActive(true);
        }
    }
}