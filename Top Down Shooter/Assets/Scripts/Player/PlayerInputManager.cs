using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    #region Input Flags

    public Vector2 MovementInput { get; private set; }
    public float moveAmount { get; private set; }

    #endregion

    #region Unity Callback Functions

    private void Update()
    {
        HandleRawMovementInput();
    }

    #endregion

    #region Input Actions

    //Reads the movement input and updates MovementInput variable.
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
    }

    #endregion

    #region Handle Input Functions

    /// <summary>
    /// Handles the raw movement and converts the Vector2 movement value to absolute float value.
    /// </summary>
    private void HandleRawMovementInput()
    {
        moveAmount = Mathf.Clamp01(Mathf.Abs(MovementInput.x) + Mathf.Abs(MovementInput.y));

        if (moveAmount <= 0.5f && moveAmount > 0f)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5f && moveAmount <= 1f)
        {
            moveAmount = 1f;
        }
    }

    #endregion

}
