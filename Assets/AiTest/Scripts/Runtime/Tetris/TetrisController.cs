using System.Threading.Tasks;
using Lumley.AiTest.GameShared;
using Lumley.AiTest.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lumley.AiTest.Tetris
{
    public class TetrisController : BaseGameController
    {
        [Header("Tetris Config")] [SerializeField]
        private TetrisGameConfig _config = null!;

        [SerializeField] private Transform _gridParent = null!;
        [SerializeField] private Transform _previewGridParent = null!;

        [SerializeField] private TetrisPiece[] _piecePrefabs = { };

        [Header("Utilities")] [SerializeField] private PoolingManager _poolingManager = null!;

        [SerializeField] private GameObject _gridCellPrefab = null!;

        [Header("Camera Settings")] [SerializeField]
        private Camera _camera = null!;

        [SerializeField, Tooltip("Percentage of camera margin relative to the board size")]
        private float _cameraDistanceFactor = 0.1f;

        [SerializeField] private Block _referenceBlock = null!;

        [Header("HUD")] [SerializeField] private TMP_Text _objectiveLinesText = null!;
        [SerializeField] private TMP_Text _currentLinesText = null!;

        private TetrisGrid _grid = null!;
        private TetrisPiece? _currentPiece;
        private TetrisPiece? _nextPiece;
        private float _fallTimer;
        private float _fallSpeed;
        private int _linesCleared;
        private int _targetLines;

        protected override Task InitializeGameAsync(GameDifficulty difficulty)
        {
            _fallSpeed = _config.FallSpeeds[(int)difficulty];
            _targetLines = _config.LinesToWin[(int)difficulty];

            _currentLinesText.text = "0";
            _objectiveLinesText.text = _targetLines.ToString();

            _grid = new TetrisGrid(_config.GridWidth, _config.GridHeight, _gridParent);
            var blockSize = _referenceBlock.GetBounds().size;
            _camera.CenterCameraOnGrid(blockSize, new Vector2Int(_config.GridWidth, _config.GridHeight), new Vector2(0.2f, 0.5f), _cameraDistanceFactor);
            
            // Move the preview grid top left of the grid
            _previewGridParent.localPosition = new Vector3(-_config.GridWidth * blockSize.x * 0.5f,
                _config.GridHeight * blockSize.y * 0.5f, 0f);

            SpawnNewPiece();
            DrawGrid();
            DrawPreviewGrid();
            
            return Task.CompletedTask;
        }

        private void DrawPreviewGrid()
        {
            // Get the maximum block size from _piecePrefabs by traversing each piece's bounds.
            Vector2Int maxBlockSize = Vector2Int.one;
            foreach (var piecePrefab in _piecePrefabs)
            {
                Vector2Int logicalBlockSize = piecePrefab.GetMaxLogicalSize();
                maxBlockSize.x = Mathf.Max(maxBlockSize.x, logicalBlockSize.x);
                maxBlockSize.y = Mathf.Max(maxBlockSize.y, logicalBlockSize.y);
            }
            maxBlockSize = new Vector2Int(Mathf.Max(maxBlockSize.x, maxBlockSize.y), Mathf.Max(maxBlockSize.x, maxBlockSize.y));
            
            var blockSize = _referenceBlock.GetBounds().size;
            for (int x = 0; x < maxBlockSize.x; x++)
            {
                for (int y = 0; y < maxBlockSize.y; y++)
                {
                    var cellPosition = new Vector3(x * blockSize.x, y * blockSize.y, 10f);
                    var cell = Instantiate(_gridCellPrefab, _previewGridParent);
                    cell.transform.localPosition = cellPosition;
                }
            }
        }

        private void DrawGrid()
        {
            var blockSize = _referenceBlock.GetBounds().size;
            for (int x = 0; x < _config.GridWidth; x++)
            {
                for (int y = 0; y < _config.GridHeight; y++)
                {
                    var cellPosition = new Vector3(x * blockSize.x, y * blockSize.y, 10f);
                    var cell = Instantiate(_gridCellPrefab, _gridParent);
                    cell.transform.localPosition = cellPosition;
                }
            }
        }

        protected override void UpdateGameplay()
        {
            HandleInput();
            HandleFalling();
        }

        private void HandleInput()
        {
            if (_currentPiece == null) return;

            var touchscreen = Touchscreen.current;
            if (touchscreen != null)
            {
                var touches = touchscreen.touches;
                foreach (var touch in touches)
                {
                    if (touch.press.wasPressedThisFrame)
                    {
                        Vector2 touchPos = touch.position.ReadValue();
                        // left half = move left, right half = move right, top = rotate, bottom = fast drop
                        if (touchPos.x < Screen.width * 0.25f)
                        {
                            _currentPiece.Move(Vector2Int.left, _grid);
                        }
                        else if (touchPos.x > Screen.width * 0.75f)
                        {
                            _currentPiece.Move(Vector2Int.right, _grid);
                        }
                        else if (touchPos.y > Screen.height * 0.75f)
                        {
                            _currentPiece.Rotate(_grid);
                        }
                        else if (touchPos.y < Screen.height * 0.25f)
                        {
                            _fallTimer += _fallSpeed; // Fast drop
                        }
                    }
                }
            }

            var move = Keyboard.current;
            if (move != null)
            {
                if (move.aKey.wasPressedThisFrame || move.leftArrowKey.wasPressedThisFrame)
                {
                    _currentPiece.Move(Vector2Int.left, _grid);
                }

                if (move.dKey.wasPressedThisFrame || move.rightArrowKey.wasPressedThisFrame)
                {
                    _currentPiece.Move(Vector2Int.right, _grid);
                }

                if (move.sKey.wasPressedThisFrame || move.downArrowKey.wasPressedThisFrame)
                {
                    _fallTimer += _fallSpeed; // Fast drop
                }

                if (move.wKey.wasPressedThisFrame || move.upArrowKey.wasPressedThisFrame)
                {
                    _currentPiece.Rotate(_grid);
                }
            }
        }

        private void HandleFalling()
        {
            _fallTimer += Time.deltaTime;

            if (_fallTimer >= _fallSpeed)
            {
                if (_currentPiece != null && !_currentPiece.Move(Vector2Int.down, _grid))
                {
                    PlacePiece();
                    CheckLines();
                    SpawnNewPiece();
                }

                _fallTimer = 0f;
            }
        }

        private void SpawnNewPiece()
        {
            if (_nextPiece == null)
            {
                var tetrisPiecePrefab = _piecePrefabs[Random.Range(0, _piecePrefabs.Length)];
                _nextPiece = _poolingManager.GetOrCreate(tetrisPiecePrefab, _previewGridParent);
                _nextPiece.Initialize(_poolingManager, tetrisPiecePrefab);
            }

            _currentPiece = _nextPiece;
            _currentPiece.RecycleInnerBlocks();
            _currentPiece.transform.SetParent(_gridParent, worldPositionStays: false);

            var piecePrefab = _piecePrefabs[Random.Range(0, _piecePrefabs.Length)];
            _nextPiece = _poolingManager.GetOrCreate(piecePrefab, _previewGridParent);
            _nextPiece.Initialize(_poolingManager, piecePrefab);
            _nextPiece.SpawnInnerBlocks(Vector2Int.up, _poolingManager);

            // Spawn the blocks at the top-center of the grid, down enough that it fits the piece
            Vector2Int min = _currentPiece.GetMinLogicalSize();
            Vector2Int max = _currentPiece.GetMaxLogicalSize();
            int pieceWidth = max.x - min.x + 1;
            int spawnX = (_config.GridWidth - pieceWidth) / 2 - min.x;
            int spawnY = _config.GridHeight - 1 - max.y;
            Vector2Int spawnPoint = new Vector2Int(spawnX, spawnY);
            _currentPiece.SpawnInnerBlocks(spawnPoint, _poolingManager);
        }

        private void PlacePiece()
        {
            var isCurrentPieceNull = _currentPiece == null;
            if (isCurrentPieceNull || !_currentPiece!.IsValidPosition(_grid))
            {
                HandleLose();
            }

            if (!isCurrentPieceNull)
            {
                _currentPiece!.PlaceOnGrid(_grid);
            }

            _currentPiece = null;
        }

        private void CheckLines()
        {
            int clearedThisTurn = _grid.ClearCompletedLines();
            _linesCleared += clearedThisTurn;
            _currentLinesText.text = _linesCleared.ToString();

            if (_linesCleared >= _targetLines)
            {
                HandleWin();
            }
        }
    }
}