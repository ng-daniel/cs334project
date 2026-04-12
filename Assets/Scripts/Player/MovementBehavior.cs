using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Player
{
    /// <summary>
    /// Handles movement input and applies it to a Rigidbody.
    /// </summary>
    public class MovementBehavior : MonoBehaviour
    {
        Rigidbody rb;
        Collider col;
        bool debugMode = false;
        [SerializeField] MovementStats moveStats;
        [SerializeField] MovementStats debugMoveStats;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();
        }
        public void MoveRegular(Vector2 rawInput)
        {
            Move(rawInput, moveStats);
        }
        public void MoveDebug(Vector2 rawInput)
        {
            Move(rawInput, debugMoveStats);
        }

        /// <summary>
        /// Moves the player based on input values with acceleration and speed limits.
        /// </summary>
        /// <param name="rawInput">The input vector representing the desired movement direction.</param>
        /// <param name="moveStats">The movement stats to use for this movement.</param>
        void Move(Vector2 rawInput, MovementStats moveStats)
        {   
            const float inputThreshold = 0.01f;
            float dt = Time.fixedDeltaTime;

            bool hasInput = Mathf.Abs(rawInput.x) > inputThreshold || Mathf.Abs(rawInput.y) > inputThreshold;
            Vector2 inputDir = hasInput ? rawInput.normalized : Vector2.zero;

            // Convert input direction to world space relative to the player's horizontal facing
            Vector3 forward = transform.forward;
            forward.y = 0f;
            forward.Normalize();
            Vector3 right = transform.right;
            right.y = 0f;
            right.Normalize();
            Vector3 worldDir = right * inputDir.x + forward * inputDir.y;
            Vector2 worldInputDir = hasInput ? new Vector2(worldDir.x, worldDir.z) : Vector2.zero;

            Vector2 currentHorizontalVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z);
            Vector2 targetHorizontalVelocity = worldInputDir * moveStats.WalkSpeed;

            float acceleration = moveStats.WalkSpeedAccel;
            float deceleration = moveStats.WalkSpeedAccel * 1.35f;
            float directionChangeBoost = 2.0f;

            if (!hasInput)
            {
                // Decelerate to a stop when there is no input
                currentHorizontalVelocity = Vector2.MoveTowards(
                    currentHorizontalVelocity,
                    Vector2.zero,
                    deceleration * dt
                );
            }
            else
            {
                // Apply a boost to acceleration when changing direction sharply
                float turnMultiplier = 1f;
                if (Mathf.Abs(currentHorizontalVelocity.x) > inputThreshold ||
                    Mathf.Abs(currentHorizontalVelocity.y) > inputThreshold)
                {
                    Vector2 currentDir = currentHorizontalVelocity.normalized;
                    float alignment = Vector2.Dot(currentDir, inputDir);
                    if (alignment < 0f)
                    {
                        turnMultiplier = directionChangeBoost;
                    }
                }

                // Move towards the target velocity with the calculated acceleration
                currentHorizontalVelocity = Vector2.MoveTowards(
                    currentHorizontalVelocity,
                    targetHorizontalVelocity,
                    acceleration * turnMultiplier * dt
                );
            }

            rb.linearVelocity = new Vector3(currentHorizontalVelocity.x, rb.linearVelocity.y, currentHorizontalVelocity.y);
        }
    }
}