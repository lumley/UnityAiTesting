using System.Threading.Tasks;

namespace Lumley.AiTest.GameShared
{
    /// <summary>
    /// Loads a session from any persistence source (a remote server, preferences or from a file in the hard drive) and saves it in memory. Access to the session loaded should be performed via <see cref="ICurrentSessionManager"/>
    /// </summary>
    public interface ISessionPersistenceManager
    {
        /// <summary>
        /// Starts loading the last session from a given source.
        /// </summary>
        /// <returns><see cref="Task{TResult}"/> with a <see cref="SerializableSession"/> as result, <see cref="PSerializableSessionNull"/> when no session exists</returns>
        Task<SerializableSession> LoadSessionAsync();

        /// <summary>
        /// Persists the given session
        /// </summary>
        /// <param name="serializableSession"><see cref="SerializableSession"/> to persist</param>
        /// <returns><see cref="Task"/> completed once session is persisted</returns>
        Task PersistSessionAsync(SerializableSession serializableSession);
    }
}