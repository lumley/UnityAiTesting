using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Lumley.AiTest.ComponentUtilities
{
    /// <summary>
    /// Component to animate the fade in an image.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public sealed class AnimateFadeImage : MonoBehaviour
    {
        [SerializeField] private Image _image = null!;
        [SerializeField] private bool _playOnEnable = true;
        
        [Header("Animation")]
        [SerializeField] private float _delay;
        [SerializeField] private float _duration = 0.3f;
        
        [SerializeField] private Ease _ease = Ease.InOutSine;
        
        [Header("Fading")]
        [SerializeField] private float _targetAlpha = 1f;
        [SerializeField] private bool _shouldSetAlphaOnAwake = true;
        [SerializeField] private bool _shouldSetAlphaOnDisable = true;
        [SerializeField] private float _resetAlpha;
        private Tween? _tween;
        
        private void Awake()
        {
            TrySetAlpha(_shouldSetAlphaOnAwake);
        }
        
        private void OnEnable()
        {
            if (_playOnEnable)
            {
                KillPreviousTween();
                _tween = _image.DOFade(_targetAlpha, _duration).SetEase(_ease).SetDelay(_delay);
            }
        }
        
        private void OnDisable()
        {
            KillPreviousTween();
            TrySetAlpha(_shouldSetAlphaOnDisable);
        }

        private void Reset()
        {
            _image = GetComponent<Image>();
        }

        private void KillPreviousTween()
        {
            if (_tween != null && _tween.IsActive() && !_tween.IsComplete())
            {
                _tween.Kill();
                _tween = null;
            }
        }
        
        private void TrySetAlpha(bool shouldSetAlpha)
        {
            if (shouldSetAlpha)
            {
                var image = GetComponent<Image>();
                if (image != null)
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, _resetAlpha);
                }
            }
        }
    }
}