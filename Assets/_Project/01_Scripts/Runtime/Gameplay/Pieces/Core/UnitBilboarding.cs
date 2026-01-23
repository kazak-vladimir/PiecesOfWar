using System.Linq;
using UnityEngine;

namespace ForestGambit.Gameplay.Core.Entity
{
    public class UnitBillboard : MonoBehaviour
    {
        [System.Serializable]
        struct ClampedAngle
        {
            public bool clamped;
            public int min;
            public int max;
            ClampedAngle(int min, int max)
            {
                clamped = false;
                this.min = min;
                this.max = max;
            }
            public float Clamp(float input)
            {
                if (!clamped) return input;
                return Mathf.Clamp(input, min, max);
            }
        }

        [Header("Billboard Settings")]
        [SerializeField] private Vector2Int axis = new(1, 0);
        [SerializeField] private ClampedAngle ClampX, ClampY;

        [Header("Offset")]
        [SerializeField] private Transform rotationOffset;

        private Transform mainCameraTransform;
        private Transform unitTransform;

        private void OnValidate()
        {
            if (axis.x == 0 && axis.y == 0)
            {
                Debug.LogWarning("UnitBillboard axis cannot be (0,0). Resetting to (1,0).");
                axis = new Vector2Int(1, 0);
            }
            axis = new(
                axis.x < 0 ? 0 : axis.x > 1 ? 1 : axis.x,
                axis.y < 0 ? 0 : axis.y > 1 ? 1 : axis.y
                );
            ClampX.max = ClampX.max < ClampX.min ? ClampX.min : ClampX.max;
            ClampY.max = ClampY.max < ClampY.min ? ClampY.min : ClampY.max;
        }

        private void Awake()
        {
            if (Camera.main == null)
            {
                Debug.LogError("No MainCamera found for UnitBillboard.");
                enabled = false;
                return;
            }

            mainCameraTransform ??= Camera.main.transform;
            unitTransform ??= transform;
        }

        private void LateUpdate()
        {
            if (mainCameraTransform == null) return;

            Vector3 directionToCamera = unitTransform.position - mainCameraTransform.position;

            directionToCamera = new(directionToCamera.x * axis.y, directionToCamera.y * axis.x, directionToCamera.z);

            if (directionToCamera.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToCamera.normalized, Vector3.up);
                Vector3 eulerAngles = targetRotation.eulerAngles;

                // Convert to -180 to 180 range for proper clamping
                float angleX = eulerAngles.x > 180 ? eulerAngles.x - 360 : eulerAngles.x;
                float angleY = eulerAngles.y > 180 ? eulerAngles.y - 360 : eulerAngles.y;

                // Apply clamping
                angleX = ClampX.Clamp(angleX);
                angleY = ClampY.Clamp(angleY);

                // Apply the clamped billboard rotation
                Quaternion billboardRotation = Quaternion.Euler(angleX, angleY, eulerAngles.z);

                // Apply rotation offset if assigned
                if (rotationOffset != null)
                {
                    unitTransform.rotation = billboardRotation * rotationOffset.localRotation;
                }
                else
                {
                    unitTransform.rotation = billboardRotation;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            int segments = 36;

            // y axis circle (XZ plane)
            var axisLinesY = Enumerable.Range(0, segments)
                .Select(i => transform.position + new Vector3(
                    Mathf.Cos(Mathf.Deg2Rad * -ClampY.Clamp(i * 720f / segments - 360f)) * 0.5f,
                    0,
                    Mathf.Sin(Mathf.Deg2Rad * -ClampY.Clamp(i * 720f / segments - 360f)) * 0.5f))
                .ToArray();
            Gizmos.color = Color.green;
            Gizmos.DrawLineStrip(axisLinesY, false);

            // x axis circle (YZ plane)
            var axisLinesX = Enumerable.Range(0, segments)
                .Select(i => transform.position + new Vector3(
                    0,
                    Mathf.Cos(Mathf.Deg2Rad * ClampX.Clamp(i * 720f / segments - 360f)) * 0.5f,
                    Mathf.Sin(Mathf.Deg2Rad * ClampX.Clamp(i * 720f / segments - 360f)) * 0.5f))
                .ToArray();
            Gizmos.color = Color.red;
            Gizmos.DrawLineStrip(axisLinesX, false);
        }
    }
}