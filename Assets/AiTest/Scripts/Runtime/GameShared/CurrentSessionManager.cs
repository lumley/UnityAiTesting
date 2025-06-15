using System.Collections.Generic;
using Lumley.AiTest.Utilities;
using UnityEngine;

namespace Lumley.AiTest.GameShared
{
    public class CurrentSessionManager : MonoBehaviour, ICurrentSessionManager
    {
        private bool[] _gameCompletionArray = { };
        private long _startingRealtimeDayEpoch;
        private long _baseSeed;

        public int PlayerGameStreak { get; private set; }

        public long LastSavedRealtimeDay => _startingRealtimeDayEpoch + PlayerGameStreak;

        public int SeedForLastSavedRealtimeDay =>
            SeedGenerationUtilities.GetSeedForDay(_baseSeed, LastSavedRealtimeDay);

        public long BaseSeed => _baseSeed;

        public IReadOnlyList<bool> GameCompletionList => _gameCompletionArray;

        public int CompletedGameCount
        {
            get
            {
                int gameCompletedCount = 0;
                foreach (var isGameCompleted in _gameCompletionArray)
                {
                    if (isGameCompleted)
                    {
                        gameCompletedCount += 1;
                    }
                }

                return gameCompletedCount;
            }
        }

        public void SetGameIndexCompleted(int gameIndex)
        {
            if (gameIndex < 0 || gameIndex >= _gameCompletionArray.Length)
            {
                return;
            }

            _gameCompletionArray[gameIndex] = true;
        }

        public ICurrentSessionManager.SessionRealtimeResult SetRealtimeDay(long realtimeDay)
        {
            var daysSincePrevious = realtimeDay - LastSavedRealtimeDay;
            if (daysSincePrevious == 0)
            {
                return ICurrentSessionManager.SessionRealtimeResult.StreakRemains;
            }

            if (daysSincePrevious == 1 && AreAllGamesCompleted())
            {
                for (var i = 0; i < _gameCompletionArray.Length; i++)
                {
                    _gameCompletionArray[i] = false;
                }

                PlayerGameStreak += 1;
                return ICurrentSessionManager.SessionRealtimeResult.StreakContinues;
            }

            return ICurrentSessionManager.SessionRealtimeResult.StreakBroken;
        }

        private bool AreAllGamesCompleted()
        {
            foreach (bool gameCompleted in _gameCompletionArray)
            {
                if (!gameCompleted)
                {
                    return false;
                }
            }

            return true;
        }

        public void LoadSession(SerializableSession serializableSession)
        {
            _startingRealtimeDayEpoch = serializableSession.StartingDayEpoch;
            PlayerGameStreak = serializableSession.GameStreak;
            _baseSeed = serializableSession.BaseSeed;
            _gameCompletionArray = serializableSession.CurrentDayCompletion;
        }

        public SerializableSession ExportSession()
        {
            return SerializableSession.Create(
                SerializableSession.LatestVersion,
                PlayerGameStreak,
                _startingRealtimeDayEpoch,
                _baseSeed,
                _gameCompletionArray);
        }
    }
}