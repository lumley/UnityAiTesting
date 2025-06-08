using System.Collections.Generic;
using System.Linq;
using Lumley.AiTest.GameShared;
using UnityEngine;

namespace Lumley.AiTest.ColorSort
{
    public class ColorTube : MonoBehaviour
    {
        private List<Block> blocks = new List<Block>();
        private int capacity;
        private bool isSelected = false;
        private LineRenderer outline;

        public void Initialize(int tubeCapacity)
        {
            capacity = tubeCapacity;

            // Create visual outline
            outline = gameObject.AddComponent<LineRenderer>();
            outline.material = new Material(Shader.Find("Sprites/Default"));
            outline.startColor = Color.white;
            outline.endColor = Color.white;
            outline.startWidth = 0.1f;
            outline.endWidth = 0.1f;
            outline.useWorldSpace = false;

            DrawTubeOutline();
        }

        private void DrawTubeOutline()
        {
            Vector3[] points = new Vector3[5];
            float width = 1f;
            float height = capacity + 0.5f;

            points[0] = new Vector3(-width / 2, 0, 0);
            points[1] = new Vector3(-width / 2, height, 0);
            points[2] = new Vector3(width / 2, height, 0);
            points[3] = new Vector3(width / 2, 0, 0);
            points[4] = points[0]; // Close the loop

            outline.positionCount = 5;
            outline.SetPositions(points);
        }

        public void AddBlock(Block block)
        {
            if (blocks.Count < capacity)
            {
                blocks.Add(block);
                block.transform.SetParent(transform);
                block.transform.localPosition = new Vector3(0, blocks.Count - 1, 0);
            }
        }

        public Block RemoveTopBlock()
        {
            if (blocks.Count > 0)
            {
                Block topBlock = blocks[blocks.Count - 1];
                blocks.RemoveAt(blocks.Count - 1);
                return topBlock;
            }

            return null;
        }

        public Color GetTopColor()
        {
            if (blocks.Count > 0)
                return blocks[blocks.Count - 1].blockColor;
            return Color.clear;
        }

        public int GetConsecutiveTopBlocks(Color color)
        {
            int count = 0;
            for (int i = blocks.Count - 1; i >= 0; i--)
            {
                if (blocks[i].blockColor == color)
                    count++;
                else
                    break;
            }

            return count;
        }

        public int GetEmptySpace()
        {
            return capacity - blocks.Count;
        }

        public bool IsEmpty() => blocks.Count == 0;
        public bool IsFull() => blocks.Count >= capacity;

        public bool IsSingleColor()
        {
            if (blocks.Count == 0) return true;

            Color firstColor = blocks[0].blockColor;
            return blocks.All(block => block.blockColor == firstColor);
        }

        public bool ContainsPoint(Vector2 worldPoint)
        {
            Vector2 localPoint = transform.InverseTransformPoint(worldPoint);
            return localPoint.x >= -0.5f && localPoint.x <= 0.5f &&
                   localPoint.y >= 0f && localPoint.y <= capacity + 0.5f;
        }

        public void SetSelected(bool selected)
        {
            isSelected = selected;
            outline.startColor = selected ? Color.yellow : Color.white;
            outline.endColor = selected ? Color.yellow : Color.white;
        }
    }
}