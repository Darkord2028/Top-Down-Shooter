using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player/Base Data")]
public class PlayerData : ScriptableObject
{
    [Header("Base Data")]
    public float gravity;
    public float downwardForce;
    public float groundCheckRadius;
    public LayerMask whatIsGround;

    [Header("Locomotion State")]
    public float moveSpeed;
    public float sprintSpeed;
    public float movementRotation;

}
