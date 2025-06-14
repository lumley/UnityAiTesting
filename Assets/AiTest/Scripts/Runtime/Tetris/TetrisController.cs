using System.Threading.Tasks;
using Lumley.AiTest.GameShared;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lumley.AiTest.Tetris
{
    public class TetrisController : BaseGameController
    {
        [Header("Tetris Config")] [SerializeField]
        private TetrisGameConfig _config = null!;

        [Header("Utilities")]
        [SerializeField] private PoolingManager poolingManager = null!;

        [SerializeField] private LineRenderer _lineRenderer = null!;

        [Header("Camera Settings")]
        [SerializeField] private Camera _camera = null!;

        [SerializeField, Tooltip("Percentage of camera margin relative to the board size")] private float _cameraDistanceFactor = 0.1f;

        [SerializeField] private Block _referenceBlock = null!;
        
        [Header("HUD")]
        [SerializeField] private TMP_Text _objectiveLinesText = null!;
        [SerializeField] private TMP_Text _currentLinesText = null!;

        public Transform _gridParent = null!;
        public TetrisPiece[] _piecePrefabs = {};

        private TetrisGrid grid;
        private TetrisPiece? currentPiece;
        private TetrisPiece nextPiece; // TODO (slumley): Create a grid for previews, place there
        private float _fallTimer;
        private float _fallSpeed;
        private int _linesCleared;
        private int _targetLines;

        protected override Task InitializeGameAsync(GameDifficulty difficulty)
        {
            _fallSpeed = _config.fallSpeeds[(int)difficulty];
            _targetLines = _config.linesToWin[(int)difficulty];

            _currentLinesText.text = "0";
            _objectiveLinesText.text = _targetLines.ToString();

            grid = new TetrisGrid(_config.gridWidth, _config.gridHeight, _gridParent);
            var blockSize = _referenceBlock.GetBounds().size;
            var originalCameraPositionZ = _camera.transform.position.z;
            _camera.transform.position = new Vector3(+_config.gridWidth * blockSize.x / 2f,
                +_config.gridHeight * blockSize.y / 2f, originalCameraPositionZ);
            _camera.orthographicSize = Mathf.Max(_config.gridWidth * blockSize.x, _config.gridHeight * blockSize.y) * (0.5f + _cameraDistanceFactor);

            SpawnNewPiece();
            DrawGrid();
            return Task.CompletedTask;
        }

        private void DrawGrid()
        {
            _lineRenderer.positionCount = (_config.gridWidth + 1) * 2 + (_config.gridHeight + 1) * 2;
            int idx = 0;
            var blockSize = _referenceBlock.GetBounds().size;
            var blockWidth = blockSize.x;
            var blockHeight = blockSize.y;

            // Vertical lines
            for (int x = 0; x <= _config.gridWidth; x++)
            {
                _lineRenderer.SetPosition(idx++, new Vector3((x - 0.5f) * blockWidth, -0.5f, 0));
                _lineRenderer.SetPosition(idx++, new Vector3((x - 0.5f) * blockWidth, (_config.gridHeight - 0.5f) * blockHeight, 0));
            }

            // Horizontal lines
            for (int y = 0; y <= _config.gridHeight; y++)
            {
                _lineRenderer.SetPosition(idx++, new Vector3(-0.5f, (y - 0.5f) * blockHeight, 0));
                _lineRenderer.SetPosition(idx++, new Vector3((_config.gridWidth - 0.5f) * blockWidth, (y - 0.5f) * blockHeight, 0));
            }
        }

        protected override void UpdateGameplay()
        {
            HandleInput();
            HandleFalling();
        }

        private void HandleInput()
        {
            if (currentPiece == null) return;

            var move = Keyboard.current;
            if (move != null)
            {
                if (move.aKey.wasPressedThisFrame || move.leftArrowKey.wasPressedThisFrame)
                {
                    currentPiece.Move(Vector2Int.left, grid);
                }

                if (move.dKey.wasPressedThisFrame || move.rightArrowKey.wasPressedThisFrame)
                {
                    currentPiece.Move(Vector2Int.right, grid);
                }

                if (move.sKey.wasPressedThisFrame || move.downArrowKey.wasPressedThisFrame)
                {
                    _fallTimer += _fallSpeed; // Fast drop
                }

                if (move.wKey.wasPressedThisFrame || move.upArrowKey.wasPressedThisFrame)
                {
                    currentPiece.Rotate(grid);
                }
            }

            // Touch controls for mobile
            if (Touchscreen.current != null)
            {
                var touches = Touchscreen.current.touches;
                foreach (var touch in touches)
                {
                    if (touch.press.wasPressedThisFrame)
                    {
                        Vector2 touchPos = touch.position.ReadValue();
                        // Example: left half = move left, right half = move right, top = rotate, bottom = fast drop
                        if (touchPos.x < Screen.width * 0.25f)
                            currentPiece.Move(Vector2Int.left, grid);
                        else if (touchPos.x > Screen.width * 0.75f)
                            currentPiece.Move(Vector2Int.right, grid);
                        else if (touchPos.y > Screen.height * 0.75f)
                            currentPiece.Rotate(grid);
                        else if (touchPos.y < Screen.height * 0.25f)
                            _fallSpeed *= 10f;
                    }
                }
            }
        }

        private void HandleFalling()
        {
            _fallTimer += Time.deltaTime;

            if (_fallTimer >= _fallSpeed)
            {
                if (currentPiece != null && !currentPiece.Move(Vector2Int.down, grid))
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
            if (nextPiece == null)
                nextPiece = Instantiate(_piecePrefabs[Random.Range(0, _piecePrefabs.Length)], _gridParent);

            currentPiece = nextPiece;
            nextPiece = Instantiate(_piecePrefabs[Random.Range(0, _piecePrefabs.Length)], _gridParent);

            currentPiece.Initialize(new Vector2Int(5, 18), poolingManager);
        }

        private void PlacePiece()
        {
            var isCurrentPieceNull = currentPiece == null;
            if (isCurrentPieceNull || !currentPiece!.IsValidPosition(grid))
            {
                HandleLose();
            }

            if (!isCurrentPieceNull)
            {
                currentPiece!.PlaceOnGrid(grid);
            }
            currentPiece = null;
        }

        private void CheckLines()
        {
            int clearedThisTurn = grid.ClearCompletedLines();
            _linesCleared += clearedThisTurn;
            _currentLinesText.text = _linesCleared.ToString();

            if (_linesCleared >= _targetLines)
            {
                HandleWin();
            }
        }
    }
}