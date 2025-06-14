using Lumley.AiTest.GameShared;
using UnityEngine;

namespace Lumley.AiTest.Tetris
{
    public class TetrisGrid
    {
        private readonly Block?[,] _grid;
        private readonly int _width;
        private readonly int _height;
        private readonly Transform _gridParent;
        
        public Transform GridParent => _gridParent;

        public TetrisGrid(int w, int h, Transform gridParent)
        {
            _width = w;
            _height = h;
            _gridParent = gridParent;
            _grid = new Block[_width, _height];
        }

        public bool IsValidPosition(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < _width && pos.y >= 0 && pos.y < _height && _grid[pos.x, pos.y] == null;
        }

        public void SetBlock(Vector2Int pos, Block block)
        {
            if (IsValidPosition(pos))
                _grid[pos.x, pos.y] = block;
        }

        public int ClearCompletedLines()
        {
            int linesCleared = 0;

            for (int y = 0; y < _height; y++)
            {
                if (IsLineFull(y))
                {
                    ClearLine(y);
                    DropLinesAbove(y);
                    linesCleared++;
                    y--; // Check the same line again
                }
            }

            return linesCleared;
        }

        private bool IsLineFull(int y)
        {
            for (int x = 0; x < _width; x++)
            {
                if (_grid[x, y] == null) return false;
            }

            return true;
        }

        private void ClearLine(int y)
        {
            for (int x = 0; x < _width; x++)
            {
                var block = _grid[x, y];
                if (block != null)
                {
                    block.OnBlockDestroyed();
                    _grid[x, y] = null;
                }
            }
        }

        private void DropLinesAbove(int clearedY)
        {
            for (int y = clearedY + 1; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    _grid[x, y - 1] = _grid[x, y];
                    _grid[x, y] = null;

                    var block = _grid[x, y - 1];
                    if (block != null)
                    {
                        block.transform.position += Vector3.down * block.GetBounds().size.y;
                    }
                }
            }
        }
    }
}