using Lumley.AiTest.GameShared;
using UnityEngine;

namespace Lumley.AiTest.Woodoku
{
    public sealed class WoodokuGrid
    {
        private readonly Block?[,] _grid;
        private readonly int _size;

        public WoodokuGrid(int gridSize)
        {
            _size = gridSize;
            _grid = new Block[_size, _size];
        }

        public bool CanPlacePiece(WoodokuPiece piece, Vector2 worldPos)
        {
            Vector2Int gridPos = WorldToGrid(worldPos);

            foreach (var blockPos in piece.GetBlockPositions())
            {
                Vector2Int finalPos = gridPos + blockPos;
                if (!IsValidPosition(finalPos) || _grid[finalPos.x, finalPos.y] != null)
                    return false;
            }

            return true;
        }

        public void PlacePiece(WoodokuPiece piece, Vector2 worldPos, Transform gridParent)
        {
            Vector2Int gridPos = WorldToGrid(worldPos);

            var blockPositions = piece.GetBlockPositions();
            var blocks = piece.Blocks;
            for (var index = 0; index < blockPositions.Length; index++)
            {
                var block = blocks[index];
                var blockPos = blockPositions[index];
                Vector2Int finalPos = gridPos + blockPos;
                block.transform.SetParent(gridParent);

                _grid[finalPos.x, finalPos.y] = block;
                block.transform.position = GridToWorld(finalPos);
            }

            piece.Recycle();
        }

        public int ClearCompletedLines()
        {
            int linesCleared = 0;

            // Check rows
            for (int y = 0; y < _size; y++)
            {
                if (IsRowFull(y))
                {
                    ClearRow(y);
                    linesCleared++;
                }
            }

            // Check columns
            for (int x = 0; x < _size; x++)
            {
                if (IsColumnFull(x))
                {
                    ClearColumn(x);
                    linesCleared++;
                }
            }

            return linesCleared;
        }

        private bool IsValidPosition(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < _size && pos.y >= 0 && pos.y < _size;
        }

        private Vector2Int WorldToGrid(Vector2 worldPos)
        {
            // TODO (slumley): This needs a base block size to work correctly
            return new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y));
        }

        private Vector2 GridToWorld(Vector2Int gridPos)
        {
            return new Vector2(gridPos.x, gridPos.y);
        }

        private bool IsRowFull(int y)
        {
            for (int x = 0; x < _size; x++)
            {
                if (_grid[x, y] == null) return false;
            }

            return true;
        }

        private bool IsColumnFull(int x)
        {
            for (int y = 0; y < _size; y++)
            {
                if (_grid[x, y] == null) return false;
            }

            return true;
        }

        private void ClearRow(int y)
        {
            for (int x = 0; x < _size; x++)
            {
                var block = _grid[x, y];
                if (block != null)
                {
                    block.OnBlockDestroyed();
                    _grid[x, y] = null;
                }
            }
        }

        private void ClearColumn(int x)
        {
            for (int y = 0; y < _size; y++)
            {
                var block = _grid[x, y];
                if (block != null)
                {
                    block.OnBlockDestroyed();
                    _grid[x, y] = null;
                }
            }
        }
    }
}