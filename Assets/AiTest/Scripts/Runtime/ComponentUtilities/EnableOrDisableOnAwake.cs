using UnityEngine;

namespace Lumley.AiTest.ComponentUtilities
{
    /// <summary>
    /// It is common to have game objects that you want directly enabled while editing but not during runtime. There are a few solutions to this problem, the easiest way (at the expense of minimal runtime performance cost) is to simply add this component and let it turn off when the object is instantiated.
    ///
    /// A more appropriate solution would be to have a specific interface that can be located as a pre-build step and locate and disable these objects. The problem here is that performing local builds would also change the serialization of the affected objects, usually causing confusion among designers/artists. 
    /// </summary>
    public sealed class EnableOrDisableOnAwake : MonoBehaviour
    {
        [SerializeField, Tooltip("Sets this game object as active as configured in this field")]
        private bool _shouldSetActive;

        private void Awake()
        {
            gameObject.SetActive(_shouldSetActive);
        }
    }
}