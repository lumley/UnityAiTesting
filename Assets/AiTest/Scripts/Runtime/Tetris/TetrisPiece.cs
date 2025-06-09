using Lumley.AiTest.GameShared;
using UnityEngine;

namespace Lumley.AiTest.Tetris
{
    public class TetrisPiece : MonoBehaviour
    {
        [Header("Piece Configuration")] public Vector2Int[] blockPositions;
        public Color pieceColor = Color.white;

        private Vector2Int position;
        private Block[] blocks;
        private ObjectPool<Block> blockPool;

        public void Initialize(Vector2Int startPos, ObjectPool<Block> pool)
        {
            position = startPos;
            blockPool = pool;

            blocks = new Block[blockPositions.Length];
            for (int i = 0; i < blockPositions.Length; i++)
            {
                Block block;
                if (blockPool != null)
                {
                    block = blockPool.GetObject();
                    block.gameObject.name = $"Block_{i}";
                    block.transform.SetParent(transform);
                }
                else
                {
                    GameObject blockObj = new GameObject($"Block_{i}");
                    blockObj.transform.SetParent(transform);
                    block = blockObj.AddComponent<Block>();
                }

                blocks[i] = block;
                blocks[i].Initialize(Block.BlockType.Standard, pieceColor);

                UpdateBlockPosition(i);
            }
        }

        public bool Move(Vector2Int direction, TetrisGrid grid)
        {
            Vector2Int newPos = position + direction;

            if (IsValidPosition(newPos, grid))
            {
                position = newPos;
                UpdateAllBlockPositions();
                return true;
            }

            return false;
        }

        public void Rotate(TetrisGrid grid)
        {
            // Simple 90-degree rotation
            Vector2Int[] rotatedPositions = new Vector2Int[blockPositions.Length];

            for (int i = 0; i < blockPositions.Length; i++)
            {
                rotatedPositions[i] = new Vector2Int(-blockPositions[i].y, blockPositions[i].x);
            }

            if (IsValidRotation(rotatedPositions, grid))
            {
                blockPositions = rotatedPositions;
                UpdateAllBlockPositions();
            }
        }

        public bool IsValidPosition(TetrisGrid grid)
        {
            return IsValidPosition(position, grid);
        }

        private bool IsValidPosition(Vector2Int pos, TetrisGrid grid)
        {
            foreach (var blockPos in blockPositions)
            {
                Vector2Int worldPos = pos + blockPos;
                if (!grid.IsValidPosition(worldPos))
                    return false;
            }

            return true;
        }

        private bool IsValidRotation(Vector2Int[] rotatedPos, TetrisGrid grid)
        {
            foreach (var blockPos in rotatedPos)
            {
                Vector2Int worldPos = position + blockPos;
                if (!grid.IsValidPosition(worldPos))
                    return false;
            }

            return true;
        }

        public void PlaceOnGrid(TetrisGrid grid)
        {
            for (int i = 0; i < blocks.Length; i++)
            {
                Vector2Int worldPos = position + blockPositions[i];
                grid.SetBlock(worldPos, blocks[i]);
                blocks[i].transform.SetParent(null); // Remove from piece hierarchy
            }
        }

        private void UpdateAllBlockPositions()
        {
            for (int i = 0; i < blocks.Length; i++)
            {
                UpdateBlockPosition(i);
            }
        }

        private void UpdateBlockPosition(int blockIndex)
        {
            Vector2Int worldPos = position + blockPositions[blockIndex];
            blocks[blockIndex].transform.position = new Vector3(worldPos.x, worldPos.y, 0);
        }
    }
}