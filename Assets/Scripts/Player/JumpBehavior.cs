using UnityEngine;

namespace Assets.Scripts.Player
{
    public class JumpBehavior : MonoBehaviour
    {
        Rigidbody rb;
        [SerializeField] JumpStats jumpStats;
        [SerializeField] float flySpeed;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        public bool TryStartJump()
        {
            if (IsGrounded())
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                rb.AddForce(0, jumpStats.JumpImpulse, 0, ForceMode.Impulse);
                return true;
            }
            return false;
        }

        public void Fly(bool isUpwards)
        {
            float flyForce = isUpwards ? flySpeed : -flySpeed;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, flyForce, rb.linearVelocity.z);
        }

        public void StopFly()
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        }

        public bool IsGrounded()
        {
            Vector3 origin = transform.position + Vector3.down * jumpStats.distanceToGroundCheck;
            return Physics.CheckSphere(origin, jumpStats.groundCheckRadius, jumpStats.groundLayer);
        }

        private void OnDrawGizmosSelected() {
            if (jumpStats != null)
            {
                Vector3 origin = transform.position + Vector3.down * jumpStats.distanceToGroundCheck;
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(origin, jumpStats.groundCheckRadius);
            }
        }
    }
}