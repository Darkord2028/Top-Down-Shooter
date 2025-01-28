using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    #region Input Flags

    //Input flags for storing inputs
    public Vector2 MovementInput { get; private set; }
    public float moveAmount { get; private set; }
    public bool EquipRightHandWeaponInput { get; private set; }
    public bool EquipLeftHandWeaponInput { get; private set; }
    public bool DodgeInput { get; private set; }

    #endregion

    #region Unity Callback Functions

    //Updating Movement Input logic
    private void Update()
    {
        HandleRawMovementInput();
    }

    #endregion

    #region Input Actions

    //Reading and stroing Input through Unity events.

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
    }

    public void OnEquipRightHandWeapon(InputAction.CallbackContext context)
    {
        if(context.started) EquipRightHandWeaponInput = true;
    }

    public void OnEquipLeftHandWeapon(InputAction.CallbackContext context)
    {
        if (context.started) EquipLeftHandWeaponInput = true;
    }

    public void OnDodgeInput(InputAction.CallbackContext context)
    {
        if (context.started) DodgeInput = true;
    }

    public void OnPauseInput(InputAction.CallbackContext context)
    {
        if (context.started) Time.timeScale = 0.0f;
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

    #region Use Input Function

    //Use Input flags after using it

    public void UseEquipRightHandWeaponInput() => EquipRightHandWeaponInput = false;
    public void UseEquipLeftHandWeaponInput() => EquipLeftHandWeaponInput = false;
    public void UseDodgeInput() => DodgeInput = false;

    #endregion

}
