using Lumley.AiTest.GameShared;
using UnityEngine;

namespace Lumley.AiTest.Tetris
{
    public class TetrisPiece : MonoBehaviour
    {
        [Header("Piece Configuration")] [SerializeField]
        private Vector2Int[] _blockPositions = { };

        [SerializeField] private Block _blockPrefab = null!;

        private Vector2Int position;
        private Block[] blocks = { };

        public void Initialize(Vector2Int startPos, PoolingManager pool)
        {
            position = startPos;

            blocks = new Block[_blockPositions.Length];
            for (int i = 0; i < _blockPositions.Length; i++)
            {
                var block = pool.GetOrCreate(_blockPrefab, transform);
                block.gameObject.name = $"Block_{i}";

                blocks[i] = block;
                blocks[i].Initialize(Color.white, pool, _blockPrefab.gameObject);

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
            Vector2Int[] rotatedPositions = new Vector2Int[_blockPositions.Length];

            for (int i = 0; i < _blockPositions.Length; i++)
            {
                rotatedPositions[i] = new Vector2Int(-_blockPositions[i].y, _blockPositions[i].x);
            }

            if (IsValidRotation(rotatedPositions, grid))
            {
                _blockPositions = rotatedPositions;
                UpdateAllBlockPositions();
            }
        }

        public bool IsValidPosition(TetrisGrid grid)
        {
            return IsValidPosition(position, grid);
        }

        private bool IsValidPosition(Vector2Int pos, TetrisGrid grid)
        {
            foreach (var blockPos in _blockPositions)
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
                Vector2Int worldPos = position + _blockPositions[i];
                grid.SetBlock(worldPos, blocks[i]);
                blocks[i].transform.SetParent(grid.GridParent, worldPositionStays: true); // Remove from piece
            }

            // TODO (slumley): Use pooling for the grid
            Destroy(gameObject); // Destroy the piece after placing it on the grid
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
            Vector2Int logicalPos = position + _blockPositions[blockIndex];
            var block = blocks[blockIndex];
            var blockBounds = block.GetBounds();
            var size = blockBounds.size;
            block.transform.position = new Vector3(logicalPos.x * size.x, logicalPos.y * size.y, 0);
        }
    }
}