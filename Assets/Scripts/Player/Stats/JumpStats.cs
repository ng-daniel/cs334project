using UnityEngine;

[CreateAssetMenu(fileName = "JumpStats", menuName = "Scriptable Objects/JumpStats")]
public class JumpStats : ScriptableObject
{
    [SerializeField] public float JumpImpulse;
    [SerializeField] public float distanceToGroundCheck;
    [SerializeField] public float groundCheckRadius;
    [SerializeField] public LayerMask groundLayer;
}
