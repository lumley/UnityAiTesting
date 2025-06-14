using System.Collections.Generic;
using Lumley.AiTest.GameShared;
using UnityEngine;

namespace Lumley.AiTest.Woodoku
{
    public sealed class WoodokuPiece : MonoBehaviour
    {
        [SerializeField]
        private Vector2Int[] _blockPositions = { };
        
        [SerializeField]
        private Block _blockPrefab = null!;
        
        private Color _pieceColor;
        private bool _isSelected;

        public bool IsSelected => _isSelected;
        
        private GameObject? _ownPrefab;
        private PoolingManager? _poolingManager;
        private List<Block> _blocks = new();
        
        public IReadOnlyList<Block> Blocks => _blocks;

        public void Initialize(PoolingManager poolingManager, GameObject ownPrefab)
        {
            _ownPrefab = ownPrefab;
            _poolingManager = poolingManager;
            _pieceColor = Random.ColorHSV();

            // Create visual representation
            foreach (var pos in _blockPositions)
            {
                var block = poolingManager.GetOrCreate(_blockPrefab, transform);
#if UNITY_EDITOR
                block.name = $"PieceBlock_{pos}";
#endif
                block.transform.localPosition = new Vector3(pos.x, pos.y, 0);
                block.Initialize(_pieceColor, poolingManager, _blockPrefab.gameObject);
                _blocks.Add(block);
            }
        }

        public Vector2Int[] GetBlockPositions() => _blockPositions;
        public Color GetColor() => _pieceColor;

        public int GetScore()
        {
            return _blockPositions.Length * 10;
        }

        public void Recycle()
        {
            if (_poolingManager != null && _ownPrefab != null)
            {
                _poolingManager.Recycle(_ownPrefab, gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnMouseDown()
        {
            _isSelected = true;
        }

        private void OnMouseUp()
        {
            _isSelected = false;
        }
    }
}