using System;
using Lumley.AiTest.GameShared;
using Lumley.AiTest.SceneManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Lumley.AiTest.ComponentUtilities
{
    /// <summary>
    /// Switches to a specified scene when the object is pressed.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public sealed class SwitchToSceneOnPressed : MonoBehaviour
    {
        [SerializeField] private Button _button = null!;
        [SerializeField] private AssetReference _sceneReference = null!;
        [SerializeField] private LoadSceneMode _loadSceneMode = LoadSceneMode.Single;

        private void OnEnable()
        {
            _button.onClick.AddListener(SwitchToScene);
        }
        
        private void OnDisable()
        {
            _button.onClick.RemoveListener(SwitchToScene);
        }

        private async void SwitchToScene()
        {
            try
            {
                _button.interactable = false;
                var sceneTransitionManager = Toolbox.Get<ISceneTransitionManager>();
                await sceneTransitionManager.TransitionToSceneAsync(_sceneReference, _loadSceneMode);
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
            finally
            {
                if (_button != null) // Button may have been destroyed by now
                {
                    _button.interactable = true;
                }
            }
        }

        private void Reset()
        {
            _button = GetComponent<Button>();
        }
    }
}