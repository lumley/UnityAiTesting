using UnityEngine;

namespace Lumley.AiTest.ColorSort
{
    [CreateAssetMenu(fileName = nameof(ColorSortGameConfig), menuName = "Game/" + nameof(ColorSortGameConfig))]
    public sealed class ColorSortGameConfig : ScriptableObject
    {
        public int[] TubeCount = { 6, 8, 10, 12 };
        public int[] ColorCount = { 4, 6, 8, 10 };
        public int TubeCapacity = 4;
    }
}