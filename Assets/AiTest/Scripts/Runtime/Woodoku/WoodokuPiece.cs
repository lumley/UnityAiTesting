using Lumley.AiTest.GameShared;
using UnityEngine;

namespace Lumley.AiTest.Woodoku
{
    public class WoodokuPiece : MonoBehaviour
    {
        private Vector2Int[] blockPositions;
        private Color pieceColor;
        private bool isSelected = false;

        public bool IsSelected => isSelected;

        public void Initialize(Vector2Int[] positions)
        {
            blockPositions = positions;
            pieceColor = Random.ColorHSV();

            // Create visual representation
            foreach (var pos in blockPositions)
            {
                GameObject blockObj = new GameObject($"PieceBlock_{pos}");
                blockObj.transform.SetParent(transform);
                blockObj.transform.localPosition = new Vector3(pos.x, pos.y, 0);

                Block block = blockObj.AddComponent<Block>();
                block.Initialize(pieceColor);
            }
        }

        public Vector2Int[] GetBlockPositions() => blockPositions;
        public Color GetColor() => pieceColor;

        public int GetScore()
        {
            return blockPositions.Length * 10;
        }

        private void OnMouseDown()
        {
            isSelected = true;
        }

        private void OnMouseUp()
        {
            isSelected = false;
        }
    }
}