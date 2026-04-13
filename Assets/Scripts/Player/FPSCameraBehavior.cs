using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Player
{
    /// <summary>
    /// Handles movement input and applies it to a Rigidbody.
    /// </summary>
    public class FPSCameraBehavior : MonoBehaviour
    {
        Camera cam;

        public void Awake()
        {
            cam = GetComponent<Camera>();
            if (cam == null)
            {
                Debug.LogError("Camera component not found on FPSCameraBehavior object.");
            }
        }            


        public void MatchPosition(Transform target)
        {
            cam.transform.position = target.position;
        }
        public void SetLookDirection(Quaternion rotation)
        {
            cam.transform.rotation = rotation;
        }
        public void SetLookDirectionSmooth(Quaternion rotation, float smoothTime = 0.1f)
        {
            cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, rotation, smoothTime);
        }

    }
}