using System;
using System.Collections.Generic;
using Lumley.AiTest.GameShared;
using Lumley.AiTest.SceneManagement;
using Lumley.AiTest.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lumley.AiTest.Journey
{
    /// <summary>
    /// Controls the current journey day view
    /// </summary>
    public sealed class JourneyController : MonoBehaviour
    {
        [Header("Introduction")] [SerializeField]
        private GameObject _introductionRoot = null!;
        // TODO (slumley): Add reference to timeline to start playing it and await until it's done

        [Header("Streak Lost")] [SerializeField]
        private GameObject _streakLostRoot = null!;

        [SerializeField] private CanvasGroup _streakLostCanvasGroup = null!;

        [Header("Configuration")] [SerializeField]
        private JourneyConfig _journeyConfig = null!;
        
        [SerializeField, Tooltip("Load mode of the scene when starting a game from the journey")]
        private LoadSceneMode _loadSceneMode = LoadSceneMode.Single;

        [Header("Journey view")]
        [SerializeField, Tooltip("Finds all steps inside this root, they will be set up in order from top to bottom")]
        private Transform _journeyStepRoot = null!;

        [SerializeField] private GameObject _journeyStepPrefab = null!;

        private void OnEnable()
        {
            var currentSessionManager = Toolbox.Get<ICurrentSessionManager>();
            long currentDayEpoch = (long)DateTimeUtilities.GetCurrentTimeSinceEpoch().TotalDays;
            var gameCompletionOnLastKnownSessionList = currentSessionManager.GameCompletionList;
            var lastSavedRealtimeDay = currentSessionManager.LastSavedRealtimeDay;
            var baseSeed = currentSessionManager.BaseSeed;
            var sessionRealtimeResult = currentSessionManager.SetRealtimeDay(currentDayEpoch);
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
                    Destroy(child.GetChild(j));
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

                var sceneTransitionManager = Toolbox.Get<ISceneTransitionManager>();
                await sceneTransitionManager.TransitionToSceneAsync(gameJourney.GameInfo.Scene, _loadSceneMode);
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }

        private void DisplayGameOver()
        {
            // TODO (slumley): Show the game over screen, transition to the main menu after
            _streakLostRoot.gameObject.SetActive(true);
        }

        private void DisplayIntroduction()
        {
            // TODO (slumley): Show the introduction screen, hide after
            _introductionRoot.gameObject.SetActive(true);
        }
    }
}