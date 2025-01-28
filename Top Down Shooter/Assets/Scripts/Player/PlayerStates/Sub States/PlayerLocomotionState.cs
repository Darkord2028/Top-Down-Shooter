using UnityEngine;

/// <summary>
/// Locomotion state, that handles runs the logic of moving and rotationg the player while shooting
/// </summary>
public class PlayerLocomotionState : PlayerGroundedState
{
    public float speedUpgrade;

    public PlayerLocomotionState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
        player.animator.SetFloat("moveX", 0);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        player.SetRotation(playerData.movementRotation);
        player.SetMovement(playerData.moveSpeed + speedUpgrade);
        player.animator.SetFloat("moveY", moveAmount);

        player.EquipmentManager.ShootRightWeapon();
        player.EquipmentManager.ShootLeftWeapon();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
