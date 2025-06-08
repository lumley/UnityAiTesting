using Lumley.AiTest.GameShared;
using UnityEngine;

namespace Lumley.AiTest.Woodoku
{
    public class WoodokuGrid
    {
        private Block[,] grid;
        private int size;

        public WoodokuGrid(int gridSize)
        {
            size = gridSize;
            grid = new Block[size, size];
        }

        public bool CanPlacePiece(WoodokuPiece piece, Vector2 worldPos)
        {
            Vector2Int gridPos = WorldToGrid(worldPos);

            foreach (var blockPos in piece.GetBlockPositions())
            {
                Vector2Int finalPos = gridPos + blockPos;
                if (!IsValidPosition(finalPos) || grid[finalPos.x, finalPos.y] != null)
                    return false;
            }

            return true;
        }

        public void PlacePiece(WoodokuPiece piece, Vector2 worldPos)
        {
            Vector2Int gridPos = WorldToGrid(worldPos);

            foreach (var blockPos in piece.GetBlockPositions())
            {
                Vector2Int finalPos = gridPos + blockPos;
                if (IsValidPosition(finalPos))
                {
                    // Create and place block
                    GameObject blockObj = new GameObject("GridBlock");
                    Block block = blockObj.AddComponent<Block>();
                    block.Initialize(Block.BlockType.Standard, piece.GetColor());

                    grid[finalPos.x, finalPos.y] = block;
                    block.transform.position = GridToWorld(finalPos);
                }
            }
        }

        public int ClearCompletedLines()
        {
            int linesCleared = 0;

            // Check rows
            for (int y = 0; y < size; y++)
            {
                if (IsRowFull(y))
                {
                    ClearRow(y);
                    linesCleared++;
                }
            }

            // Check columns
            for (int x = 0; x < size; x++)
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
            return pos.x >= 0 && pos.x < size && pos.y >= 0 && pos.y < size;
        }

        private Vector2Int WorldToGrid(Vector2 worldPos)
        {
            return new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y));
        }

        private Vector2 GridToWorld(Vector2Int gridPos)
        {
            return new Vector2(gridPos.x, gridPos.y);
        }

        private bool IsRowFull(int y)
        {
            for (int x = 0; x < size; x++)
            {
                if (grid[x, y] == null) return false;
            }

            return true;
        }

        private bool IsColumnFull(int x)
        {
            for (int y = 0; y < size; y++)
            {
                if (grid[x, y] == null) return false;
            }

            return true;
        }

        private void ClearRow(int y)
        {
            for (int x = 0; x < size; x++)
            {
                if (grid[x, y] != null)
                {
                    grid[x, y].OnBlockDestroyed();
                    grid[x, y] = null;
                }
            }
        }

        private void ClearColumn(int x)
        {
            for (int y = 0; y < size; y++)
            {
                if (grid[x, y] != null)
                {
                    grid[x, y].OnBlockDestroyed();
                    grid[x, y] = null;
                }
            }
        }
    }
}