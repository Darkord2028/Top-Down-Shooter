using UnityEngine;

/// <summary>
/// This is the base script that runs the updates loop as well as manages and 
/// stores the references of states.
/// </summary>
public class Player : MonoBehaviour
{
    #region State Variables

    //Creating state machines
    public PlayerStateMachine StateMachine { get; private set; }

    //Creating different states
    public PlayerLocomotionState LocomotionState { get; private set; }
    public PlayerWeaponEquipState WeaponEquipState { get; private set; }
    public PlayerDodgeState DodgeState { get; private set; }

    #endregion

    #region Public Get - Private Set Variables

    //Different components attached to the player
    public Animator animator { get; private set; }
    public CharacterController characterController { get; private set; }
    public PlayerInputManager InputManager { get; private set; }
    public PlayerEquipmentManager EquipmentManager { get; private set; }
    public Health PlayerHealth { get; private set; }
    public PlayerUpgrade PlayerUpgrade { get; private set; }

    public Vector2 playerVelocity;

    public int MaxAbilityToGainUpgradePoint { get; private set; }
    public int CurrentAbility { get; private set; }
    public int CurrentLevel { get; private set; }

    #endregion

    #region Inspector Variables

    [Header("Debug Animation Bool Name")]
    public bool debugAnimationBoolName;

    [Header("Player Data")]
    public PlayerData playerData;

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
        DodgeState = new PlayerDodgeState(this, StateMachine, playerData, "dodge");
    }

    //Get component reference in Start and Initializing starting state
    private void Start()
    {
        //Getting required components
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        InputManager = GetComponent<PlayerInputManager>();
        EquipmentManager = GetComponent<PlayerEquipmentManager>();
        PlayerHealth = GetComponent<Health>();
        PlayerUpgrade = GetComponent<PlayerUpgrade>();

        //Initializing the player health UI as well as hooking up event to DamagePlayerHealth function
        WorldUIManager.instance.InitializePlayerHealth(PlayerHealth._MaxHealth, PlayerHealth._Health);
        PlayerHealth.OnTakeDamage += WorldUIManager.instance.DamagePlayerHealth;

        MaxAbilityToGainUpgradePoint = playerData.InitialMaxAbilityPoint;
        WorldUIManager.instance.InitializePlayerAbility(CurrentAbility, MaxAbilityToGainUpgradePoint, CurrentLevel);

        //Hooking up death event
        PlayerHealth.OnDeath += HandleDeath;

        //Initializing Current State
        StateMachine.InitializeState(LocomotionState);
        StateMachine.CurrentState.Enter();

        Time.timeScale = 1.0f;
    }

    //Updating the base logic of the current state.
    private void Update()
    {
        StateMachine.CurrentState.LogicUpdate();

        if (WorldUIManager.instance.currentWave > 5)
        {
            HandleDeath(transform.position);
        }
    }

    //Updating the physics of current state
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

    /// <summary>
    /// Setting up ability point as well as updating ability slider UI.
    /// </summary>
    /// <param name="abilityPoint"></param>
    public void SetAbilityPoint(int abilityPoint)
    {
        CurrentAbility += abilityPoint;
        WorldUIManager.instance.UpdatePlayerAbility(CurrentAbility);

        if(CurrentAbility >= MaxAbilityToGainUpgradePoint)
        {
            CurrentLevel += 1;
            CurrentAbility -= MaxAbilityToGainUpgradePoint;
            MaxAbilityToGainUpgradePoint += Mathf.FloorToInt(MaxAbilityToGainUpgradePoint * playerData.AbilityPointMultiplier / 100f);
            WorldUIManager.instance.InitializePlayerAbility(CurrentAbility, MaxAbilityToGainUpgradePoint, CurrentLevel);
            PlayerUpgrade.EnableUpgradeTree();
            Time.timeScale = 0.0f;
        }
        
    }

    /// <summary>
    /// If Player health drops to zero. Save high score and set death UI
    /// </summary>
    /// <param name="position"></param>
    private void HandleDeath(Vector3 position)
    {
        SaveManager saveManager = new SaveManager();
        saveManager.SaveHighScore(CurrentAbility);
        saveManager.SaveTimeInMinutes(WorldUIManager.instance.seconds);
        saveManager.SaveTimeInSeconds(WorldUIManager.instance.minutes);

        WorldUIManager.instance.SetDeathUI(false);
        Time.timeScale = 0.0f;
    }

    /// <summary>
    /// If current wave = 5. Set Win UI.
    /// </summary>
    private void HandleWin()
    {
        SaveManager saveManager = new SaveManager();
        saveManager.SaveHighScore(CurrentAbility);
        saveManager.SaveTimeInMinutes(WorldUIManager.instance.seconds);
        saveManager.SaveTimeInSeconds(WorldUIManager.instance.minutes);

        WorldUIManager.instance.SetDeathUI(true);
        Time.timeScale = 0.0f;
    }

    #endregion

    #region Animation Trigger Functions

    //Animation triggers used that could be used during states
    private void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();


    //Animations triggers that can be used to exit out off states
    private void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();

    #endregion

    #region Do Check Functions

    /// <summary>
    /// Checks if player is touching ground and returns bool value.
    /// </summary>
    /// <returns></returns>
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


    //Check is player is around weapon which it can pick up.
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

    /// <summary>
    /// Drawing gizmos for scene view.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(groundCheck.position, playerData.groundCheckRadius);
        Gizmos.DrawWireSphere(gunCheck.position, playerData.gunCheckRadius);
    }

    #endregion

}
