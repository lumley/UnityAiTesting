using UnityEngine;

namespace Lumley.AiTest.BlockJam
{
    [CreateAssetMenu(fileName = nameof(BlockJamGameConfig), menuName = "Game/" + nameof(BlockJamGameConfig))]
    public sealed class BlockJamGameConfig : ScriptableObject
    {
        public int[] MovesToWin = { 50, 30, 20, 10 };
        public int[] ObstacleCount = { 5, 10, 15, 25 };
        public int GridWidth = 8;
        public int GridHeight = 10;
    }
}