using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lumley.AiTest.GameShared
{
    [Obsolete("Use Pooling Manager instead")]
    public class ObjectPool<T> : MonoBehaviour where T : Component
    {
        [Header("Pool Settings")] public T prefab;
        public int initialPoolSize = 10;
        public bool canExpand = true;

        private Queue<T> pool = new();
        private List<T> activeObjects = new();

        private void Start()
        {
            InitializePool();
        }

        private void InitializePool()
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                T obj = Instantiate(prefab, transform);
                obj.gameObject.SetActive(false);
                pool.Enqueue(obj);
            }
        }

        public T GetObject()
        {
            T obj;

            if (pool.Count > 0)
            {
                obj = pool.Dequeue();
            }
            else if (canExpand)
            {
                obj = Instantiate(prefab, transform);
            }
            else
            {
                return null;
            }

            obj.gameObject.SetActive(true);
            activeObjects.Add(obj);
            return obj;
        }

        public void ReturnObject(T obj)
        {
            if (activeObjects.Contains(obj))
            {
                activeObjects.Remove(obj);
                obj.gameObject.SetActive(false);
                pool.Enqueue(obj);
            }
        }

        public void ReturnAllObjects()
        {
            for (int i = activeObjects.Count - 1; i >= 0; i--)
            {
                ReturnObject(activeObjects[i]);
            }
        }
    }
}