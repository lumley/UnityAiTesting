using System;
using Lumley.AiTest.GameShared;
using UnityEngine;
using UnityEngine.UI;

namespace Lumley.AiTest.MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button _continueButton = null!;
        [SerializeField] private Button _newGameButton = null!;
        [SerializeField] private Button _settingsButton = null!;

        private void Start()
        {
            SetupButtons();
        }

        private void OnEnable()
        {
            // TODO (slumley): Check if a game can be continued
            _continueButton.gameObject.SetActive(false);
        }

        private void SetupButtons()
        {
            _continueButton.onClick.AddListener(OnContinueClicked);
            _newGameButton.onClick.AddListener(OnNewGameClicked);
            _settingsButton.onClick.AddListener(OnSettingsClicked);
        }

        private void OnSettingsClicked()
        {
            // TODO (slumley): Open settings menu
        }

        private void OnNewGameClicked()
        {
        }

        private void OnContinueClicked()
        {
            // TODO (slumley): Continue the progress with the last saved game (use preferences)
        }
    }
}