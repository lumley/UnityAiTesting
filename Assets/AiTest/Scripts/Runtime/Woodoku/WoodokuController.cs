using System.Collections.Generic;
using System.Threading.Tasks;
using Lumley.AiTest.GameShared;
using Lumley.AiTest.Utilities;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lumley.AiTest.Woodoku
{
    public class WoodokuController : BaseGameController
    {
        [Header("Woodoku Specific")] [SerializeField]
        private Transform _gridParent = null!;

        [SerializeField] private Transform _pieceArea = null!;
        [SerializeField] private PoolingManager _poolingManager = null!;
        [SerializeField] private SpriteRenderer _gridCellPrefab = null!;
        [SerializeField] private Color _gridCellColor1 = Color.sandyBrown;
        [SerializeField] private Color _gridCellColor2 = Color.rosyBrown;

        [SerializeField] private WoodokuGameConfig _config = null!;
        
        [Header("Camera Settings")] [SerializeField]
        private Camera _camera = null!;

        [SerializeField, Tooltip("Percentage of camera margin relative to the board size")]
        private float _cameraDistanceFactor = 0.1f;

        [SerializeField] private Block _referenceBlock = null!;

        [Header("Pieces")] [SerializeField] private WoodokuPiece[] _piecePool = { };
        
        [Header("HUD")] [SerializeField] private TMP_Text _objectivePointsText = null!;
        [SerializeField] private TMP_Text _currentPointsText = null!;
        [SerializeField] private TMP_Text _piecesLeftText = null!;
        [SerializeField] private string _piecesLeftTextFormat = "You have {0} pieces left";

        private WoodokuGrid _grid = null!; // Grid is only accessed internally and nothing runs until initialized, so it's safe to use null-forgiving operator here.
        private readonly List<WoodokuPiece> _availablePieces = new();
        private int _currentScore;
        private int _targetScore;
        private int _piecesRemaining;

        protected override Task InitializeGameAsync(GameDifficulty difficulty)
        {
            _targetScore = _config.TargetScores[(int)difficulty];
            SetPiecesRemaining(_config.PiecesCount[(int)difficulty]);
            
            _objectivePointsText.text = _targetScore.ToString();
            _currentPointsText.text = "0";

            var blockSize = _referenceBlock.GetBounds().size;
            _grid = new WoodokuGrid(_config.GridSize, blockSize);
            _camera.CenterCameraOnGrid(
                blockSize,
                new Vector2Int(_config.GridSize, _config.GridSize),
                new Vector2(0.45f, 0.4f),
                _cameraDistanceFactor);
            SpawnNewPieces();
            DrawGrid();
            return Task.CompletedTask;
        }

        protected override void UpdateGameplay()
        {
            HandleInput();
            CheckWinCondition();
        }

        private void HandleInput()
        {
            for (var index = _availablePieces.Count - 1; index >= 0; index--)
            {
                var availablePiece = _availablePieces[index];
                if (availablePiece.HasBeenDraggedAndDropped())
                {
                    HandlePiecePlacement(availablePiece);
                }
            }
        }

        private void HandlePiecePlacement(WoodokuPiece piece)
        {
            Vector2 worldPos = piece.transform.position;
            if (_grid.CanPlacePiece(piece, worldPos))
            {
                _availablePieces.Remove(piece);
                _currentScore += piece.GetScore();
                _currentPointsText.text = _currentScore.ToString();
                _grid.PlacePiece(piece, worldPos, _gridParent);

                SetPiecesRemaining(_piecesRemaining - 1);

                CheckForCompletedLines();

                if (_availablePieces.Count == 0 && _piecesRemaining > 0)
                {
                    SpawnNewPieces();
                }
            }
            else
            {
                piece.ResetPosition();
            }
        }

        private void DrawGrid()
        {
            var blockSize = _referenceBlock.GetBounds().size;
            for (int x = 0; x < _config.GridSize; x++)
            {
                for (int y = 0; y < _config.GridSize; y++)
                {
                    var cellPosition = new Vector3(x * blockSize.x, y * blockSize.y, 10f);
                    var cell = Instantiate(_gridCellPrefab.gameObject, _gridParent);
                    var spriteRenderer = cell.GetComponent<SpriteRenderer>();
                    bool isAltColor = (x / 3 + y / 3) % 2 == 0;
                    spriteRenderer.color = isAltColor ? _gridCellColor1 : _gridCellColor2;
                    cell.transform.localPosition = cellPosition;
                }
            }
        }

        private void SpawnNewPieces()
        {
            _availablePieces.Clear();
            var spawnAtPosition = Vector3.zero;

            int piecesToSpawn = Mathf.Min(3, _piecesRemaining);
            for (int i = 0; i < piecesToSpawn; i++)
            {
                WoodokuPiece newPiece = CreateRandomPiece();
                _availablePieces.Add(newPiece);

                newPiece.SetInitialPosition(spawnAtPosition);
                spawnAtPosition = new Vector3(
                    spawnAtPosition.x + newPiece.GetBounds().size.x * 1.5f, 
                    spawnAtPosition.y, 
                    spawnAtPosition.z);
            }
        }

        private WoodokuPiece CreateRandomPiece()
        {
            var selectedPrefab = _piecePool[Random.Range(0, _piecePool.Length)];
            var piece = _poolingManager.GetOrCreate(selectedPrefab, _pieceArea);

            piece.Initialize(_poolingManager, selectedPrefab.gameObject, _camera);

            return piece;
        }

        private void CheckForCompletedLines()
        {
            int linesCleared = _grid.ClearCompletedLinesOrSquareAreas();
            if (linesCleared > 0)
            {
                _currentScore += linesCleared * _config.LineOrSquareBonus;
                _currentPointsText.text = _currentScore.ToString();
            }
        }

        private void CheckWinCondition()
        {
            if (_currentScore >= _targetScore)
            {
                HandleWin();
            }
            else if (_piecesRemaining <= 0 && _availablePieces.Count == 0)
            {
                HandleLose();
            }
        }

        private void SetPiecesRemaining(int piecesRemaining)
        {
            _piecesRemaining = piecesRemaining;
            _piecesLeftText.text = string.Format(_piecesLeftTextFormat, _piecesRemaining);
        }
    }
}