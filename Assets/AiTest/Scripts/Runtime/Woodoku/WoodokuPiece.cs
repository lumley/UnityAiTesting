using System.Collections.Generic;
using DG.Tweening;
using Lumley.AiTest.GameShared;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lumley.AiTest.Woodoku
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class WoodokuPiece : MonoBehaviour,
        IPointerDownHandler,
        IPointerUpHandler,
        IDragHandler
    {
        [SerializeField] private Vector2Int[] _blockPositions = { };

        [SerializeField] private Block _blockPrefab = null!;

        [Header("Selection")]
        [SerializeField] private Collider2D _collider = null!;
        [SerializeField] private float _resetDuration = 0.2f;
        [SerializeField] private Ease _resetEase = Ease.OutSine;

        private GameObject? _ownPrefab;
        private PoolingManager? _poolingManager;
        private readonly List<Block> _blocks = new();
        private bool _isDragging;
        private bool _wasDragUnhandled;
        private Vector3 _draggingOffset;
        private Camera? _camera;
        private Vector3 _initialPosition;

        public IReadOnlyList<Block> Blocks => _blocks;

        public void Initialize(PoolingManager poolingManager, GameObject ownPrefab, Camera gameCamera)
        {
            _ownPrefab = ownPrefab;
            _poolingManager = poolingManager;
            _camera = gameCamera;
            
            _isDragging = false;
            _blocks.Clear();
            Vector2Int maxPosition = Vector2Int.zero;
            // Create visual representation
            foreach (var pos in _blockPositions)
            {
                var block = poolingManager.GetOrCreate(_blockPrefab, transform);
#if UNITY_EDITOR
                block.name = $"PieceBlock_{pos}";
#endif
                var blockSize = block.GetBounds().size;
                block.transform.localPosition = new Vector3(pos.x * blockSize.x, pos.y * blockSize.y, 0);
                block.Initialize(Color.white, poolingManager, _blockPrefab.gameObject);
                _blocks.Add(block);
                maxPosition = Vector2Int.Max(maxPosition, pos);
            }

            var newBounds = GetBounds();
            var defaultBlockSize = (Vector2)_blockPrefab.GetBounds().size;
            _collider.offset = maxPosition * defaultBlockSize * 0.5f;

            if (_collider is BoxCollider2D boxCollider)
            {
                boxCollider.size = newBounds.size;
            }
            else if (_collider is CircleCollider2D circleCollider)
            {
                circleCollider.radius = Mathf.Max(newBounds.size.x, newBounds.size.y);
            }
        }

        public Vector2Int[] GetBlockPositions() => _blockPositions;

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

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_camera == null)
            {
                return;
            }
            _isDragging = true;
            Vector3 worldPos = _camera.ScreenToWorldPoint(eventData.position);
            worldPos.z = transform.position.z;
            _draggingOffset = transform.position - worldPos;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_isDragging)
            {
                _wasDragUnhandled = true;
            }
            _isDragging = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isDragging && _camera != null)
            {
                Vector3 worldPos = _camera.ScreenToWorldPoint(eventData.position);
                worldPos.z = transform.position.z;
                transform.position = worldPos + _draggingOffset;
            }
        }

        public Bounds GetBounds()
        {
            var bounds = new Bounds(transform.position, Vector3.zero);
            foreach (var block in _blocks)
            {
                // Make it encapsulate the blocks top right corner
                bounds.Encapsulate(block.transform.position + block.GetBounds().size);
            }

            return bounds;
        }

        public void SetInitialPosition(Vector3 spawnAtPosition)
        {
            transform.localPosition = spawnAtPosition;
            _initialPosition = transform.position;
        }

        public bool HasBeenDraggedAndDropped()
        {
            if (_isDragging == false && _wasDragUnhandled)
            {
                _wasDragUnhandled = false;
                return true;
            }

            return false;
        }

        public void ResetPosition()
        {
            _collider.enabled = false;
            transform.DOMove(_initialPosition, _resetDuration)
                .SetEase(_resetEase)
                .OnComplete(() => _collider.enabled = true);
        }
    }
}