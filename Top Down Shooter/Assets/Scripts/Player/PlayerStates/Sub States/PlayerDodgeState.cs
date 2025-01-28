using UnityEngine;

/// <summary>
/// Player Dodge state, trasitions are handles by animation triggers
/// </summary>
public class PlayerDodgeState : PlayerAbilityState
{
    private int dodgeMultiplier;

    public PlayerDodgeState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();

        isAbilityDone = true;
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();

        dodgeMultiplier = 0;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        player.PlayerHealth.canDamage = false;
        isAbilityDone = false;
        dodgeMultiplier = 1;
    }

    public override void Exit()
    {
        base.Exit();
        player.PlayerHealth.canDamage = true;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        player.SetMovement(playerData.dodgeSpeed * dodgeMultiplier);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

}
