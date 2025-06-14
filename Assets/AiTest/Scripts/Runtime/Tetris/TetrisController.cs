using System.Threading.Tasks;
using Lumley.AiTest.GameShared;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lumley.AiTest.Tetris
{
    public class TetrisController : BaseGameController
    {
        [Header("Tetris Specific")] [SerializeField]
        private TetrisGameConfig _config = null!;

        [SerializeField] private PoolingManager poolingManager = null!;

        [SerializeField] private LineRenderer _lineRenderer = null!;

        [SerializeField] private Camera _camera = null!;

        [SerializeField] private Block _referenceBlock = null!;

        public Transform gridParent;
        public TetrisPiece[] piecePrefabs;

        private TetrisGrid grid;
        private TetrisPiece currentPiece;
        private TetrisPiece nextPiece;
        private float fallTimer = 0f;
        private float fallSpeed;
        private int linesCleared = 0;
        private int targetLines;

        protected override Task InitializeGameAsync(GameDifficulty difficulty)
        {
            fallSpeed = _config.fallSpeeds[(int)difficulty];
            targetLines = _config.linesToWin[(int)difficulty];

            grid = new TetrisGrid(_config.gridWidth, _config.gridHeight);
            var blockSize = _referenceBlock.GetBounds().size;
            var originalCameraPositionZ = _camera.transform.position.z;
            _camera.transform.position = new Vector3(+_config.gridWidth * blockSize.x / 2f,
                +_config.gridHeight * blockSize.y / 2f, originalCameraPositionZ);
            _camera.orthographicSize = Mathf.Max(_config.gridWidth * blockSize.x, _config.gridHeight * blockSize.y) / 2f;

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
                _lineRenderer.SetPosition(idx++, new Vector3(x * blockWidth, 0, 0));
                _lineRenderer.SetPosition(idx++, new Vector3(x * blockWidth, _config.gridHeight * blockHeight, 0));
            }

            // Horizontal lines
            for (int y = 0; y <= _config.gridHeight; y++)
            {
                _lineRenderer.SetPosition(idx++, new Vector3(0, y * blockHeight, 0));
                _lineRenderer.SetPosition(idx++, new Vector3(_config.gridWidth * blockWidth, y * blockHeight, 0));
            }
        }

        protected override void UpdateGameplay()
        {
            HandleInput();
            HandleFalling();
        }

        protected override void HandleWin()
        {
            throw new System.NotImplementedException();
        }

        protected override void HandleLose()
        {
            // TODO (slumley): Implement game over logic
            Debug.Log("Game Over! You lost.", this);
        }

        private void HandleInput()
        {
            if (currentPiece == null) return;

            var move = Keyboard.current;
            if (move != null)
            {
                if (move.aKey.wasPressedThisFrame || move.leftArrowKey.wasPressedThisFrame)
                    currentPiece.Move(Vector2Int.left, grid);

                if (move.dKey.wasPressedThisFrame || move.rightArrowKey.wasPressedThisFrame)
                    currentPiece.Move(Vector2Int.right, grid);

                if (move.sKey.wasPressedThisFrame || move.downArrowKey.wasPressedThisFrame)
                    fallSpeed *= 10f; // Fast drop

                if (move.wKey.wasPressedThisFrame || move.upArrowKey.wasPressedThisFrame)
                    currentPiece.Rotate(grid);
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
                            fallSpeed *= 10f;
                    }
                }
            }
        }

        private void HandleFalling()
        {
            fallTimer += Time.deltaTime;

            if (fallTimer >= fallSpeed)
            {
                if (currentPiece != null && !currentPiece.Move(Vector2Int.down, grid))
                {
                    PlacePiece();
                    CheckLines();
                    SpawnNewPiece();
                }

                fallTimer = 0f;
            }
        }

        private void SpawnNewPiece()
        {
            if (nextPiece == null)
                nextPiece = Instantiate(piecePrefabs[Random.Range(0, piecePrefabs.Length)], gridParent);

            currentPiece = nextPiece;
            nextPiece = Instantiate(piecePrefabs[Random.Range(0, piecePrefabs.Length)], gridParent);

            currentPiece.Initialize(new Vector2Int(5, 18), poolingManager);

            if (!currentPiece.IsValidPosition(grid))
            {
                HandleLose();
            }
        }

        private void PlacePiece()
        {
            currentPiece.PlaceOnGrid(grid);
            currentPiece = null;
        }

        private void CheckLines()
        {
            int clearedThisTurn = grid.ClearCompletedLines();
            linesCleared += clearedThisTurn;

            if (linesCleared >= targetLines)
            {
                HandleWin();
            }
        }
    }
}