using System.Collections.Generic;

namespace Lumley.AiTest.GameShared
{
    /// <summary>
    /// Handles the current session of the user, this includes, which day the user is playing, the seed selected, save file, etc.
    /// </summary>
    public interface ICurrentSessionManager
    {
        /// <summary>
        /// The game streak of the player (0 = playing its first day, 1 = second day, etc.)
        /// </summary>
        int PlayerGameStreak { get; }
        
        /// <summary>
        /// The current day of this player as known by the session since EPOCH.
        /// </summary>
        long LastSavedRealtimeDay { get; }
        
        /// <summary>
        /// The seed generated for this session on the current day <see cref="LastSavedRealtimeDay"/>
        /// </summary>
        int SeedForLastSavedRealtimeDay { get; }
        
        /// <summary>
        /// The base seed saved in the session, used to generate the games for the current day.
        /// </summary>
        long BaseSeed { get; }

        /// <summary>
        /// List that maps each index to each game and their completions status (true = completed, false otherwise)
        /// </summary>
        IReadOnlyList<bool> GameCompletionList { get; }
        
        /// <summary>
        /// Gets the amount of completed games in the current day, same as checking each game state in <see cref="GameCompletionList"/>
        /// </summary>
        int CompletedGameCount { get; }

        /// <summary>
        /// On a game completion, sets the given game index as completed
        /// </summary>
        /// <param name="gameIndex"></param>
        void SetGameIndexCompleted(int gameIndex);

        /// <summary>
        /// Sets the realtime day (usually, number of days since EPOCH). Calculates the current streak result on day advancement.
        /// </summary>
        /// <param name="realtimeDay"><see cref="long"/> with the current day in epoch</param>
        /// <returns><see cref="SessionRealtimeResult.StreakBroken"/> when the current day is not consecutive with the last one or when the <see cref="GameCompletionList"/> is not all completed. <see cref="SessionRealtimeResult.StreakContinues"/> otherwise.</returns>
        SessionRealtimeResult SetRealtimeDay(long realtimeDay);

        /// <summary>
        /// Loads a given session into the current session manager, resetting the full state into that given session.
        /// </summary>
        /// <param name="serializableSession"><see cref="SerializableSession"/> with the contents of a session to use in the current session</param>
        void LoadSession(SerializableSession serializableSession);

        /// <summary>
        /// Exports the currently loaded session as a <see cref="SerializableSession"/>
        /// </summary>
        /// <returns><see cref="SerializableSession"/> with the current contents of the session.</returns>
        SerializableSession ExportSession();
        
        /// <summary>
        /// Indicates the status of the session after advancing the day
        /// </summary>
        public enum SessionRealtimeResult
        {
            /// <summary>
            /// Day has not changed
            /// </summary>
            StreakRemains = 0,
            
            /// <summary>
            /// Day has changed and the streak continues
            /// </summary>
            StreakContinues = 1,
            
            /// <summary>
            /// Day has changed and the streak is broken, either because the player didn't complete all games or because the day is not consecutive with the last one.
            /// </summary>
            StreakBroken = 2,
        }
    }
}