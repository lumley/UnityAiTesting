using System;
using Lumley.AiTest.GameShared;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Lumley.AiTest.SceneManagement
{
    /// <summary>
    /// Switches to a given scene when a button is pressed
    /// </summary>
    [RequireComponent(typeof(Button))]
    public sealed class SwitchSceneOnPress : MonoBehaviour
    {
        [SerializeField] private Button _button = null!;

        [SerializeField] private AssetReference _targetScene = null!;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnPressed);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnPressed);
        }

        private void Reset()
        {
            _button = GetComponent<Button>();
        }

        private async void OnPressed()
        {
            _button.interactable = false;
            try
            {
                var sceneTransitionManager = Toolbox.Get<ISceneTransitionManager>();
                await sceneTransitionManager.TransitionToSceneAsync(_targetScene);
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }
    }
}