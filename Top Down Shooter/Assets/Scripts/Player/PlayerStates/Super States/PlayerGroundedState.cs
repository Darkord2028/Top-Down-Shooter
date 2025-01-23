using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    protected Vector2 MovementInput;
    protected float moveAmount;
    protected bool EquipRightHandWeaponInput;
    protected bool EquipLeftHandWeaponInput;

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
