using Lumley.AiTest.GameShared;
using UnityEngine;

namespace Lumley.AiTest.Tetris
{
    public class TetrisGrid
    {
        private Block[,] grid;
        private int width;
        private int height;

        public TetrisGrid(int w, int h)
        {
            width = w;
            height = h;
            grid = new Block[width, height];
        }

        public bool IsValidPosition(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height && grid[pos.x, pos.y] == null;
        }

        public void SetBlock(Vector2Int pos, Block block)
        {
            if (IsValidPosition(pos))
                grid[pos.x, pos.y] = block;
        }

        public Block GetBlock(Vector2Int pos)
        {
            if (pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height)
                return grid[pos.x, pos.y];
            return null;
        }

        public int ClearCompletedLines()
        {
            int linesCleared = 0;

            for (int y = 0; y < height; y++)
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
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y] == null) return false;
            }

            return true;
        }

        private void ClearLine(int y)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y] != null)
                {
                    grid[x, y].OnBlockDestroyed();
                    grid[x, y] = null;
                }
            }
        }

        private void DropLinesAbove(int clearedY)
        {
            for (int y = clearedY + 1; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    grid[x, y - 1] = grid[x, y];
                    grid[x, y] = null;

                    if (grid[x, y - 1] != null)
                    {
                        grid[x, y - 1].transform.position += Vector3.down;
                    }
                }
            }
        }
    }
}