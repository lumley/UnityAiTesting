using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lumley.AiTest.GameShared
{
    /// <summary>
    /// Pool for any kind of game object, does not keep a reference to alive pooled objects so they can be destroyed without harming the pool. Expects that the prefab matches the instance on return.
    /// Taken from my own utility: https://github.com/lumley/unity-gamejam-utilities/blob/master/LumleyJamUtilities/Runtime/Pooling/PoolingManager.cs
    /// </summary>
    public class PoolingManager : MonoBehaviour
    {
        private readonly Dictionary<GameObject, Stack<GameObject>> _prefabToObjectMap = new();

        public T Create<T>(T prefab, Transform? onTransform = null) where T : MonoBehaviour
        {
            GameObject? instance = Create(prefab.gameObject, onTransform);
            if (instance != null)
            {
                var cmp = instance.GetComponent<T>();
                if (cmp)
                {
                    return cmp;
                }

                throw new InvalidOperationException(
                    $"Prefab {prefab.name} does not have the required component {typeof(T).Name}.");
            }

            throw new InvalidOperationException($"Prefab could not be instantiated from {prefab.name}");
        }

        public GameObject Create(GameObject prefab, Transform? onTransform = null)
        {
            if (_prefabToObjectMap.TryGetValue(prefab, out Stack<GameObject> pool))
            {
                if (pool.Count > 0)
                {
                    var instance = pool.Pop();
                    instance.transform.parent = onTransform;
                    instance.SetActive(true);
                    return instance;
                }
            }

            GameObject? newInstance = Instantiate(prefab, onTransform);
            if (newInstance == null)
            {
                throw new InvalidOperationException($"Could not instantiate prefab: {prefab.name}");
            }

            newInstance.SetActive(true);
            return newInstance;
        }

        public void Recycle<T>(T prefab, T instance) where T : MonoBehaviour
        {
            Recycle(prefab.gameObject, instance.gameObject);
        }

        public void Recycle(GameObject prefab, GameObject instance)
        {
            if (!_prefabToObjectMap.TryGetValue(prefab, out Stack<GameObject> pool))
            {
                pool = new Stack<GameObject>();
                _prefabToObjectMap[prefab] = pool;
            }

            instance.SetActive(false);
            instance.transform.parent = transform;
            pool.Push(instance);
        }
    }
}