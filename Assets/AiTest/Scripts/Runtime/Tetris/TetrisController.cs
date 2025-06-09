using Lumley.AiTest.GameShared;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lumley.AiTest.Tetris
{
    public class TetrisController : BaseGameController
    {
        [Header("Tetris Specific")] public Transform gridParent;
        public BlockPool blockPool;
        public TetrisPiece[] piecePrefabs;

        private TetrisGrid grid;
        private TetrisPiece currentPiece;
        private TetrisPiece nextPiece;
        private float fallTimer = 0f;
        private float fallSpeed;
        private int linesCleared = 0;
        private int targetLines;

        protected override void InitializeGame(GameManager.Difficulty difficulty)
        {
            var config = GameManager.Instance.gameConfig.tetrisConfig;
            fallSpeed = config.fallSpeeds[(int)GameManager.Instance.CurrentDifficulty];
            targetLines = config.linesToWin[(int)GameManager.Instance.CurrentDifficulty];

            grid = new TetrisGrid(config.gridWidth, config.gridHeight);
            SpawnNewPiece();
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
                nextPiece = Instantiate(piecePrefabs[Random.Range(0, piecePrefabs.Length)]);

            currentPiece = nextPiece;
            nextPiece = Instantiate(piecePrefabs[Random.Range(0, piecePrefabs.Length)]);

            currentPiece.Initialize(new Vector2Int(5, 18), blockPool);

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