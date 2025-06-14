using UnityEngine;

namespace Lumley.AiTest.Tetris
{
    [CreateAssetMenu(fileName = nameof(TetrisGameConfig), menuName = "Game/" + nameof(TetrisGameConfig))]
    public sealed class TetrisGameConfig : ScriptableObject
    {
        public float[] fallSpeeds = { 1f, 0.7f, 0.4f, 0.2f }; // Easy to Impossible
        public int[] linesToWin = { 5, 10, 20, 40 };
        public int gridWidth = 10;
        public int gridHeight = 20;
    }
}