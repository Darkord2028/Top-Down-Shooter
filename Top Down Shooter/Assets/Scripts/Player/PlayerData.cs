using UnityEngine;

/// <summary>
/// This scriptable object holds the base data for player
/// </summary>
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
    public float movementRotation;

    [Header("Dodge State")]
    public float dodgeSpeed;
    public float timeBetweenDodge;

    [Header("Player Gun Check")]
    public float gunCheckRadius;
    public LayerMask gunCheckLayerMask;
    public LayerMask gemCheckLayer;

    [Header("Player Ability Slider")]
    public int InitialMaxAbilityPoint;
    public float AbilityPointMultiplier;

}
