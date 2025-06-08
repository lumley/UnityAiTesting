using System.Collections.Generic;
using Lumley.AiTest.GameShared;
using UnityEngine;

namespace Lumley.AiTest.ColorSort
{
    public class ColorSortController : BaseGameController
    {
        [Header("Color Sort Specific")] public Transform tubesParent;
        public BlockPool blockPool;

        private List<ColorTube> tubes = new List<ColorTube>();
        private ColorTube selectedTube;
        private int tubeCount;
        private int colorCount;
        private int tubeCapacity;

        protected override void InitializeGame()
        {
            var config = GameManager.Instance.gameConfig.colorSortConfig;
            tubeCount = config.tubeCount[(int)GameManager.Instance.CurrentDifficulty];
            colorCount = config.colorCount[(int)GameManager.Instance.CurrentDifficulty];
            tubeCapacity = config.tubeCapacity;

            GeneratePuzzle();
        }

        protected override void UpdateGameplay()
        {
            HandleInput();
            CheckWinCondition();
        }

        private void HandleInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                HandleTubeSelection(mousePos);
            }
        }

        private void HandleTubeSelection(Vector2 worldPos)
        {
            ColorTube clickedTube = GetTubeAtPosition(worldPos);

            if (clickedTube == null) return;

            if (selectedTube == null)
            {
                // First selection - select source tube
                if (!clickedTube.IsEmpty())
                {
                    selectedTube = clickedTube;
                    clickedTube.SetSelected(true);
                }
            }
            else
            {
                // Second selection - try to pour
                if (clickedTube == selectedTube)
                {
                    // Deselect if clicking same tube
                    selectedTube.SetSelected(false);
                    selectedTube = null;
                }
                else
                {
                    // Try to pour from selected to clicked tube
                    if (TryPourBetweenTubes(selectedTube, clickedTube))
                    {
                        selectedTube.SetSelected(false);
                        selectedTube = null;
                    }
                }
            }
        }

        private ColorTube GetTubeAtPosition(Vector2 worldPos)
        {
            foreach (var tube in tubes)
            {
                if (tube.ContainsPoint(worldPos))
                    return tube;
            }

            return null;
        }

        private bool TryPourBetweenTubes(ColorTube source, ColorTube target)
        {
            if (source.IsEmpty() || target.IsFull()) return false;

            Color topColor = source.GetTopColor();

            if (!target.IsEmpty() && target.GetTopColor() != topColor) return false;

            // Count how many blocks of the same color can be moved
            int blocksToMove = source.GetConsecutiveTopBlocks(topColor);
            int spaceInTarget = target.GetEmptySpace();

            blocksToMove = Mathf.Min(blocksToMove, spaceInTarget);

            if (blocksToMove > 0)
            {
                for (int i = 0; i < blocksToMove; i++)
                {
                    Block block = source.RemoveTopBlock();
                    target.AddBlock(block);
                }

                return true;
            }

            return false;
        }

        private void GeneratePuzzle()
        {
            // Create tubes
            for (int i = 0; i < tubeCount; i++)
            {
                GameObject tubeObj = new GameObject($"Tube_{i}");
                tubeObj.transform.SetParent(tubesParent);
                tubeObj.transform.position = new Vector3(i * 2f, 0, 0);

                ColorTube tube = tubeObj.AddComponent<ColorTube>();
                tube.Initialize(tubeCapacity);
                tubes.Add(tube);
            }

            // Generate colors
            Color[] colors = GenerateColors(colorCount);

            // Fill tubes with mixed colors (leave some empty for solving)
            List<Block> allBlocks = new List<Block>();

            for (int colorIndex = 0; colorIndex < colorCount; colorIndex++)
            {
                for (int j = 0; j < tubeCapacity; j++)
                {
                    GameObject blockObj = new GameObject($"ColorBlock_{colorIndex}_{j}");
                    Block block = blockObj.AddComponent<Block>();
                    block.Initialize(Block.BlockType.Standard, colors[colorIndex]);
                    allBlocks.Add(block);
                }
            }

            // Shuffle blocks
            for (int i = 0; i < allBlocks.Count; i++)
            {
                Block temp = allBlocks[i];
                int randomIndex = Random.Range(i, allBlocks.Count);
                allBlocks[i] = allBlocks[randomIndex];
                allBlocks[randomIndex] = temp;
            }

            // Distribute blocks to tubes (leaving some tubes empty)
            int tubesForBlocks = tubeCount - 2; // Leave 2 tubes empty for maneuvering
            int blocksPerTube = allBlocks.Count / tubesForBlocks;

            for (int i = 0; i < tubesForBlocks; i++)
            {
                for (int j = 0; j < blocksPerTube && i * blocksPerTube + j < allBlocks.Count; j++)
                {
                    tubes[i].AddBlock(allBlocks[i * blocksPerTube + j]);
                }
            }
        }

        private Color[] GenerateColors(int count)
        {
            Color[] colors = new Color[count];

            for (int i = 0; i < count; i++)
            {
                float hue = (float)i / count;
                colors[i] = Color.HSVToRGB(hue, 0.8f, 0.9f);
            }

            return colors;
        }

        private void CheckWinCondition()
        {
            bool allSorted = true;

            foreach (var tube in tubes)
            {
                if (!tube.IsEmpty() && !tube.IsSingleColor())
                {
                    allSorted = false;
                    break;
                }
            }

            if (allSorted)
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