using UnityEngine;

namespace Lumley.AiTest.ComponentUtilities
{
    /// <summary>
    /// Sets the object where it is placed as don't destroy on load
    /// </summary>
    public sealed class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}