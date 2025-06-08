using System;
using UnityEngine;
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

        [SerializeField] private string _targetSceneName = null!;

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

        private void OnPressed()
        {
            _button.interactable = false;
            SceneTransitionManager.Instance.TransitionToScene(_targetSceneName);
        }
    }
}