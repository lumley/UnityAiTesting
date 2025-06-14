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

        public void Initialize(Color color)
        {
            BlockColor = color;
            // TODO (slumley): Remove this? Keep at the moment to find usages in other games
        }

        public Bounds GetBounds()
        {
            return _spriteRenderer.sprite.bounds;
        }

        public void OnBlockDestroyed()
        {
            // TODO (slumley): No! Receive the pool and return itself here
            Destroy(gameObject);
        }
    }
}