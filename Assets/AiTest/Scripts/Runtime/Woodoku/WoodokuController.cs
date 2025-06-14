using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lumley.AiTest.GameShared;
using Lumley.AiTest.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lumley.AiTest.Woodoku
{
    public class WoodokuController : BaseGameController
    {
        [Header("Woodoku Specific")] public Transform _gridParent;
        public Transform _pieceArea;
        [SerializeField] private PoolingManager _poolingManager = null!;

        [SerializeField] private WoodokuGameConfig _config;
        
        [Header("Camera Settings")] [SerializeField]
        private Camera _camera = null!;

        [SerializeField, Tooltip("Percentage of camera margin relative to the board size")]
        private float _cameraDistanceFactor = 0.1f;
        
        [SerializeField] private Block _referenceBlock = null!;
        
        [Header("Pieces")]
        [SerializeField]
        private WoodokuPiece[] _piecePool = { };
        
        private WoodokuGrid grid;
        private List<WoodokuPiece> availablePieces = new();
        private int currentScore;
        private int targetScore;
        private int piecesRemaining;

        protected override Task InitializeGameAsync(GameDifficulty difficulty)
        {
            targetScore = _config.TargetScores[(int)difficulty];
            piecesRemaining = _config.PiecesCount[(int)difficulty];

            grid = new WoodokuGrid(_config.GridSize);
            var blockSize = _referenceBlock.GetBounds().size;
            _camera.CenterCameraOnGrid(
                blockSize,
                new Vector2Int(_config.GridSize, _config.GridSize),
                _cameraDistanceFactor);
            SpawnNewPieces();
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
                Vector2 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
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
                    availablePieces.Remove(piece);
                    currentScore += piece.GetScore();
                    grid.PlacePiece(piece, worldPos, _gridParent);
                    
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
            var selectedPrefab = _piecePool[Random.Range(0, _piecePool.Length)];
            var piece = _poolingManager.GetOrCreate(selectedPrefab, _pieceArea);

            piece.Initialize(_poolingManager, selectedPrefab.gameObject);

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
    }
}