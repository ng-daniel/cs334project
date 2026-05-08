using Assets.Scripts;
using UnityEngine;

namespace Assets.Scripts.Player
{
    /// <summary>
    /// Main controller for the player character, coordinating movement, looking, and jumping behaviors.
    /// 
    /// Base Controls:
    /// - Move: WASD or arrow keys
    /// - Look: Mouse movement
    /// - Jump: Spacebar
    /// 
    /// Toggle Debug Mode:
    /// - Interact: E key (toggles debug mode on/off)
    /// 
    /// Debug Mode Controls:
    /// - Fly Up: Spacebar (while in debug mode, holds the player in the air)
    /// - Fly Down: C (while in debug mode, moves the player downwards)
    /// - Horizontal movement and looking remain the same in debug mode.
    /// </summary>
    [RequireComponent(typeof(LookBehavior))]
    [RequireComponent(typeof(MovementBehavior))]
    [RequireComponent(typeof(JumpBehavior))]
    public class PlayerController : MonoBehaviour
    {
        
        Rigidbody rb;
        Collider col;
        
        LookBehavior lookBehavior;
        MovementBehavior movementBehavior;
        JumpBehavior jumpBehavior;
        FPSCameraBehavior fpsCameraBehavior;
        bool debugMode = false;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();
            
            lookBehavior = GetComponent<LookBehavior>();
            movementBehavior = GetComponent<MovementBehavior>();
            jumpBehavior = GetComponent<JumpBehavior>();
            fpsCameraBehavior = FindFirstObjectByType<FPSCameraBehavior>();
        }

        void Update()
        {
            if (fpsCameraBehavior != null)
            {
                fpsCameraBehavior.MatchPosition(transform);
            }           
        }

        public void Move(Vector2 moveInput)
        {
            if (debugMode)
            {
                movementBehavior.MoveDebug(moveInput);
            }
            else
            {
                movementBehavior.MoveRegular(moveInput);
            }
        }

        public void Look(Vector2 lookInput)
        {
            Quaternion result = lookBehavior.LookToQuaternion(lookInput);
            Quaternion horizontalRotation = Quaternion.Euler(0, result.eulerAngles.y, 0);
            transform.rotation = horizontalRotation;

            if (fpsCameraBehavior != null)
            {
                fpsCameraBehavior.SetLookDirection(result);
            }
        }

        /// <summary>
        /// Attempts to start a fly action if the player is in debug mode.
        /// </summary>
        /// <param name="isUpwards">Is the fly direction upwards?</param>
        public void TryFlyIfDebug(bool isUpwards)
        {
            if (debugMode)
            {
                jumpBehavior.Fly(isUpwards);
            }
        }
        public void TryStopFlyIfDebug()
        {
            if (debugMode)
            {
                jumpBehavior.StopFly();
            }
        }

        public LookBehavior GetLookBehavior()
        {
            return lookBehavior;
        }

        public MovementBehavior GetMovementBehavior()
        {
            return movementBehavior;
        }

        public JumpBehavior GetJumpBehavior()
        {
            return jumpBehavior;
        }

        public void ToggleDebugMode()
        {
            if (!debugMode)
            {
                Debug.Log("Debug mode enabled");
                rb.useGravity = false;
                col.enabled = false;
                debugMode = true;
                
            }
            else
            {
                Debug.Log("Debug mode disabled");
                rb.useGravity = true;
                col.enabled = true;
                debugMode = false;
             }
        }        
    }
}
