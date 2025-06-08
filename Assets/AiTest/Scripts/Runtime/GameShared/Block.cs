namespace Lumley.AiTest.GameShared
{
    using UnityEngine;

    public class Block : MonoBehaviour
    {
        [Header("Block Properties")] public BlockType blockType;
        public Color blockColor = Color.white;
        public bool isMovable = true;

        private SpriteRenderer spriteRenderer;
        private Rigidbody2D rb;

        public enum BlockType
        {
            Standard,
            Special,
            Obstacle
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();

            if (spriteRenderer != null)
                spriteRenderer.color = blockColor;
        }

        public virtual void Initialize(BlockType type, Color color, bool movable = true)
        {
            blockType = type;
            blockColor = color;
            isMovable = movable;

            if (spriteRenderer != null)
                spriteRenderer.color = color;

            if (rb != null)
                rb.isKinematic = !movable;
        }

        public virtual void OnBlockDestroyed()
        {
            // Override in derived classes for special effects
            Destroy(gameObject);
        }
    }
}