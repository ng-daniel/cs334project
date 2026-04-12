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

        PlayerController playerController;

        private void Awake()
        {
            lookAction = InputSystem.actions.FindAction("Look");
            moveAction = InputSystem.actions.FindAction("Move");
            jumpAction = InputSystem.actions.FindAction("Jump");
            playerController = FindFirstObjectByType<PlayerController>();
            if (playerController == null)
            {
                Debug.LogError("PlayerController not found in the scene.");
            }
        }

        void OnEnable()
        {
            jumpAction.started += OnJumpStart;
        }
        void OnDisable()
        {
            jumpAction.started -= OnJumpStart;
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
        }
        public void OnJumpStart(InputAction.CallbackContext context)
        {
            if (!playerController.GetJumpBehavior().TryStartJump())
            {
                Debug.Log("Jump failed: player is not grounded.");
            }
        }
    }
}