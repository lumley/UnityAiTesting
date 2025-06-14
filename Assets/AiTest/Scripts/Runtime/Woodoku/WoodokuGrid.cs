using System.Collections.Generic;
using Lumley.AiTest.GameShared;
using UnityEngine;

namespace Lumley.AiTest.Woodoku
{
    public sealed class WoodokuGrid
    {
        private readonly Block?[,] _grid;
        private readonly int _size;
        private readonly Vector2 _blockSize;

        public WoodokuGrid(int gridSize, Vector2 blockSize)
        {
            _size = gridSize;
            _blockSize = blockSize;
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

        public int ClearCompletedLinesOrSquareAreas()
        {
            int linesOrSquaresCleared = 0;
            var rowsToClear = new List<int>();
            var colsToClear = new List<int>();
            var squaresToClear = new List<(int, int)>();

            // Register full rows
            for (int y = 0; y < _size; y++)
            {
                if (IsRowFull(y))
                {
                    rowsToClear.Add(y);
                }
            }

            // Register full columns
            for (int x = 0; x < _size; x++)
            {
                if (IsColumnFull(x))
                {
                    colsToClear.Add(x);
                }
            }

            // Register full 3x3 squares
            int squareSize = 3;
            for (int startX = 0; startX < _size; startX += squareSize)
            {
                for (int startY = 0; startY < _size; startY += squareSize)
                {
                    if (IsSquareFull(startX, startY, squareSize))
                    {
                        squaresToClear.Add((startX, startY));
                    }
                }
            }

            // Clear registered rows
            foreach (var y in rowsToClear)
            {
                ClearRow(y);
                linesOrSquaresCleared++;
            }

            // Clear registered columns
            foreach (var x in colsToClear)
            {
                ClearColumn(x);
                linesOrSquaresCleared++;
            }

            // Clear registered squares
            foreach (var (startX, startY) in squaresToClear)
            {
                ClearSquare(startX, startY, squareSize);
                linesOrSquaresCleared++;
            }

            return linesOrSquaresCleared;
        }
        
        private bool IsSquareFull(int startX, int startY, int squareSize)
        {
            for (int x = startX; x < startX + squareSize && x < _size; x++)
            {
                for (int y = startY; y < startY + squareSize && y < _size; y++)
                {
                    if (_grid[x, y] == null)
                        return false;
                }
            }
            return true;
        }

        private void ClearSquare(int startX, int startY, int squareSize)
        {
            for (int x = startX; x < startX + squareSize && x < _size; x++)
            {
                for (int y = startY; y < startY + squareSize && y < _size; y++)
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

        private bool IsValidPosition(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < _size && pos.y >= 0 && pos.y < _size;
        }

        private Vector2Int WorldToGrid(Vector2 worldPos)
        {
            return new Vector2Int(Mathf.RoundToInt(worldPos.x / _blockSize.x), Mathf.RoundToInt(worldPos.y / _blockSize.y));
        }

        private Vector2 GridToWorld(Vector2Int gridPos)
        {
            return new Vector2(gridPos.x * _blockSize.x, gridPos.y * _blockSize.y);
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