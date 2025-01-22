using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    protected Vector2 MovementInput;
    protected float moveAmount;

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

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

}
