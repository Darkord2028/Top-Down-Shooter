using UnityEngine;
using UnityEngine.Animations.Rigging;

/// <summary>
/// Base class that handles equipment of the player
/// It handles loading and unloading of weapons and special weapons
/// It also handles IK for holding the weapon
/// </summary>
public class PlayerEquipmentManager : MonoBehaviour
{
    private Player player;

    [Header("Current Weapon")]
    public WeaponScriptableObject rightHandWeapon;
    public WeaponScriptableObject leftHandWeapon;

    private GameObject currentRightHandWeaponModel;
    private GameObject currentLeftHandWeaponModel;

    [Header("Current Special Weapon")]
    [SerializeField] SpecialWeaponScriptableObject[] specialWeapon;

    [Header("Hand IK")]
    [SerializeField] TwoBoneIKConstraint rightHandIK;
    [SerializeField] Transform rightHandIKTarget;
    [SerializeField] TwoBoneIKConstraint leftHandIK;
    [SerializeField] Transform leftHandIKTarget;
    private RigBuilder rigBuilder;

    [Header("Weapon Loader Slot")]
    [SerializeField] Transform rightHandLoaderSlot;
    [SerializeField] Transform leftHandLoaderSlot;
    [SerializeField] Transform specialWeaponLoaderSlot;

    private ParticleSystem rightHandWeaponParticleSystem;
    private ParticleSystem leftHandWeaponParticleSystem;

    private AudioSource rightHandWeaponAudioSource;
    private AudioSource leftHandWeaponAudioSource;

    private float rightHandLastShootTime;
    private float leftHandLastShootTime;

    #region Unity Callback Function

    /// <summary>
    /// Storring component references.
    /// Loading weapons and special weapons.
    /// Building the rig.
    /// </summary>
    private void Start()
    {
        rigBuilder = GetComponent<RigBuilder>();
        player = GetComponent<Player>();

        LoadWeapon(rightHandWeapon);
        LoadWeapon(leftHandWeapon);
        LoadSpecialWeapon();

        rigBuilder.Build();
    }

    /// <summary>
    /// Handling shooting of special weapon
    /// </summary>
    private void Update()
    {
        for (int i = 0; i < specialWeapon.Length; i++)
        {
            if(specialWeapon[i] != null)
            {
                specialWeapon[i].Shoot();
            }
        }
    }

    #endregion

    #region Load Weapon

    /// <summary>
    /// Instantiating weapon models, initializing weapon as well as assigning hand IK for holding weapons.
    /// </summary>
    /// <param name="weapon"></param>
    public void LoadWeapon(WeaponScriptableObject weapon)
    {
        if (weapon.isRightHandedWeapon)
        {
            if (currentRightHandWeaponModel != null)
            {
                WorldUIManager.instance.SetWeaponDefaultSprite(true);
                UnloadAndDestroyWeapon(currentRightHandWeaponModel);
            }

            rightHandWeapon = weapon;
            WorldUIManager.instance.SetWeaponSprite(weapon);

            currentRightHandWeaponModel = Instantiate(rightHandWeapon.itemModel);
            currentRightHandWeaponModel.transform.SetParent(rightHandLoaderSlot.transform, false);
            currentRightHandWeaponModel.transform.position = rightHandLoaderSlot.position;

            rightHandWeaponParticleSystem = currentRightHandWeaponModel.GetComponentInChildren<ParticleSystem>();
            rightHandWeaponAudioSource = currentRightHandWeaponModel.GetComponentInChildren<AudioSource>();
            rightHandWeapon.Initialize(player, rightHandWeaponParticleSystem, rightHandWeaponAudioSource);
        }
        else if (!weapon.isRightHandedWeapon)
        {
            if (currentLeftHandWeaponModel != null)
            {
                WorldUIManager.instance.SetWeaponDefaultSprite(false);
                UnloadAndDestroyWeapon(currentLeftHandWeaponModel);
            }

            leftHandWeapon = weapon;
            WorldUIManager.instance.SetWeaponSprite(weapon);
            
            currentLeftHandWeaponModel = Instantiate(leftHandWeapon.itemModel);
            currentLeftHandWeaponModel.transform.SetParent(leftHandLoaderSlot.transform, false);
            currentLeftHandWeaponModel.transform.position = leftHandLoaderSlot.position;

            leftHandWeaponParticleSystem = currentLeftHandWeaponModel.GetComponentInChildren<ParticleSystem>();
            leftHandWeaponAudioSource = currentLeftHandWeaponModel.GetComponentInChildren<AudioSource>();
            leftHandWeapon.Initialize(player, leftHandWeaponParticleSystem, leftHandWeaponAudioSource);
        }

        AssignHandIK(weapon);
    }


