using UnityEngine;

/// <summary>
/// Base class that handles ability transition from one state to another.
/// </summary>
public class PlayerAbilityState : PlayerState
{
    protected bool isAbilityDone;

    public PlayerAbilityState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
        isAbilityDone = false;

        player.EquipmentManager.SetIKWeight(0);
        player.EquipmentManager.ToggleWeaponModel(false);
    }

    public override void Exit()
    {
        base.Exit();

        player.EquipmentManager.SetIKWeight(1);
        player.EquipmentManager.ToggleWeaponModel(true);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(isAbilityDone && !isExitingState)
        {
            StateMachine.ChangeState(player.LocomotionState);
        }

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public bool CanDodge()
    {
        if(Time.time > playerData.timeBetweenDodge + startTime)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
