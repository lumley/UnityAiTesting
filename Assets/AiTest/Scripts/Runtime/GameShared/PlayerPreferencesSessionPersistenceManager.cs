using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Lumley.AiTest.GameShared
{
    /// <summary>
    /// Persistence manager which saves and loads from player preferences
    /// </summary>
    public sealed class PlayerPreferencesSessionPersistenceManager : MonoBehaviour, ISessionPersistenceManager
    {
        [SerializeField] private string _preferencesKey = "LAST_GAME";
        
        public Task<SerializableSession> LoadSessionAsync()
        {
            string savedGame = PlayerPrefs.GetString(_preferencesKey, string.Empty);
            SerializableSession serializableSession = JsonUtility.FromJson<SerializableSession>(savedGame);
            return Task.FromResult(serializableSession);
        }

        public Task PersistSessionAsync(SerializableSession serializableSession)
        {
            string? savedGame = JsonUtility.ToJson(serializableSession, prettyPrint: false);
            if (savedGame == null)
            {
                throw new ArgumentException("SerializableSession was not convertible to JSON");
            }
            PlayerPrefs.SetString(_preferencesKey, savedGame);
            PlayerPrefs.Save();
            return Task.CompletedTask;
        }
    }
}