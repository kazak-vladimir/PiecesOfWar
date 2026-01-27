using ForestGambit.Gameplay.Core.Board;
using System;
using UnityEngine;

namespace ForestGambit.Gameplay.Core.Entity
{
    /// <summary>
    /// Manages grid-based positioning for game units.
    /// Synchronizes with Unity Transform
    /// </summary>
    public class UnitTransform : MonoBehaviour
    {
        [SerializeField] private GridCoordinates position;

        public event Action<GridCoordinates> OnPositionChanged;

        public GridCoordinates Position => position;

        // Movement
        public void MoveTo(GridCoordinates destination)
        {
            if (position == destination) return;
            position = destination;
            SyncTransform();
            OnPositionChanged?.Invoke(position);
        }

        public void MoveBy(GridCoordinates translation) => MoveTo(position + translation);

        // Sync
        public void SnapToGrid()
        {
            position = transform.position;
            SyncTransform();
        }

        public void SyncTransform()
        {
            transform.position = (Vector3)position;
        }

        // Editor
        private void OnValidate()
        {
            SyncTransform();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Vector3 worldPos = position;
            Gizmos.DrawWireCube(worldPos, new Vector3(.9f * GridCoordinates.CellSize, .1f, .9f * GridCoordinates.CellSize));
        }
#endif
    }
}