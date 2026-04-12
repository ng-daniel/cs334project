using Assets.Scripts;
using UnityEngine;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(LookBehavior))]
    [RequireComponent(typeof(MovementBehavior))]
    [RequireComponent(typeof(JumpBehavior))]
    public class PlayerController : MonoBehaviour
    {
        
        LookBehavior lookBehavior;
        MovementBehavior movementBehavior;
        JumpBehavior jumpBehavior;
        FPSCameraBehavior fpsCameraBehavior;

        void Awake()
        {
            lookBehavior = GetComponent<LookBehavior>();
            movementBehavior = GetComponent<MovementBehavior>();
            jumpBehavior = GetComponent<JumpBehavior>();
            fpsCameraBehavior = FindFirstObjectByType<FPSCameraBehavior>();
        }

        public void Move(Vector2 moveInput)
        {
            movementBehavior.Move(moveInput);
            if (fpsCameraBehavior != null)
            {
                fpsCameraBehavior.MatchPosition(transform);
            }
        }

        public void Look(Vector2 lookInput)
        {
            Quaternion result = lookBehavior.LookToQuaternion(lookInput);
            
            // ensure the player's horizontal rotation matches the look direction while keeping the player upright
            Vector3 flattenedForward = Vector3.ProjectOnPlane(result * Vector3.forward, Vector3.up);
            if (flattenedForward.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(flattenedForward, Vector3.up);
            }

            if (fpsCameraBehavior != null)
            {
                fpsCameraBehavior.SetLookDirection(result);
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
    }
}
