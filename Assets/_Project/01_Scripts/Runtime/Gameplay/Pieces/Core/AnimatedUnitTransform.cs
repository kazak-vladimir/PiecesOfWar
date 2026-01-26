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
        [SerializeField, Tooltip("Transform to animate")] private Transform animatedTransform;
        [SerializeField, Tooltip("Transform to follow")] private UnitTransform targetTransform;
        [SerializeField, Tooltip("Duration of animation from start to finish")] private float duration = 0.25f;
        [SerializeField, Tooltip("Easing curve for the animation")] private AnimationCurve easingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Rotation Settings")]
        [SerializeField, Tooltip("Enable dynamic rotation during movement")] private bool enableRotation = true;
        [SerializeField, Tooltip("Maximum tilt angle in degrees"), Range(0f, 45f)] private float maxTiltAngle = 15f;
        [SerializeField, Tooltip("Rotation behavior mode")] private RotationMode rotationMode = RotationMode.DirectionBased;
        [SerializeField, Tooltip("Rotation damping factor"), Range(1f, 10f)] private float rotationDamping = 5f;
        [SerializeField, Tooltip("Duration for returning to neutral rotation")] private float rotationReturnDuration = 0.15f;

        private Vector3 startPosition;
        private Vector3 targetPosition;
        private CancellationTokenSource cts;

        private float realDuration;

        /// <summary>
        /// Defines the rotation behavior during movement animation.
        /// </summary>
        private enum RotationMode
        {
            /// <summary>Tilt forward/backward along X axis</summary>
            ForwardTilt,
            /// <summary>Tilt left/right along Z axis</summary>
            LateralTilt,
            /// <summary>Dynamic tilt based on movement direction vector</summary>
            DirectionBased
        }

        public void OnEnable()
        {
            SetInstant(targetTransform.Position);
            targetTransform.OnPositionChanged += HandlePositionChanged;
        }

        public void OnDisable()
        {
            SetInstant(targetTransform.Position);
            targetTransform.OnPositionChanged -= HandlePositionChanged;
        }

        private async void HandlePositionChanged(GridCoordinates coordinates)
        {
            realDuration = duration * Vector3.Distance(animatedTransform.position, coordinates) / GridCoordinates.CellSize;
            await AnimateToAsync(coordinates);
        }

        /// <summary>
        /// Animates the unit transform to the target grid coordinates with optional rotation effects.
        /// </summary>
        /// <param name="newTarget">Target grid coordinates to move to</param>
        /// <param name="externalToken">Optional cancellation token for external cancellation</param>
        /// <returns>A UniTask that completes when the animation finishes or is cancelled</returns>
        public async UniTask AnimateToAsync(GridCoordinates newTarget, CancellationToken externalToken = default)
        {
            CancelCurrentAnimation();
            cts = new CancellationTokenSource();

            using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, externalToken))
            {
                try
                {
                    await ExecuteMovementAnimation(newTarget, linkedCts.Token);
                }
                catch (OperationCanceledException)
                {
                    HandleAnimationCancellation();
                }
            }
        }

        /// <summary>
        /// Executes the core movement and rotation animation logic.
        /// </summary>
        private async UniTask ExecuteMovementAnimation(GridCoordinates newTarget, CancellationToken token)
        {
            InitializeAnimationParameters(newTarget);
            Vector3 moveDirection = CalculateMoveDirection();

            await PerformMovementLoop(moveDirection, token);

            FinalizePosition();

            if (enableRotation)
            {
                await ReturnToNeutralRotationAsync(token);
            }
        }

        /// <summary>
        /// Initializes animation start and target parameters.
        /// </summary>
        private void InitializeAnimationParameters(GridCoordinates newTarget)
        {
            startPosition = animatedTransform.position;
            targetPosition = newTarget;
        }

        /// <summary>
        /// Calculates the normalized direction vector for movement.
        /// </summary>
        private Vector3 CalculateMoveDirection()
        {
            Vector3 direction = targetPosition - startPosition;
            return direction.sqrMagnitude > 0.001f ? direction.normalized : Vector3.forward;
        }

        /// <summary>
        /// Performs the main animation loop for position and rotation interpolation.
        /// </summary>
        private async UniTask PerformMovementLoop(Vector3 moveDirection, CancellationToken token)
        {
            float elapsed = 0f;

            while (elapsed < realDuration)
            {
                token.ThrowIfCancellationRequested();

                elapsed += Time.deltaTime;
                float normalizedTime = Mathf.Clamp01(elapsed / realDuration);
                float easedTime = easingCurve.Evaluate(normalizedTime);

                UpdatePosition(easedTime);

                if (enableRotation)
                {
                    UpdateRotation(normalizedTime, moveDirection);
                }

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }

        /// <summary>
        /// Updates the transform position based on interpolation.
        /// </summary>
        private void UpdatePosition(float easedTime)
        {
            animatedTransform.position = Vector3.LerpUnclamped(startPosition, targetPosition, easedTime);
        }

        /// <summary>
        /// Finalizes the position to ensure accuracy after animation completion.
        /// </summary>
        private void FinalizePosition()
        {
            animatedTransform.position = targetTransform.Position;
        }

        /// <summary>
        /// Updates the transform rotation based on movement progress and direction.
        /// </summary>
        /// <param name="normalizedTime">Normalized animation progress (0-1)</param>
        /// <param name="moveDirection">Normalized movement direction vector</param>
        private void UpdateRotation(float normalizedTime, Vector3 moveDirection)
        {
            float tiltIntensity = CalculateTiltIntensity(normalizedTime);
            Quaternion targetRotation = CalculateTargetRotation(tiltIntensity, moveDirection);

            ApplyRotationSmoothing(targetRotation);
        }

        /// <summary>
        /// Calculates tilt intensity using a sine wave that peaks at mid-movement.
        /// </summary>
        private float CalculateTiltIntensity(float normalizedTime)
        {
            return Mathf.Sin(normalizedTime * Mathf.PI) * maxTiltAngle;
        }

        /// <summary>
        /// Calculates the target rotation quaternion based on the selected rotation mode.
        /// </summary>
        private Quaternion CalculateTargetRotation(float tiltIntensity, Vector3 moveDirection)
        {
            return rotationMode switch
            {
                RotationMode.ForwardTilt => Quaternion.Euler(tiltIntensity, 0f, 0f),
                RotationMode.LateralTilt => Quaternion.Euler(0f, 0f, -tiltIntensity * Mathf.Sign(moveDirection.x)),
                RotationMode.DirectionBased => CalculateDirectionBasedRotation(tiltIntensity, moveDirection),
                _ => Quaternion.identity
            };
        }

        /// <summary>
        /// Calculates rotation based on the actual movement direction vector.
        /// </summary>
        private Quaternion CalculateDirectionBasedRotation(float tiltIntensity, Vector3 moveDirection)
        {
            float xTilt = -moveDirection.z * tiltIntensity;
            float zTilt = moveDirection.x * tiltIntensity;
            return Quaternion.Euler(xTilt, 0f, zTilt);
        }

        /// <summary>
        /// Applies smooth interpolation to the target rotation.
        /// </summary>
        private void ApplyRotationSmoothing(Quaternion targetRotation)
        {
            float dampingFactor = Time.deltaTime * rotationDamping * 10f;
            animatedTransform.localRotation = Quaternion.Slerp(
                animatedTransform.localRotation,
                targetRotation,
                dampingFactor
            );
        }

        /// <summary>
        /// Smoothly returns the transform to neutral rotation after movement completes.
        /// </summary>
        private async UniTask ReturnToNeutralRotationAsync(CancellationToken token)
        {
            Quaternion startRotation = animatedTransform.localRotation;
            float elapsed = 0f;

            while (elapsed < rotationReturnDuration)
            {
                token.ThrowIfCancellationRequested();

                elapsed += Time.deltaTime;
                float normalizedTime = Mathf.Clamp01(elapsed / rotationReturnDuration);

                animatedTransform.localRotation = Quaternion.Slerp(
                    startRotation,
                    Quaternion.identity,
                    normalizedTime
                );

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            animatedTransform.localRotation = Quaternion.identity;
        }

        /// <summary>
        /// Cancels any currently running animation.
        /// </summary>
        private void CancelCurrentAnimation()
        {
            cts?.Cancel();
        }

        /// <summary>
        /// Handles cleanup when an animation is cancelled.
        /// </summary>
        private void HandleAnimationCancellation()
        {
            SetInstant(targetPosition);

            if (enableRotation)
            {
                animatedTransform.localRotation = Quaternion.identity;
            }
        }

        /// <summary>
        /// Immediately sets the transform position without animation and resets rotation.
        /// </summary>
        /// <param name="position">Target position to set</param>
        public void SetInstant(Vector3 position)
        {
            CancelCurrentAnimation();
            animatedTransform.position = position;

            if (enableRotation)
            {
                animatedTransform.localRotation = Quaternion.identity;
            }
        }

        private void OnDestroy()
        {
            cts?.Cancel();
            cts?.Dispose();
        }
    }
}