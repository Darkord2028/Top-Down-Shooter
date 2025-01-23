using UnityEngine;

public class Player : MonoBehaviour
{
    #region State Variables

    //State Machine
    public PlayerStateMachine StateMachine { get; private set; }

    //Different States
    public PlayerLocomotionState LocomotionState { get; private set; }
    public PlayerWeaponEquipState WeaponEquipState { get; private set; }

    #endregion

    #region Public Get - Private Set Variables

    public Animator animator { get; private set; }
    public CharacterController characterController { get; private set; }
    public PlayerInputManager InputManager { get; private set; }
    public PlayerEquipmentManager EquipmentManager { get; private set; }

    public Vector2 playerVelocity;

    #endregion

    #region Inspector Variables

    [Header("Debug Animation Bool Name")]
    public bool debugAnimationBoolName;

    [Header("Player Data")]
    [SerializeField] PlayerData playerData;

    [Header("Player Camera")]
    [SerializeField] Transform playerCamera;

    [Header("Player Checks")]
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform gunCheck;

    #endregion

    #region Unity Callback Functions

    //Creating State Machine and States before calling Start
    private void Awake()
    {
        StateMachine = new PlayerStateMachine();

        LocomotionState = new PlayerLocomotionState(this, StateMachine, playerData, "move");
        WeaponEquipState = new PlayerWeaponEquipState(this, StateMachine, playerData, "equipWeapon");
    }

    //Get component reference in Start and Initializing starting state
    private void Start()
    {
        //Getting required components
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        InputManager = GetComponent<PlayerInputManager>();
        EquipmentManager = GetComponent<PlayerEquipmentManager>();

        //Initializing Current State
        StateMachine.InitializeState(LocomotionState);
        StateMachine.CurrentState.Enter();
    }

    private void Update()
    {
        StateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }

    #endregion

    #region Set Functions

    /// <summary>
    /// Handles and applies gravity if player is in the air
    /// </summary>
    public void HandleGravity()
    {
        playerVelocity.y = playerVelocity.y + playerData.gravity * Time.deltaTime;
        if (isGrounded() && playerVelocity.y < 0)
        {
            playerVelocity.y = playerData.downwardForce;
        }
        characterController.Move(playerVelocity * Time.deltaTime);

    }

    /// <summary>
    /// Responsable for movement of the player. Can be accessed across different states.
    /// </summary>
    /// <param name="movementSpeed"></param>
    public void SetMovement(float movementSpeed)
    {
        Vector3 moveDirection;
        moveDirection = playerCamera.forward * InputManager.MovementInput.y;
        moveDirection = moveDirection + playerCamera.right * InputManager.MovementInput.x;
        moveDirection.Normalize();
        moveDirection = moveDirection * movementSpeed;

        Vector3 movementVelocity = moveDirection;
        characterController.Move(movementVelocity * Time.deltaTime);
    }

    /// <summary>
    /// Responsable for player rotation during movement. Can be accessed across different states.
    /// </summary>
    /// <param name="rotationSpeed"></param>
    public void SetRotation(float rotationSpeed)
    {
        Vector3 targetDirection = Vector3.zero;

        targetDirection = playerCamera.forward * InputManager.MovementInput.y;
        targetDirection = targetDirection + playerCamera.right * InputManager.MovementInput.x;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = playerRotation;
    }

    #endregion

    #region Animation Trigger Functions

    private void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();

    private void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();

    #endregion

    #region Do Check Functions

    public bool isGrounded()
    {
        Collider[] hitColliders = new Collider[10];
        int numColliders = Physics.OverlapSphereNonAlloc(groundCheck.position, playerData.groundCheckRadius, hitColliders, playerData.whatIsGround);

        foreach (Collider collider in hitColliders)
        {
            if (collider != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        if (characterController.isGrounded)
        {
            return true;
        }

        return false;
    }

    public PickUpWeaponItem GetWeaponOnCollision()
    {
        Collider[] hitColliders = new Collider[10];

        Physics.OverlapSphereNonAlloc(gunCheck.transform.position, playerData.gunCheckRadius, hitColliders, playerData.gunCheckLayerMask);

        foreach (Collider collider in hitColliders)
        {
            if (collider == null)
            {
                WorldUIManager.instance.SetPickUpWeaponUI(null);
                return null;
            }

            if (collider.TryGetComponent<PickUpWeaponItem>(out PickUpWeaponItem pickUpWeaponItem))
            {
                WorldUIManager.instance.SetPickUpWeaponUI(pickUpWeaponItem.LeftHandPickUpWeapon);
                return pickUpWeaponItem;
            }
            else
            {
                WorldUIManager.instance.SetPickUpWeaponUI(null);
                return null;
            }
        }

        return null;

    }

    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(groundCheck.position, playerData.groundCheckRadius);
        Gizmos.DrawWireSphere(gunCheck.position, playerData.gunCheckRadius);
    }

    #endregion

}
