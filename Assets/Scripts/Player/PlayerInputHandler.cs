using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Player
{
    /// <summary>
    /// Handles player movement input and applies it to the Rigidbody.
    /// </summary>
    public class PlayerInputHandler : MonoBehaviour
    {
        
        InputAction lookAction;
        InputAction moveAction;
        InputAction jumpAction;
        InputAction interactAction;
        InputAction crouchAction;

        PlayerController playerController;

        private void Awake()
        {
            lookAction = InputSystem.actions.FindAction("Look");
            moveAction = InputSystem.actions.FindAction("Move");
            jumpAction = InputSystem.actions.FindAction("Jump");
            interactAction = InputSystem.actions.FindAction("Interact");
            crouchAction = InputSystem.actions.FindAction("Crouch");

            playerController = FindFirstObjectByType<PlayerController>();
            if (playerController == null)
            {
                Debug.LogError("PlayerController not found in the scene.");
            }
        }

        void OnEnable()
        {
            jumpAction.started += OnJumpStart;
            interactAction.started += OnInteractStart;
        }
        void OnDisable()
        {
            jumpAction.started -= OnJumpStart;
            interactAction.started -= OnInteractStart;
        }

        void Update()
        {
            Vector2 lookValue = lookAction.ReadValue<Vector2>();
            if (playerController != null)
            {
                playerController.Look(lookValue);
            }
        }
        void FixedUpdate()
        {
            Vector2 moveValue = moveAction.ReadValue<Vector2>();
            if (playerController != null)
            {
                playerController.Move(moveValue);
            }

            bool jumping = jumpAction.ReadValue<float>() > 0.1f;
            bool crouching = crouchAction.ReadValue<float>() > 0.1f;
            if (jumping && !crouching)
            {
                playerController.TryFlyIfDebug(true);
            }
            else if (crouching && !jumping)
            {
                playerController.TryFlyIfDebug(false);
            }
            else
            {
                playerController.TryStopFlyIfDebug();
            }
        }
        void OnJumpStart(InputAction.CallbackContext context)
        {
            if (!playerController.GetJumpBehavior().TryStartJump())
            {
                Debug.Log("Jump failed: player is not grounded.");
            }
        }

        void OnFlyUp(InputAction.CallbackContext context)
        {
            playerController.TryFlyIfDebug(true);
        }

        void OnFlyDown(InputAction.CallbackContext context)
        {
            playerController.TryFlyIfDebug(false);
        }

        void OnInteractStart(InputAction.CallbackContext context)
        {
            playerController.ToggleDebugMode();
        }
    }
}