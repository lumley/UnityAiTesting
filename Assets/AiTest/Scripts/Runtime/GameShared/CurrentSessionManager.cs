using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lumley.AiTest.GameShared
{
    public class CurrentSessionManager : MonoBehaviour, ICurrentSessionManager
    {
        private bool[] _gameCompletionArray = { };
        private int _startingRealtimeDayEpoch;
        private int _baseSeed;

        public int PlayerDay { get; private set; }

        public int RealtimeDay =>
            throw new NotImplementedException(); // Calculate the current realtime day from the startingRealtimeDayEpoch

        public int Seed => throw new NotImplementedException(); // Calculate the current seed as _baseSeed + PlayerDay
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

        public ICurrentSessionManager.SessionRealtimeResult SetRealtimeDay(int realtimeDay)
        {
            var daysSincePrevious = realtimeDay - RealtimeDay;
            if (daysSincePrevious == 0)
            {
                return ICurrentSessionManager.SessionRealtimeResult.StreakContinues;
            }

            if (daysSincePrevious == 1 && AreAllGamesCompleted())
            {
                for (var i = 0; i < _gameCompletionArray.Length; i++)
                {
                    _gameCompletionArray[i] = false;
                }

                PlayerDay += 1;
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
            PlayerDay = serializableSession.GameStreak;
            _baseSeed = serializableSession.BaseSeed;
            _gameCompletionArray = serializableSession.CurrentDayCompletion;
        }

        public SerializableSession ExportSession()
        {
            return SerializableSession.Create(
                SerializableSession.LatestVersion,
                PlayerDay,
                _startingRealtimeDayEpoch,
                _baseSeed,
                _gameCompletionArray);
        }
    }
}