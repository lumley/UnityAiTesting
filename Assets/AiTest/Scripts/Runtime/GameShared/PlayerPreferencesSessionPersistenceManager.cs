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

        [Header("Editor only values")]
        [SerializeField, Tooltip("The last game that was loaded or saved")] private string _lastLoadedGame = string.Empty;

        [SerializeField,
         Tooltip("Only in editor, when active, the editor will only write into this object instead of in preferences")]
        private bool _useOnlyLastLoadedGame;

        public Task<SerializableSession> LoadSessionAsync()
        {
            string savedGame = GetLastLoadedGame();
            if (string.IsNullOrEmpty(savedGame))
            {
                return Task.FromResult(SerializableSession.Null);
            }

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

            SaveGame(savedGame);
            return Task.CompletedTask;
        }

        private void SaveGame(string savedGame)
        {
#if UNITY_EDITOR
            if (_useOnlyLastLoadedGame)
            {
                _lastLoadedGame = savedGame;
            }
#endif
            PlayerPrefs.SetString(_preferencesKey, savedGame);
            PlayerPrefs.Save();

#if UNITY_EDITOR
            _lastLoadedGame = savedGame;
#endif
        }

        private string GetLastLoadedGame()
        {
#if UNITY_EDITOR
            if (_useOnlyLastLoadedGame)
            {
                return _lastLoadedGame;
            }
#endif
            var lastLoadedGame = PlayerPrefs.GetString(_preferencesKey, string.Empty);
#if UNITY_EDITOR
            _lastLoadedGame = lastLoadedGame;
#endif
            return lastLoadedGame;
        }
    }
}