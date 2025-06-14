using UnityEngine;

namespace Lumley.AiTest.Woodoku
{
    [CreateAssetMenu(fileName = nameof(WoodokuGameConfig), menuName = "Game/" + nameof(WoodokuGameConfig))]
    public sealed class WoodokuGameConfig : ScriptableObject
    {
        public int[] TargetScores = { 500, 1000, 2000, 5000 };
        public int[] PiecesCount = { 20, 15, 10, 5 }; // pieces to place
        public int GridSize = 9;
    }
}