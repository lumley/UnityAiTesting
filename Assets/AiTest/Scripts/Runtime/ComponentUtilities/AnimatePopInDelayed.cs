using DG.Tweening;
using UnityEngine;

namespace Lumley.AiTest.ComponentUtilities
{
    /// <summary>
    /// Utility class for adding a delayed pop-in animations to game objects.
    /// </summary>
    public sealed class AnimatePopInDelayed : MonoBehaviour
    {
        [SerializeField] private bool _playOnEnable = true;
        
        [Header("Animation")]
        [SerializeField] private float _delay = 0.5f;
        [SerializeField] private float _duration = 0.5f;
        
        [SerializeField] private Ease _ease = Ease.OutBack;
        
        [Header("Scaling")]
        [SerializeField] private bool _shouldSetScaleOnAwake = true;
        [SerializeField] private bool _shouldSetScaleOnDisable = true;
        [SerializeField] private Vector3 _setScaleOnAwake = Vector3.one * 0.1f;
        private Tween? _tween;

        private void Awake()
        {
            TrySetScale(_shouldSetScaleOnAwake);
        }

        private void OnEnable()
        {
            KillPreviousTween();
            _tween = transform.DOScale(Vector3.one, duration: _duration).SetEase(_ease).SetDelay(_delay);
        }

        private void OnDisable()
        {
            KillPreviousTween();
            TrySetScale(_shouldSetScaleOnDisable);
        }
        
        private void KillPreviousTween()
        {
            if (_tween != null && _tween.IsActive() && !_tween.IsComplete())
            {
                _tween.Kill();
                _tween = null;
            }
        }
        
        private void TrySetScale(bool shouldSetScale)
        {
            if (shouldSetScale)
            {
                transform.localScale = _setScaleOnAwake;
            }
        }
    }
}