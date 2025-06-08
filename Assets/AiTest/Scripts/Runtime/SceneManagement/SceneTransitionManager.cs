using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lumley.AiTest.SceneManagement
{
    public class SceneTransitionManager : MonoBehaviour
    {
        public static SceneTransitionManager Instance { get; private set; }

        [Header("Transition Settings")] public GameObject transitionPanel;
        public float transitionDuration = 1f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void TransitionToScene(string sceneName)
        {
            StartCoroutine(TransitionCoroutine(sceneName));
        }

        private IEnumerator TransitionCoroutine(string sceneName)
        {
            // Fade out
            if (transitionPanel != null)
            {
                transitionPanel.SetActive(true);
                yield return new WaitForSeconds(transitionDuration / 2);
            }

            // Load scene
            SceneManager.LoadScene(sceneName);

            // Fade in
            yield return new WaitForSeconds(transitionDuration / 2);

            if (transitionPanel != null)
            {
                transitionPanel.SetActive(false);
            }
        }
    }
}