using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Player
{
    /// <summary>
    /// Handles movement input and applies it to a Rigidbody.
    /// </summary>
    public class LookBehavior : MonoBehaviour
    {
        Vector2 rotation;
        [SerializeField] float lookSensitivity = 0.25f;

        public void Start()
        {
            rotation = Vector2.zero;           
        }

        public Quaternion LookToQuaternion(Vector2 lookInput)
        {
            Cursor.lockState = CursorLockMode.Locked;

            rotation.x += lookInput.x * lookSensitivity;
            rotation.y += lookInput.y * lookSensitivity;
            rotation.y = Mathf.Clamp(rotation.y, -90f, 90f);
            Quaternion xRotation = Quaternion.Euler(0f, rotation.x, 0f);
            Quaternion yRotation = Quaternion.Euler(-rotation.y, 0f, 0f);
            Quaternion result = xRotation * yRotation;
            return result;
        }
    }
}