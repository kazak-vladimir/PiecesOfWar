using ForestGambit.Gameplay.Core.Board;
using System;
using UnityEngine;

namespace ForestGambit.Gameplay.Core.Entity
{
    /// <summary>
    /// Manages grid-based positioning and rotation for game units.
    /// Synchronizes with Unity Transform (1 grid unit = 1 world unit)
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
            position = WorldToGrid(transform.position);
            SyncTransform();
        }

        public void SyncTransform()
        {
            transform.position = GetWorldPosition();
        }

        // Conversion
        public Vector3 GetWorldPosition() =>
            new Vector3(position.X, 0, position.Y);

        public static GridCoordinates WorldToGrid(Vector3 worldPosition) =>
            new GridCoordinates(
                Mathf.RoundToInt(worldPosition.x),
                Mathf.RoundToInt(worldPosition.z)
            );

        // Editor
        private void OnValidate()
        {
            SyncTransform();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Vector3 worldPos = GetWorldPosition();
            Gizmos.DrawWireCube(worldPos, new Vector3(.9f, .1f, .9f));
        }
#endif
    }
}