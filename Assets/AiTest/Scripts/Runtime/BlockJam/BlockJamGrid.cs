using UnityEngine;

namespace Lumley.AiTest.BlockJam
{
    public class BlockJamGrid
    {
        private BlockJamPiece[,] grid;
        private int width;
        private int height;
        private Vector2Int exitPosition;

        public int Width => width;
        public int Height => height;

        public BlockJamGrid(int w, int h)
        {
            width = w;
            height = h;
            grid = new BlockJamPiece[width, height];
            exitPosition = new Vector2Int(width - 1, height / 2); // Right side, middle
        }

        public bool CanMovePiece(BlockJamPiece piece, Vector2Int direction)
        {
            Vector2Int newPos = piece.GetPosition() + direction;

            // Check bounds
            if (newPos.x < 0 || newPos.y < 0 ||
                newPos.x + piece.GetSize().x > width ||
                newPos.y + piece.GetSize().y > height)
                return false;

            // Check collision with other pieces
            for (int x = 0; x < piece.GetSize().x; x++)
            {
                for (int y = 0; y < piece.GetSize().y; y++)
                {
                    Vector2Int checkPos = newPos + new Vector2Int(x, y);
                    if (grid[checkPos.x, checkPos.y] != null && grid[checkPos.x, checkPos.y] != piece)
                        return false;
                }
            }

            return true;
        }

        public void MovePiece(BlockJamPiece piece, Vector2Int direction)
        {
            // Clear old position
            ClearPieceFromGrid(piece);

            // Update piece position
            piece.Move(direction);

            // Place at new position
            PlacePiece(piece);
        }

        public void PlacePiece(BlockJamPiece piece)
        {
            Vector2Int pos = piece.GetPosition();
            Vector2Int size = piece.GetSize();

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Vector2Int gridPos = pos + new Vector2Int(x, y);
                    if (IsValidPosition(gridPos))
                        grid[gridPos.x, gridPos.y] = piece;
                }
            }
        }

        public void ClearPieceFromGrid(BlockJamPiece piece)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid[x, y] == piece)
                        grid[x, y] = null;
                }
            }
        }

        public bool IsAreaFree(Vector2Int position, Vector2Int size)
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Vector2Int checkPos = position + new Vector2Int(x, y);
                    if (!IsValidPosition(checkPos) || grid[checkPos.x, checkPos.y] != null)
                        return false;
                }
            }

            return true;
        }

        public bool IsPieceAtExit(BlockJamPiece piece)
        {
            Vector2Int piecePos = piece.GetPosition();
            return piecePos.x + piece.GetSize().x > exitPosition.x &&
                   piecePos.y == exitPosition.y;
        }

        public Vector2Int WorldToGrid(Vector2 worldPos)
        {
            return new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y));
        }

        private bool IsValidPosition(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
        }
    }
}