using UnityEngine;

public class PlayerLocomotionState : PlayerGroundedState
{
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
        player.SetMovement(playerData.moveSpeed);
        player.animator.SetFloat("moveY", moveAmount);

        //player.EquipmentManager.Shoot(player.EquipmentManager.rightHandWeapon, true);
        //player.EquipmentManager.Shoot(player.EquipmentManager.leftHandWeapon, false);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
