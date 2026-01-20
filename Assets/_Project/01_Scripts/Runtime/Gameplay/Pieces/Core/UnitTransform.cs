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
        [SerializeField, Range(0, 360)] private float rotation;

        public event Action<GridCoordinates> OnPositionChanged;
        public event Action<float> OnRotationChanged;

        public GridCoordinates Position => position;
        public float Rotation => rotation;

        // Movement
        public void MoveTo(GridCoordinates destination)
        {
            if (position == destination) return;
            position = destination;
            SyncTransform();
            OnPositionChanged?.Invoke(position);
        }

        public void MoveBy(GridCoordinates translation) => MoveTo(position + translation);

        // Rotation
        public void RotateTo(float newRotation)
        {
            newRotation = NormalizeRotation(newRotation);
            if (Mathf.Approximately(rotation, newRotation)) return;
            rotation = newRotation;
            SyncTransform();
            OnRotationChanged?.Invoke(rotation);
        }

        public void RotateBy(float delta) => RotateTo(rotation + delta);

        // Sync
        public void SnapToGrid()
        {
            position = WorldToGrid(transform.position);
            rotation = NormalizeRotation(transform.eulerAngles.y);
            SyncTransform();
        }

        public void SyncTransform()
        {
            transform.position = GetWorldPosition();
            transform.rotation = Quaternion.Euler(0f, rotation, 0f);
        }

        // Conversion
        public Vector3 GetWorldPosition() =>
            new Vector3(position.X, 0, position.Y);

        public static GridCoordinates WorldToGrid(Vector3 worldPosition) =>
            new GridCoordinates(
                Mathf.RoundToInt(worldPosition.x),
                Mathf.RoundToInt(worldPosition.z)
            );

        // Utility
        private float NormalizeRotation(float angle)
        {
            angle %= 360f;
            return angle < 0f ? angle + 360f : angle;
        }

        private void OnValidate()
        {
            rotation = NormalizeRotation(rotation);
            SyncTransform();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Vector3 worldPos = GetWorldPosition();
            Gizmos.DrawWireCube(worldPos, Vector3.one * 0.9f);

            Gizmos.color = Color.blue;
            Vector3 forward = Quaternion.Euler(0f, rotation, 0f) * Vector3.forward;
            Gizmos.DrawLine(worldPos, worldPos + forward * 0.4f);
        }
#endif
    }
}