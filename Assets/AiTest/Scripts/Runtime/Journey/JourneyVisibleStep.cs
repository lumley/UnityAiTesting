using System;
using DG.Tweening;
using TMPro;
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
        
        [SerializeField]
        private TMP_Text _difficultyText = null!;
        
        [Header("Configuration")]
        [SerializeField]
        private JourneyConfig _journeyConfig = null!;
        
        [Header("Animation")]
        [SerializeField]
        private float _fadeAnimationDuration = 0.3f;
        
        [SerializeField]
        private float _delayPerGameIndex = 0.15f;
        
        private Action<GameJourney>? _onStartGameClicked;
        private GameJourney? _gameJourney;

        public void Setup(GameJourney gameJourney, Action<GameJourney> onStartGameClicked)
        {
            _gameJourney = gameJourney;
            _onStartGameClicked = onStartGameClicked;
            _stepImage.sprite = gameJourney.GameInfo.GameSprite;
            var infoForDifficulty = _journeyConfig.GetInfoForDifficulty(gameJourney.Difficulty);
            _colorGraphic.color = infoForDifficulty.Color;
            _difficultyText.text = infoForDifficulty.Description;
            _startGameButton.interactable = !gameJourney.IsCompleted;
            _completedTransform.gameObject.SetActive(false);
            
            transform.localScale = Vector3.one * 0.1f;
            transform.DOScale(Vector3.one, _fadeAnimationDuration)
                .SetDelay(_delayPerGameIndex * gameJourney.GameIndex)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    if (gameJourney.IsCompleted)
                    {
                        _completedTransform.localScale = Vector3.one * 0.1f;
                        _completedTransform.gameObject.SetActive(true);
                        _completedTransform.DOScale(Vector3.one, _fadeAnimationDuration)
                            .SetDelay(_delayPerGameIndex * gameJourney.GameIndex)
                            .SetEase(Ease.OutBack);
                    }
                });
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