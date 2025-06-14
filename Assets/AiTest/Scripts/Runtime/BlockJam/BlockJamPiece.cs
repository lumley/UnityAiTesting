using Lumley.AiTest.GameShared;
using UnityEngine;

namespace Lumley.AiTest.BlockJam
{
    public class BlockJamPiece : MonoBehaviour
    {
        private Vector2Int position;
        private Vector2Int size;
        private Color pieceColor;
        private bool isTarget;
        private bool isSelected;
        private Block[] blocks;

        public void Initialize(Vector2Int pos, Vector2Int pieceSize, Color color, bool target)
        {
            position = pos;
            size = pieceSize;
            pieceColor = color;
            isTarget = target;

            CreateVisualBlocks();
            UpdateVisualPosition();
        }

        private void CreateVisualBlocks()
        {
            blocks = new Block[size.x * size.y];

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    GameObject blockObj = new GameObject($"Block_{x}_{y}");
                    blockObj.transform.SetParent(transform);

                    Block block = blockObj.AddComponent<Block>();
                    block.Initialize(pieceColor);

                    blocks[x * size.y + y] = block;
                    block.transform.localPosition = new Vector3(x, y, 0);
                }
            }
        }

        public void Move(Vector2Int direction)
        {
            position += direction;
            UpdateVisualPosition();
        }

        private void UpdateVisualPosition()
        {
            transform.position = new Vector3(position.x, position.y, 0);
        }

        public void SetSelected(bool selected)
        {
            isSelected = selected;

            // Visual feedback for selection
            Color displayColor = selected ? Color.white : pieceColor;
            foreach (var block in blocks)
            {
                block.GetComponent<SpriteRenderer>().color = displayColor;
            }
        }

        public bool ContainsPosition(Vector2Int gridPos)
        {
            return gridPos.x >= position.x && gridPos.x < position.x + size.x &&
                   gridPos.y >= position.y && gridPos.y < position.y + size.y;
        }

        public Vector2Int GetPosition() => position;
        public Vector2Int GetSize() => size;
        public bool IsTarget() => isTarget;
    }
}