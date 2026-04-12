using UnityEngine;

[CreateAssetMenu(fileName = "MovementStats", menuName = "Scriptable Objects/MovementStats")]
public class MovementStats : ScriptableObject
{
    [SerializeField] public float WalkSpeed;
    [SerializeField] public float WalkSpeedAccel;
}