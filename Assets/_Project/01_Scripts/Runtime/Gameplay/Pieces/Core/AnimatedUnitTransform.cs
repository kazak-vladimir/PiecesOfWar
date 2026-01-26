using Cysharp.Threading.Tasks;
using ForestGambit.Gameplay.Core.Board;
using System;
using System.Threading;
using UnityEngine;

namespace ForestGambit.Gameplay.Core.Entity
{
    public class AnimatedUnitTransform : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private Transform animatedTransform;
        [SerializeField] private UnitTransform targetTransform;
        [SerializeField] private float duration = 0.25f;
        [SerializeField] private AnimationCurve easingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Rotation Settings")]
        [SerializeField] private bool enableRotation = true;
        [SerializeField, Range(0f, 45f)] private float maxTiltAngle = 15f;
        [SerializeField, Range(1f, 10f)] private float rotationSmoothing = 5f;

        private CancellationTokenSource animationCts;
        private bool isAnimating;

        private void OnValidate()
        {
            if (animatedTransform == null)
                animatedTransform = transform;
        }

        private void OnEnable()
        {
            if (targetTransform == null)
            {
                Debug.LogError("UnitTransform not assigned on AnimatedUnitTransform", this);
                enabled = false;
                return;
            }

            SetInstant(targetTransform.Position);
            targetTransform.OnPositionChanged += HandlePositionChanged;
        }

        private void OnDisable()
        {
            if (targetTransform != null)
                targetTransform.OnPositionChanged -= HandlePositionChanged;

            CancelAnimation();
        }

        private async void HandlePositionChanged(GridCoordinates coordinates)
        {
            if (isAnimating)
                CancelAnimation();

            await AnimateToAsync(coordinates);
        }

        private async UniTask AnimateToAsync(GridCoordinates target)
        {
            animationCts?.Cancel();
            animationCts = new CancellationTokenSource();
            isAnimating = true;

            try
            {
                Vector3 startPos = animatedTransform.position;
                Vector3 targetPos = target;
                Vector3 direction = (targetPos - startPos).normalized;

                float distance = Vector3.Distance(startPos, targetPos);
                float animDuration = duration * (distance / GridCoordinates.CellSize);

                float elapsed = 0f;

                while (elapsed < animDuration)
                {
                    animationCts.Token.ThrowIfCancellationRequested();

                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / animDuration);
                    float easedT = easingCurve.Evaluate(t);

                    animatedTransform.position = Vector3.Lerp(startPos, targetPos, easedT);

                    if (enableRotation)
                    {
                        float tiltAmount = Mathf.Sin(t * Mathf.PI) * maxTiltAngle;
                        float xTilt = -direction.z * tiltAmount;
                        float zTilt = direction.x * tiltAmount;
                        Quaternion targetRot = Quaternion.Euler(xTilt, 0f, zTilt);

                        animatedTransform.localRotation = Quaternion.Slerp(
                            animatedTransform.localRotation,
                            targetRot,
                            Time.deltaTime * rotationSmoothing
                        );
                    }

                    await UniTask.Yield(PlayerLoopTiming.Update, animationCts.Token);
                }

                animatedTransform.position = targetPos;

                if (enableRotation)
                    animatedTransform.localRotation = Quaternion.identity;
            }
            catch (OperationCanceledException)
            {
                animatedTransform.position = target;
                if (enableRotation)
                    animatedTransform.localRotation = Quaternion.identity;
            }
            finally
            {
                isAnimating = false;
            }
        }

        private void CancelAnimation()
        {
            animationCts?.Cancel();
            animationCts?.Dispose();
            animationCts = null;
        }

        public void SetInstant(Vector3 position)
        {
            CancelAnimation();
            animatedTransform.position = position;
            if (enableRotation)
                animatedTransform.localRotation = Quaternion.identity;
        }

        private void OnDestroy()
        {
            CancelAnimation();
        }
    }
}