using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lumley.AiTest.GameShared
{
    /// <summary>
    /// Toolbox is a singleton that provides a way to access various objects in the game. It allows to be copied to all scenes so that each can be tested independently in editor. It has minimal impact in runtime, allows each object within to safely reference each other. It does not support removing an object.
    ///
    /// Taken from my own utility: https://github.com/lumley/unity-gamejam-utilities/blob/master/LumleyJamUtilities/Runtime/Toolbox/Toolbox.cs
    /// </summary>
    public class Toolbox : MonoBehaviour
    {
        private static Toolbox? _instance;

        [SerializeField] private bool _shouldNotDestroyOnLoad = true;
        private readonly List<object> _allObjects = new();
        private readonly Dictionary<Type, object> _cachedObjectMap = new();
        private bool _isInitialized;

        private void Awake()
        {
            TryInitialize();
        }

        private void TryInitialize()
        {
            if (_isInitialized)
            {
                return;
            }

            _isInitialized = true;

            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            if (_shouldNotDestroyOnLoad)
            {
                DontDestroyOnLoad(this);
            }

            var currentTransform = gameObject.transform;
            for (int i = 0; i < currentTransform.childCount; i++)
            {
                var child = currentTransform.GetChild(i);
                Add(child);
            }
        }

        public static void Add(object accessibleObject)
        {
            _instance?.AddInternal(accessibleObject);
        }

        public static T Get<T>() where T : class
        {
            T? instanceOrNull = GetOrNull<T>();
            if (instanceOrNull != null)
            {
                return instanceOrNull;
            }

            throw new KeyNotFoundException($"No instance of type {typeof(T).Name} found in Toolbox.");
        }
        
        public static T? GetOrNull<T>() where T : class
        {
            if (_instance != null) return _instance.GetInternal<T>();
            Toolbox? toolboxInstance = FindFirstObjectByType<Toolbox>();
            if (toolboxInstance == null)
            {
                return null;
            }

            toolboxInstance.TryInitialize();
            return toolboxInstance.GetInternal<T>();
        }

        private void AddInternal(object accessibleObject)
        {
            if (accessibleObject is MonoBehaviour monoBehaviour)
            {
                monoBehaviour.transform.SetParent(transform, false);
            }

            _allObjects.Add(accessibleObject);
        }

        private T? GetInternal<T>() where T : class
        {
            var type = typeof(T);
            if (_cachedObjectMap.TryGetValue(type, out object value))
            {
                return (T)value;
            }

            foreach (object element in _allObjects)
            {
                if (element is T tElement)
                {
                    _cachedObjectMap[type] = tElement;
                    return tElement;
                }

                if (element is Component component)
                {
                    var matchingComponent = component.GetComponent<T>();
                    if (matchingComponent != null)
                    {
                        _cachedObjectMap[type] = matchingComponent;
                        return matchingComponent;
                    }
                }
            }

            return null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticFields()
        {
            _instance = null;
        }
    }
}