using UnityEngine;

namespace Lumley.AiTest.GameShared
{
    /// <summary>
    /// A single block (a square) in any game.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class Block : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer = null!;
        
        public Color BlockColor; // TODO (slumley): Color is only used for color sort, give the sprite some sort of index so it can be checked in that game
        private PoolingManager? _poolingManager;
        private GameObject? _ownPrefab;

        public void Initialize(Color color, PoolingManager pool, GameObject ownPrefab)
        {
            BlockColor = color;
            _poolingManager = pool;
            _ownPrefab = ownPrefab;
        }

        public Bounds GetBounds()
        {
            return _spriteRenderer.sprite.bounds;
        }

        public void OnBlockDestroyed()
        {
            if (_poolingManager != null && _ownPrefab != null)
            {
                _poolingManager.Recycle(_ownPrefab, gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}