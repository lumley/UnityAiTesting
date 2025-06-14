using System.Collections.Generic;
using System.Threading.Tasks;
using Lumley.AiTest.GameShared;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lumley.AiTest.BlockJam
{
    public class BlockJamController : BaseGameController
    {
        [Header("Block Jam Specific")] [SerializeField]
        private Transform _gridParent = null!;

        [SerializeField] private Block _targetBlock = null!;
        [SerializeField] private Transform _exitZone = null!;
        [SerializeField] private BlockJamGameConfig _config = null!;

        private BlockJamGrid _grid = null!;
        private readonly List<BlockJamPiece> _pieces = new();
        private int _movesUsed;
        private int _maxMoves;
        private BlockJamPiece? _selectedPiece;

        protected override Task InitializeGameAsync(GameDifficulty difficulty)
        {
            _maxMoves = _config.MovesToWin[(int)difficulty];

            _grid = new BlockJamGrid(_config.GridWidth, _config.GridHeight);
            GeneratePuzzle(_config.ObstacleCount[(int)difficulty]);
            return Task.CompletedTask;
        }

        protected override void UpdateGameplay()
        {
            HandleInput();
            CheckWinCondition();
        }

        private void HandleInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                SelectPiece(mousePos);
            }

            if (_selectedPiece != null)
            {
                HandlePieceMovement();
            }
        }

        private void SelectPiece(Vector2 worldPos)
        {
            Vector2Int gridPos = _grid.WorldToGrid(worldPos);

            foreach (var piece in _pieces)
            {
                if (piece.ContainsPosition(gridPos))
                {
                    _selectedPiece = piece;
                    piece.SetSelected(true);
                    break;
                }
            }
        }

        private void HandlePieceMovement()
        {
            Vector2Int direction = Vector2Int.zero;

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                direction = Vector2Int.up;
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                direction = Vector2Int.down;
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                direction = Vector2Int.left;
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                direction = Vector2Int.right;

            if (direction != Vector2Int.zero && _selectedPiece != null && TryMovePiece(_selectedPiece, direction))
            {
                _movesUsed++;
                _selectedPiece.SetSelected(false);
                _selectedPiece = null;
            }
        }

        private bool TryMovePiece(BlockJamPiece piece, Vector2Int direction)
        {
            if (_grid.CanMovePiece(piece, direction))
            {
                _grid.MovePiece(piece, direction);
                return true;
            }

            return false;
        }

        private void GeneratePuzzle(int obstacleCount)
        {
            // Create target piece (the one that needs to reach the exit)
            GameObject targetObj = new GameObject("TargetPiece");
            BlockJamPiece targetPiece = targetObj.AddComponent<BlockJamPiece>();
            targetPiece.Initialize(new Vector2Int(1, 3), new Vector2Int(2, 1), Color.red, true);
            _pieces.Add(targetPiece);

            // Generate random obstacles
            for (int i = 0; i < obstacleCount; i++)
            {
                Vector2Int size = Random.Range(2, 4) == 2 ? new Vector2Int(1, 2) : new Vector2Int(2, 1);
                Vector2Int position = FindValidPosition(size);

                if (position != Vector2Int.one * -1) // Valid position found
                {
                    GameObject obstacleObj = new GameObject($"Obstacle_{i}");
                    BlockJamPiece obstacle = obstacleObj.AddComponent<BlockJamPiece>();
                    obstacle.Initialize(position, size, Random.ColorHSV(), false);
                    _pieces.Add(obstacle);
                }
            }

            // Place all pieces on grid
            foreach (var piece in _pieces)
            {
                _grid.PlacePiece(piece);
            }
        }

        private Vector2Int FindValidPosition(Vector2Int size)
        {
            int attempts = 0;
            while (attempts < 50)
            {
                Vector2Int pos = new Vector2Int(
                    Random.Range(0, _grid.Width - size.x + 1),
                    Random.Range(0, _grid.Height - size.y + 1)
                );

                if (_grid.IsAreaFree(pos, size))
                    return pos;

                attempts++;
            }

            return Vector2Int.one * -1; // Invalid position
        }

        private void CheckWinCondition()
        {
            // Check if target piece reached the exit
            BlockJamPiece targetPiece = _pieces[0]; // First piece is always target
            if (_grid.IsPieceAtExit(targetPiece))
            {
                HandleWin();
            }
            else if (_movesUsed >= _maxMoves)
            {
                HandleLose();
            }
        }
    }
}