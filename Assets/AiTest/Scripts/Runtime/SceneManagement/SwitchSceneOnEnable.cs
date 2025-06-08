using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lumley.AiTest.SceneManagement
{
    /// <summary>
    /// Switches to a scene whenever the object is enabled
    /// </summary>
    public sealed class SwitchSceneOnEnable : MonoBehaviour
    {
        [SerializeField, Tooltip("Index of the target scene")]
        private int _sceneIndex = 1;

        [SerializeField] private LoadSceneMode _loadSceneMode = LoadSceneMode.Single;

        private void OnEnable()
        {
            SwitchScene();
        }

        private async void SwitchScene()
        {
            try
            {
                await SceneManager.LoadSceneAsync(_sceneIndex, _loadSceneMode);
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }
    }
}