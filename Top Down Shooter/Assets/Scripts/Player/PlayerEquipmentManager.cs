using System.Collections;
using System.Runtime.ExceptionServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

public class PlayerEquipmentManager : MonoBehaviour
{
    private Player player;

    [Header("Current Weapon")]
    public WeaponScriptableObject rightHandWeapon;
    public WeaponScriptableObject leftHandWeapon;

    private GameObject currentRightHandWeaponModel;
    private GameObject currentLeftHandWeaponModel;

    [Header("Hand IK")]
    [SerializeField] TwoBoneIKConstraint rightHandIK;
    [SerializeField] Transform rightHandIKTarget;
    [SerializeField] TwoBoneIKConstraint leftHandIK;
    [SerializeField] Transform leftHandIKTarget;
    private RigBuilder rigBuilder;

    [Header("Weapon Loader Slot")]
    [SerializeField] Transform rightHandLoaderSlot;
    [SerializeField] Transform leftHandLoaderSlot;

    private ParticleSystem rightHandWeaponParticleSystem;
    private ParticleSystem leftHandWeaponParticleSystem;

    private AudioSource rightHandWeaponAudioSource;
    private AudioSource leftHandWeaponAudioSource;

    private float rightHandLastShootTime;
    private float leftHandLastShootTime;

    #region Unity Callback Function

    private void Start()
    {
        rigBuilder = GetComponent<RigBuilder>();
        player = GetComponent<Player>();

        LoadWeapon(rightHandWeapon);
        LoadWeapon(leftHandWeapon);
        rigBuilder.Build();
    }

    #endregion

    #region Load Weapon

    /// <summary>
    /// It instantiates the weapon model as well as assign currentWeaponModel and HandIK target for player to hold the weapon.
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

    #endregion

    #region Unload Weapon

    /// <summary>
    /// It destroys and unloads the current weapon model from the game.
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

    public void SetIKWeight(float weight)
    {
        leftHandIK.weight = weight;
        rightHandIK.weight = weight;
    }

    private void ToggleRiglayer(bool enable)
    {
        foreach (RigLayer rigLayer in rigBuilder.layers)
        {
            rigLayer.active = enable;
        }
    }

    #endregion

    #region Shoot Functions

    public void ShootRightWeapon()
    {

        if (Time.time > rightHandWeapon.firerate + rightHandLastShootTime)
        {
            rightHandLastShootTime = Time.time;
            rightHandWeapon.Shoot();
        }
    }

    public void ShootLeftWeapon()
    {

        if (Time.time > leftHandWeapon.firerate + leftHandLastShootTime)
        {
            leftHandLastShootTime = Time.time;
            leftHandWeapon.Shoot();
        }
    }

    #endregion

    #region Set Functions

    public void ToggleWeaponModel(bool enable)
    {
        currentLeftHandWeaponModel.gameObject.SetActive(enable);
        currentRightHandWeaponModel.gameObject.SetActive(enable);
    }

    #endregion

}
