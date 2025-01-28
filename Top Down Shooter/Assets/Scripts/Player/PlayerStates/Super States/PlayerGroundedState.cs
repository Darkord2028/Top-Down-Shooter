using UnityEngine;

/// <summary>
/// Base class when player is touching ground and not using any ability
/// </summary>
public class PlayerGroundedState : PlayerState
{
    protected Vector2 MovementInput;
    protected float moveAmount;
    protected bool EquipRightHandWeaponInput;
    protected bool EquipLeftHandWeaponInput;
    protected bool DodgeInput;

    protected PickUpWeaponItem pickUpWeaponItem;

    public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
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
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        MovementInput = player.InputManager.MovementInput;
        moveAmount = player.InputManager.moveAmount;
        EquipRightHandWeaponInput = player.InputManager.EquipRightHandWeaponInput;
        EquipLeftHandWeaponInput = player.InputManager.EquipLeftHandWeaponInput;
        DodgeInput = player.InputManager.DodgeInput;

        pickUpWeaponItem = player.GetWeaponOnCollision();

        if (pickUpWeaponItem != null && EquipRightHandWeaponInput)
        {
            player.InputManager.UseEquipRightHandWeaponInput();
            player.WeaponEquipState.GetPickUpWeapon(pickUpWeaponItem, true);
            player.StateMachine.ChangeState(player.WeaponEquipState);
        }
        else if (pickUpWeaponItem != null && EquipLeftHandWeaponInput)
        {
            player.InputManager.UseEquipLeftHandWeaponInput();
            player.WeaponEquipState.GetPickUpWeapon(pickUpWeaponItem, false);
            player.StateMachine.ChangeState(player.WeaponEquipState);
        }
        else if (DodgeInput)
        {
            player.InputManager.UseDodgeInput();

            if (player.DodgeState.CanDodge())
            {
                player.StateMachine.ChangeState(player.DodgeState);
            }
        }
        else
        {
            player.InputManager.UseEquipLeftHandWeaponInput();
            player.InputManager.UseEquipRightHandWeaponInput();
        }

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

}
