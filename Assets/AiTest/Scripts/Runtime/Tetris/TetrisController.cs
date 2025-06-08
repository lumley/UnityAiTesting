using Lumley.AiTest.GameShared;
using UnityEngine;

namespace Lumley.AiTest.Tetris
{
    public class TetrisController : BaseGameController
    {
        [Header("Tetris Specific")] public Transform gridParent;
        public ObjectPool<Block> blockPool;
        public TetrisPiece[] piecePrefabs;

        private TetrisGrid grid;
        private TetrisPiece currentPiece;
        private TetrisPiece nextPiece;
        private float fallTimer = 0f;
        private float fallSpeed;
        private int linesCleared = 0;
        private int targetLines;

        protected override void InitializeGame()
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

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                currentPiece.Move(Vector2Int.left, grid);

            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                currentPiece.Move(Vector2Int.right, grid);

            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                fallSpeed *= 10f; // Fast drop

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                currentPiece.Rotate(grid);
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

            currentPiece.Initialize(new Vector2Int(5, 18));

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