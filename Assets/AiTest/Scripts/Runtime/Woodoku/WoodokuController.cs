using System.Collections.Generic;
using Lumley.AiTest.GameShared;
using UnityEngine;

namespace Lumley.AiTest.Woodoku
{
    public class WoodokuController : BaseGameController
    {
        [Header("Woodoku Specific")] public Transform gridParent;
        public Transform pieceArea;
        public ObjectPool<Block> blockPool;

        private WoodokuGrid grid;
        private List<WoodokuPiece> availablePieces = new List<WoodokuPiece>();
        private int currentScore = 0;
        private int targetScore;
        private int piecesRemaining;

        protected override void InitializeGame()
        {
            var config = GameManager.Instance.gameConfig.woodokuConfig;
            targetScore = config.targetScores[(int)GameManager.Instance.CurrentDifficulty];
            piecesRemaining = config.piecesCount[(int)GameManager.Instance.CurrentDifficulty];

            grid = new WoodokuGrid(config.gridSize);
            SpawnNewPieces();
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
                HandlePiecePlacement(mousePos);
            }
        }

        private void HandlePiecePlacement(Vector2 worldPos)
        {
            // Find selected piece and try to place it
            foreach (var piece in availablePieces)
            {
                if (piece.IsSelected && grid.CanPlacePiece(piece, worldPos))
                {
                    grid.PlacePiece(piece, worldPos);
                    currentScore += piece.GetScore();
                    availablePieces.Remove(piece);
                    piecesRemaining--;

                    CheckForCompletedLines();

                    if (availablePieces.Count == 0 && piecesRemaining > 0)
                    {
                        SpawnNewPieces();
                    }

                    break;
                }
            }
        }

        private void SpawnNewPieces()
        {
            availablePieces.Clear();

            int piecesToSpawn = Mathf.Min(3, piecesRemaining);
            for (int i = 0; i < piecesToSpawn; i++)
            {
                WoodokuPiece newPiece = CreateRandomPiece();
                availablePieces.Add(newPiece);
            }
        }

        private WoodokuPiece CreateRandomPiece()
        {
            // Create different piece shapes
            Vector2Int[][] shapes =
            {
                new[] { Vector2Int.zero }, // Single block
                new[] { Vector2Int.zero, Vector2Int.right }, // Two blocks
                new[] { Vector2Int.zero, Vector2Int.right, Vector2Int.up }, // L-shape
                new[] { Vector2Int.zero, Vector2Int.right, Vector2Int.up, new Vector2Int(1, 1) } // Square
            };

            GameObject pieceObj = new GameObject("WoodokuPiece");
            WoodokuPiece piece = pieceObj.AddComponent<WoodokuPiece>();
            piece.Initialize(shapes[Random.Range(0, shapes.Length)]);

            return piece;
        }

        private void CheckForCompletedLines()
        {
            int linesCleared = grid.ClearCompletedLines();
            currentScore += linesCleared * 100; // Bonus for clearing lines
        }

        private void CheckWinCondition()
        {
            if (currentScore >= targetScore)
            {
                HandleWin();
            }
            else if (piecesRemaining <= 0 && availablePieces.Count == 0)
            {
                HandleLose();
            }
        }

        protected override void HandleWin()
        {
            winPanel?.SetActive(true);
            GameManager.Instance.CompleteGame(true);
        }

        protected override void HandleLose()
        {
            losePanel?.SetActive(true);
            GameManager.Instance.CompleteGame(false);
        }
    }
}