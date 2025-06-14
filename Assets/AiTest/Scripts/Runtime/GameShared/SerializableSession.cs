using System;

namespace Lumley.AiTest.GameShared
{
    [Serializable]
    public struct SerializableSession : IEquatable<SerializableSession>
    {
        public const int LatestVersion = 1;
        public static SerializableSession Null => default;

        /// <summary>
        /// Version with which this session was serialized
        /// </summary>
        /// <version>1</version>
        public int Version;

        /// <summary>
        /// The amount of days this session has been winning
        /// </summary>
        /// <version>1</version>
        public int GameStreak;

        /// <summary>
        /// The starting day since epoch in which the session was created
        /// </summary>
        /// <version>1</version>
        public long StartingDayEpoch;

        /// <summary>
        /// The base seed selected upon session creation
        /// </summary>
        /// <version>1</version>
        public long BaseSeed;

        /// <summary>
        /// An array of boolean with each game status (completed or not) in the current day
        /// </summary>
        /// <version>1</version>
        public bool[] CurrentDayCompletion;

        private SerializableSession(int version, int gameStreak, long startingDayEpoch, long baseSeed,
            bool[] currentDayCompletion)
        {
            GameStreak = gameStreak;
            StartingDayEpoch = startingDayEpoch;
            BaseSeed = baseSeed;
            CurrentDayCompletion = currentDayCompletion;
            Version = version;
        }

        public bool Equals(SerializableSession other)
        {
            // We skip CurrentDayCompletion because it's an object which makes this struct less efficient, it's also irrelevant for our concept of "equality" where we just want to know if it's the same session, but not if the contents of the session actually match.
            return Version == other.Version && GameStreak == other.GameStreak &&
                   StartingDayEpoch == other.StartingDayEpoch && BaseSeed == other.BaseSeed;
        }

        public override bool Equals(object? obj)
        {
            return obj is SerializableSession other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Version, GameStreak, StartingDayEpoch, BaseSeed);
        }

        public static SerializableSession CreateEmpty(long startingDayEpoch, long baseSeed, int journeyGames)
        {
            return new SerializableSession(LatestVersion, 0, startingDayEpoch, baseSeed, new bool[journeyGames]);
        }

        public static SerializableSession Create(int version, int gameStreak, long startingDayEpoch, long baseSeed,
            bool[] currentDayCompletion)
        {
            while (version < LatestVersion)
            {
                // Here we'd perform specific changes from version to version, until all data is up to date
                // E.g. if (version == 1) { /* Perform here the changes required */ }
                // version++;
                version = LatestVersion; // As we don't have migrations in place yet, skip to the current version. Once we do have, increase depending on versions that require migration.
            }

            return new SerializableSession(version, gameStreak, startingDayEpoch, baseSeed, currentDayCompletion);
        }
    }
}