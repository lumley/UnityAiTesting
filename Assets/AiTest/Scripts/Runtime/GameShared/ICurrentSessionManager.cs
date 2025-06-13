using System.Collections.Generic;

namespace Lumley.AiTest.GameShared
{
    /// <summary>
    /// Handles the current session of the user, this includes, which day the user is playing, the seed selected, save file, etc.
    /// </summary>
    public interface ICurrentSessionManager
    {
        /// <summary>
        /// The current day in which the player is, relative to the starting of the game
        /// </summary>
        int PlayerDay { get; }
        
        /// <summary>
        /// The day belonging to the session
        /// </summary>
        int RealtimeDay { get; }
        
        /// <summary>
        /// The seed generated for this session
        /// </summary>
        int Seed { get; }

        /// <summary>
        /// List that maps each index to each game and their completions status (true = completed, false otherwise)
        /// </summary>
        IReadOnlyList<bool> GameCompletionList { get; }

        /// <summary>
        /// On a game completion, sets the given game index as completed
        /// </summary>
        /// <param name="gameIndex"></param>
        void SetGameIndexCompleted(int gameIndex);

        /// <summary>
        /// Sets the realtime day (usually, number of days since EPOCH). Calculates the current streak result on day advancement.
        /// </summary>
        /// <param name="realtimeDay"><see cref="int"/> with the current day in epoch</param>
        /// <returns><see cref="SessionRealtimeResult.StreakBroken"/> when the current day is not consecutive with the last one or when the <see cref="GameCompletionList"/> is not all completed. <see cref="SessionRealtimeResult.StreakContinues"/> otherwise.</returns>
        SessionRealtimeResult SetRealtimeDay(int realtimeDay);

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
            StreakContinues = 0,
            StreakBroken = 1,
        }
    }
}