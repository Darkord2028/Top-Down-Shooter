/// <summary>
/// State machine that contaions two functions and a refernce to current state
/// </summary>
public class PlayerStateMachine
{
    public PlayerState CurrentState { get; private set; }

    /// <summary>
    /// It is used to Initialize state during start
    /// </summary>
    /// <param name="startingState"></param>
    public void InitializeState(PlayerState startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    /// <summary>
    /// It is used to Change State
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(PlayerState newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

}
