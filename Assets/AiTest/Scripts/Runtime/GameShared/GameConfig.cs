namespace Lumley.AiTest.GameShared
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Tetris Settings")] public TetrisConfig tetrisConfig;

        [Header("Woodoku Settings")] public WoodokuConfig woodokuConfig;

        [Header("Block Jam Settings")] public BlockJamConfig blockJamConfig;

        [Header("Color Sort Settings")] public ColorSortConfig colorSortConfig;

        [Header("UI Settings")] public float feedbackDisplayTime = 2f;
        public Color winColor = Color.green;
        public Color loseColor = Color.red;
    }

    [System.Serializable]
    public class TetrisConfig
    {
        public float[] fallSpeeds = { 1f, 0.7f, 0.4f, 0.2f }; // Easy to Impossible
        public int[] linesToWin = { 5, 10, 20, 40 };
        public int gridWidth = 10;
        public int gridHeight = 20;
    }

    [System.Serializable]
    public class WoodokuConfig
    {
        public int[] targetScores = { 500, 1000, 2000, 5000 };
        public int[] piecesCount = { 20, 15, 10, 5 }; // pieces to place
        public int gridSize = 9;
    }

    [System.Serializable]
    public class BlockJamConfig
    {
        public int[] movesToWin = { 50, 30, 20, 10 };
        public int[] obstacleCount = { 5, 10, 15, 25 };
        public int gridWidth = 8;
        public int gridHeight = 10;
    }

    [System.Serializable]
    public class ColorSortConfig
    {
        public int[] tubeCount = { 6, 8, 10, 12 };
        public int[] colorCount = { 4, 6, 8, 10 };
        public int tubeCapacity = 4;
    }
}