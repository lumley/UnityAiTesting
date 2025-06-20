﻿using System;
using Lumley.AiTest.GameShared;
using Lumley.AiTest.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lumley.AiTest.Journey
{
    /// <summary>
    /// Journey configuration settings
    /// </summary>
    [CreateAssetMenu(fileName = nameof(JourneyConfig), menuName = "Game/" + nameof(JourneyConfig))]
    public sealed class JourneyConfig : ScriptableObject
    {
        [Header("Daily Game Generation Settings")]
        [SerializeField, Tooltip("Amount of daily games to generate in any given day")]
        private Vector2Int _dailyGameCountRange = new(4, 4);
        
        [SerializeField, Tooltip("The pool of games to use when generating maps")]
        private GameInfo[] _gamePool = new GameInfo[0];

        [Header("Difficulty levels")]
        [SerializeField,
         Tooltip(
             "The difficulty levels available. Can be arranged or repeated, maps will receive each difficulty in the order of appearance")]
        private GameDifficulty[] _availableDifficultyLevels =
        {
            GameDifficulty.Easy,
            GameDifficulty.Medium,
            GameDifficulty.Hard,
            GameDifficulty.Impossible
        };
        
        [FormerlySerializedAs("_difficultyColorMaps")] [SerializeField, Tooltip("Color map for each difficulty level")]
        private DifficultyInfo[] _difficultyInfoArray = new DifficultyInfo[0];
        
        [SerializeField, Tooltip("Default color for unknown difficulties")]
        private DifficultyInfo _defaultDifficultyMap = new()
        {
            Color = Color.white,
            Description = "Unknown"
        };

        /// <summary>
        /// Gets the amount of games that should be generated for a given day
        /// </summary>
        /// <param name="baseSeed"><see cref="long"/> base seed</param>
        /// <param name="daySinceEpoch"><see cref="long"/> days since epoch</param>
        /// <returns>int with the amount of games to generate</returns>
        /// <remarks>This method performs heavy allocations, if used often, please refactor</remarks>
        public int GetGameCountForDay(long baseSeed, long daySinceEpoch)
        {
            var gamesForDay = GetGamesForDay(baseSeed, daySinceEpoch);
            return gamesForDay.Length;
        }

        /// <summary>
        /// Generates the games required for a given day, regardless of the current state for the player
        /// </summary>
        /// <param name="baseSeed"><see cref="long"/> base seed</param>
        /// <param name="daySinceEpoch"><see cref="long"/> days since epoch</param>
        /// <returns>Array of <see cref="GameJourney"/> with the games generated for a given day</returns>
        public GameJourney[] GetGamesForDay(long baseSeed, long daySinceEpoch)
        {
            var seedForDay = SeedGenerationUtilities.GetSeedForDay(baseSeed, daySinceEpoch);
            var random = new System.Random(seedForDay);
            int gameCount = random.Next(_dailyGameCountRange.x, _dailyGameCountRange.y);
            
            GameJourney[] gameJourney = new GameJourney[gameCount];

            for (int i = 0; i < gameCount; i++)
            {
                var availableDifficultyLevel = i < _availableDifficultyLevels.Length ? _availableDifficultyLevels[i] : _availableDifficultyLevels[^1];
                var gameIndex = random.Next(0, _gamePool.Length);
                var gameInfo = _gamePool[gameIndex];
                gameJourney[i] = new GameJourney
                {
                    Difficulty = availableDifficultyLevel,
                    GameInfo = gameInfo,
                    GameIndex = i,
                    IsCompleted = false, // This needs to be adjusted outside
                };
            }
            
            return gameJourney;
        }
        
        /// <summary>
        /// Gets info for a given difficulty level
        /// </summary>
        /// <param name="difficulty"><see cref="GameDifficulty"/> to test</param>
        /// <returns><see cref="DifficultyInfo"/> with info of the difficulty</returns>
        public DifficultyInfo GetInfoForDifficulty(GameDifficulty difficulty)
        {
            foreach (var difficultyColorMap in _difficultyInfoArray)
            {
                if (difficultyColorMap.Difficulty == difficulty)
                {
                    return difficultyColorMap;
                }
            }

            return _defaultDifficultyMap;
        }
        
        /// <summary>
        /// Color map for each difficulty level
        /// </summary>
        [Serializable]
        public sealed class DifficultyInfo
        {
            public GameDifficulty Difficulty;
            public Color Color;
            public string Description;
        }
    }
}