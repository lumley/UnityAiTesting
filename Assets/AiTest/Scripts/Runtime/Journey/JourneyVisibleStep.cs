using System;
using UnityEngine;
using UnityEngine.UI;

namespace Lumley.AiTest.Journey
{
    /// <summary>
    /// A visible step in the journey, representing a game that can be played.
    /// </summary>
    public sealed class JourneyVisibleStep : MonoBehaviour
    {
        [SerializeField]
        private Image _stepImage = null!;
        
        [SerializeField, Tooltip("Changes the color of this graphics depending on the difficulty of the game")]
        private Graphic _colorGraphic = null!;
        
        [SerializeField]
        private Transform _completedTransform = null!;
        
        [SerializeField]
        private Button _startGameButton = null!;
        
        [Header("Configuration")]
        [SerializeField]
        private JourneyConfig _journeyConfig = null!;

        private Action<GameJourney>? _onStartGameClicked;
        private GameJourney? _gameJourney;

        public void Setup(GameJourney gameJourney, Action<GameJourney> onStartGameClicked)
        {
            _gameJourney = gameJourney;
            _onStartGameClicked = onStartGameClicked;
            _stepImage.sprite = gameJourney.GameInfo.GameSprite;
            _colorGraphic.color = _journeyConfig.GetColorForDifficulty(gameJourney.Difficulty);
            _startGameButton.interactable = !gameJourney.IsCompleted;
            
            // TODO (slumley): Animate the game completed with delay according to game index
            _completedTransform.gameObject.SetActive(gameJourney.IsCompleted);
        }
        
        private void OnEnable()
        {
            _startGameButton.onClick.AddListener(OnStartGameClicked);
        }

        private void OnDisable()
        {
            _startGameButton.onClick.RemoveListener(OnStartGameClicked);
        }

        private void OnStartGameClicked()
        {
            if (_onStartGameClicked != null && _gameJourney != null)
            {
                _onStartGameClicked.Invoke(_gameJourney);
            }
        }
    }
}