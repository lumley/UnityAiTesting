using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Lumley.AiTest.SceneManagement
{
    public class SceneTransitionManager : MonoBehaviour, ISceneTransitionManager
    {
        [Header("Transition Settings")] [SerializeField]
        private GameObject _transitionPanel = null!;

        [SerializeField] private float _fadeInTime = 0.3f;
        [SerializeField] private float _fadeOutTime = 0.2f;

        [SerializeField] private CanvasGroup _canvasGroup = null!;

        public async Task TransitionToSceneAsync(AssetReference sceneReference,
            LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            _canvasGroup.alpha = 0f;
            _transitionPanel.SetActive(true);

            await _canvasGroup.DOFade(1f, _fadeInTime).AsyncWaitForCompletion();
            await Addressables.LoadSceneAsync(sceneReference, loadSceneMode).Task;
            await _canvasGroup.DOFade(0f, _fadeOutTime).AsyncWaitForCompletion();
            _transitionPanel.SetActive(false);
        }
    }
}