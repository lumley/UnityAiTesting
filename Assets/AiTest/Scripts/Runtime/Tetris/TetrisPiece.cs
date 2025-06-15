using System;
using Lumley.AiTest.GameShared;
using UnityEngine;

namespace Lumley.AiTest.Tetris
{
    public class TetrisPiece : MonoBehaviour
    {
        [Header("Piece Configuration")] [SerializeField]
        private Vector2Int[] _blockPositions = { };

        [SerializeField] private Block _blockPrefab = null!;

        private Vector2Int _position;
        private Block[] _blocks = { };
        private PoolingManager? _pool;
        private TetrisPiece? _myOwnPrefab;

        public void Initialize(PoolingManager pool, TetrisPiece myOwnPrefab)
        {
            _pool = pool;
            _myOwnPrefab = myOwnPrefab;
            transform.localPosition = Vector3.zero;
        }

        public void SpawnInnerBlocks(Vector2Int startPos, PoolingManager pool)
        {
            _position = startPos;

            _blocks = new Block[_blockPositions.Length];
            for (int i = 0; i < _blockPositions.Length; i++)
            {
                var block = pool.GetOrCreate(_blockPrefab, transform);
#if UNITY_EDITOR
                block.gameObject.name = $"Block_{i}";
#endif
                _blocks[i] = block;
                _blocks[i].Initialize(Color.white, pool, _blockPrefab.gameObject);

                UpdateBlockPosition(i);
            }
        }

        public bool Move(Vector2Int direction, TetrisGrid grid)
        {
            Vector2Int newPos = _position + direction;

            if (IsValidPosition(newPos, grid))
            {
                _position = newPos;
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
            return IsValidPosition(_position, grid);
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
                Vector2Int worldPos = _position + blockPos;
                if (!grid.IsValidPosition(worldPos))
                    return false;
            }

            return true;
        }

        public void PlaceOnGrid(TetrisGrid grid)
        {
            for (int i = 0; i < _blocks.Length; i++)
            {
                Vector2Int worldPos = _position + _blockPositions[i];
                grid.SetBlock(worldPos, _blocks[i]);
                _blocks[i].transform.SetParent(grid.GridParent, worldPositionStays: true); // Remove from piece
            }
            _blocks = Array.Empty<Block>();
            Recycle();
        }
        
        public void RecycleInnerBlocks()
        {
            if (_pool != null && _myOwnPrefab != null)
            {
                foreach (var block in _blocks)
                {
                    block.OnBlockDestroyed();
                }
                _blocks = Array.Empty<Block>();
            }
        }

        public void Recycle()
        {
            if (_pool != null && _myOwnPrefab != null)
            {
                RecycleInnerBlocks();
                _pool.Recycle(_myOwnPrefab, this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void UpdateAllBlockPositions()
        {
            for (int i = 0; i < _blocks.Length; i++)
            {
                UpdateBlockPosition(i);
            }
        }

        private void UpdateBlockPosition(int blockIndex)
        {
            Vector2Int logicalPos = _position + _blockPositions[blockIndex];
            var block = _blocks[blockIndex];
            var blockBounds = block.GetBounds();
            var size = blockBounds.size;
            block.transform.localPosition = new Vector3(logicalPos.x * size.x, logicalPos.y * size.y, 0);
        }

        public Vector2Int GetMaxLogicalSize()
        {
            Vector2Int maxSize = Vector2Int.zero;
            foreach (var pos in _blockPositions)
            {
                maxSize.x = Mathf.Max(maxSize.x, pos.x);
                maxSize.y = Mathf.Max(maxSize.y, pos.y);
            }
            return maxSize + Vector2Int.one; // Add one to include the block itself
        }
        
        public Vector2Int GetMinLogicalSize()
        {
            Vector2Int minSize = Vector2Int.zero;
            foreach (var pos in _blockPositions)
            {
                minSize.x = Mathf.Min(minSize.x, pos.x);
                minSize.y = Mathf.Min(minSize.y, pos.y);
            }
            return minSize; // No need to add one here
        }
    }
}