using Lumley.AiTest.GameShared;
using UnityEngine;
using UnityEngine.UI;

namespace Lumley.AiTest.MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Menu UI")] public Button tetrisButton;
        public Button woodokuButton;
        public Button blockJamButton;
        public Button colorSortButton;

        [Header("Difficulty Buttons")] public Button easyButton;
        public Button mediumButton;
        public Button hardButton;
        public Button impossibleButton;

        private GameManager.MiniGameType selectedGame;

        private void Start()
        {
            SetupButtons();
        }

        private void SetupButtons()
        {
            tetrisButton.onClick.AddListener(() => SelectGame(GameManager.MiniGameType.Tetris));
            woodokuButton.onClick.AddListener(() => SelectGame(GameManager.MiniGameType.Woodoku));
            blockJamButton.onClick.AddListener(() => SelectGame(GameManager.MiniGameType.BlockJam));
            colorSortButton.onClick.AddListener(() => SelectGame(GameManager.MiniGameType.ColorSort));

            easyButton.onClick.AddListener(() => StartGame(GameManager.Difficulty.Easy));
            mediumButton.onClick.AddListener(() => StartGame(GameManager.Difficulty.Medium));
            hardButton.onClick.AddListener(() => StartGame(GameManager.Difficulty.Hard));
            impossibleButton.onClick.AddListener(() => StartGame(GameManager.Difficulty.Impossible));
        }

        private void SelectGame(GameManager.MiniGameType gameType)
        {
            selectedGame = gameType;
            // Show difficulty selection UI
        }

        private void StartGame(GameManager.Difficulty difficulty)
        {
            GameManager.Instance.StartMiniGame(selectedGame, difficulty);
        }
    }
}