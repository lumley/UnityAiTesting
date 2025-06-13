using System;
using Lumley.AiTest.GameShared;
using UnityEngine;

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

        private void OnEnable()
        {
            var currentSessionManager = Toolbox.Get<ICurrentSessionManager>();
            int currentDayEpoch = 0; // TODO (slumley): Update the current day since Epoch
            var sessionRealtimeResult = currentSessionManager.SetRealtimeDay(currentDayEpoch);
            if (sessionRealtimeResult == ICurrentSessionManager.SessionRealtimeResult.StreakBroken)
            {
                DisplayGameOver();
            }
            else if (currentSessionManager.PlayerDay == 0 && currentSessionManager.CompletedGameCount == 0)
            {
                DisplayIntroduction();
            }
            
            // TODO (slumley): Generate journey view for current streak and day
        }

        private void DisplayGameOver()
        {
            throw new NotImplementedException();
        }

        private void DisplayIntroduction()
        {
            throw new NotImplementedException();
        }
    }
}