    /// <summary>
    /// Instantiating special weapons and Initializing them.
    /// </summary>
    private void LoadSpecialWeapon()
    {
        for (int i = 0; i < specialWeapon.Length; i++)
        {
            GameObject instance = Instantiate(specialWeapon[i].itemModel);
            instance.transform.SetParent(specialWeaponLoaderSlot.transform, false);
            FirepointTransform transforms = instance.GetComponent<FirepointTransform>();
            specialWeapon[i].Initialize(transforms.firepointTransform, player);
        }
    }

    #endregion

    #region Unload Weapon

    /// <summary>
    /// It destroys and unloads the current weapon model to equip different weapon.
    /// </summary>
    /// <param name="currentWeapon"></param>
    private void UnloadAndDestroyWeapon(GameObject currentWeapon)
    {
        Destroy(currentWeapon);
    }

    #endregion

    #region Weapon IK system

    /// <summary>
    /// Responsible for assigning HandIK target for player to equip weapon.
    /// </summary>
    /// <param name="rightHandIKTarget"></param>
    /// <param name="leftHandIKTarget"></param>
    private void AssignHandIK(WeaponScriptableObject weapon)
    {
        if (weapon.isRightHandedWeapon)
        {
            rightHandIKTarget.transform.localPosition = weapon.HandPosition;
            rightHandIKTarget.transform.localRotation = weapon.HandRotation;
        }
        else
        {
            leftHandIKTarget.transform.localPosition = weapon.HandPosition;
            leftHandIKTarget.transform.localRotation = weapon.HandRotation;
        }
    }

    /// <summary>
    /// Function to set up IK weight
    /// </summary>
    /// <param name="weight"></param>
    public void SetIKWeight(float weight)
    {
        leftHandIK.weight = weight;
        rightHandIK.weight = weight;
    }

    #endregion

    #region Shoot Functions

    /// <summary>
    /// Handles shooting in a time interval for Right Hand Weapon
    /// </summary>
    public void ShootRightWeapon()
    {

        if (Time.time > rightHandWeapon.firerate + rightHandWeapon.upgradeFirerate + rightHandLastShootTime)
        {
            rightHandLastShootTime = Time.time;
            rightHandWeapon.Shoot();
        }
    }


    /// <summary>
    /// Handles shooting in a time interval for Left Hand Weapon
    /// </summary>
    public void ShootLeftWeapon()
    {

        if (Time.time > leftHandWeapon.firerate + leftHandWeapon.upgradeFirerate + leftHandLastShootTime)
        {
            leftHandLastShootTime = Time.time;
            leftHandWeapon.Shoot();
        }
    }

    /// <summary>
    /// Handles shooting in a time interval for Special Weapon
    /// </summary>
    public void ShootSpecialWeapon()
    {
        foreach(SpecialWeaponScriptableObject weapon in specialWeapon)
        {
            if (weapon != null)
            {
                weapon.Shoot();
            }
            else
            {
                break;
            }
        }
    }

    #endregion

    #region Set Functions

    /// <summary>
    /// Toggling weapon gameobject for picking guns.
    /// </summary>
    /// <param name="enable"></param>
    public void ToggleWeaponModel(bool enable)
    {
        currentLeftHandWeaponModel.gameObject.SetActive(enable);
        currentRightHandWeaponModel.gameObject.SetActive(enable);
    }

    #endregion

}
