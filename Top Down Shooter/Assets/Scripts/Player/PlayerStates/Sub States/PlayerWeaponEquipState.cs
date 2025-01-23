using UnityEngine;

public class PlayerWeaponEquipState : PlayerAbilityState
{
    private PickUpWeaponItem pickUpWeaponItem;
    private bool isRightHandWeapn;

    public PlayerWeaponEquipState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();

        if (isRightHandWeapn)
        {
            player.EquipmentManager.LoadWeapon(pickUpWeaponItem.RightHandPickUpWeapon);
        }
        else
        {
            player.EquipmentManager.LoadWeapon(pickUpWeaponItem.LeftHandPickUpWeapon);
        }

        isAbilityDone = true;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        player.EquipmentManager.SetIKWeight(0);
    }

    public override void Exit()
    {
        base.Exit();
        player.EquipmentManager.SetIKWeight(1);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public void GetPickUpWeapon(PickUpWeaponItem pickUpWeapon, bool RightHandWeapon = false)
    {
        pickUpWeaponItem = pickUpWeapon;
        isRightHandWeapn = RightHandWeapon;
    }

}
