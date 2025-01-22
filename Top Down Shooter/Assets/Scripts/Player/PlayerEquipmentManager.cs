using System.Collections;
using System.Runtime.ExceptionServices;
using Unity.VisualScripting;
using UnityEngine;
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
    [SerializeField] TwoBoneIKConstraint leftHandIK;
    private RigBuilder rigBuilder;

    [Header("Weapon Loader Slot")]
    [SerializeField] Transform rightHandLoaderSlot;
    [SerializeField] Transform leftHandLoaderSlot;

    private ParticleSystem rightHandWeaponParticleSystem;
    private ParticleSystem leftHandWeaponParticleSystem;

    private AudioSource rightHandWeaponAudioSource;
    private AudioSource leftHandWeaponAudioSource;

    private bool isRightHandEquipped;
    private bool isLeftHandEquipped;

    private float rightHandLastShootTime;
    private float leftHandLastShootTime;

    #region Unity Callback Function

    private void Start()
    {
        rigBuilder = GetComponent<RigBuilder>();
        player = GetComponent<Player>();

        LoadWeapon(rightHandWeapon);
        rightHandWeapon.Initialize(player, rightHandWeaponParticleSystem, rightHandWeaponAudioSource);

        LoadWeapon(leftHandWeapon);
        leftHandWeapon.Initialize(player, leftHandWeaponParticleSystem, leftHandWeaponAudioSource);
    }

    private void Update()
    {
        ShootRightWeapon();
        ShootLeftWeapon();
    }

    #endregion

    #region Load Weapon

    /// <summary>
    /// It instantiates the weapon model as well as assign currentWeaponModel and HandIK target for player to hold the weapon.
    /// </summary>
    /// <param name="weapon"></param>
    private void LoadWeapon(WeaponScriptableObject weapon)
    {

        if (!isRightHandEquipped)
        {
            if (currentRightHandWeaponModel != null)
            {
                UnloadAndDestroyWeapon(currentRightHandWeaponModel);
            }

            currentRightHandWeaponModel = Instantiate(rightHandWeapon.itemModel);
            currentRightHandWeaponModel.transform.SetParent(rightHandLoaderSlot.transform, true);
            currentRightHandWeaponModel.transform.position = rightHandLoaderSlot.position;
            AssignHandIK(currentRightHandWeaponModel.GetComponentInChildren<RightHandIKTarget>());
            rightHandWeaponParticleSystem = currentRightHandWeaponModel.GetComponentInChildren<ParticleSystem>();
            rightHandWeaponAudioSource = currentRightHandWeaponModel.GetComponentInChildren<AudioSource>();
            isRightHandEquipped = true;
        }
        else if (!isLeftHandEquipped)
        {
            if (currentLeftHandWeaponModel != null)
            {
                UnloadAndDestroyWeapon(currentLeftHandWeaponModel);
            }

            currentLeftHandWeaponModel = Instantiate(leftHandWeapon.itemModel);
            currentLeftHandWeaponModel.transform.SetParent(leftHandLoaderSlot.transform, true);
            currentLeftHandWeaponModel.transform.position = leftHandLoaderSlot.position;
            AssignHandIK(currentLeftHandWeaponModel.GetComponentInChildren<LeftHandIKTarget>());
            leftHandWeaponParticleSystem = currentLeftHandWeaponModel.GetComponentInChildren<ParticleSystem>();
            leftHandWeaponAudioSource = currentLeftHandWeaponModel.GetComponentInChildren<AudioSource>();
            isLeftHandEquipped = true;
        }
        else
        {
            Debug.LogWarning("Can not Equip weapon!");
        }
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
    private void AssignHandIK(RightHandIKTarget rightHandIKTarget)
    {
        rightHandIK.data.target = rightHandIKTarget.transform;
        rigBuilder.Build();
    }

    private void AssignHandIK(LeftHandIKTarget leftHandIKTarget)
    {
        leftHandIK.data.target = leftHandIKTarget.transform;
        rigBuilder.Build();
    }

    #endregion

    #region Shoot Functions

    public void ShootRightWeapon()
    {

        if (Time.time > rightHandWeapon.firerate + rightHandLastShootTime)
        {
            rightHandLastShootTime = Time.time;

            rightHandWeaponParticleSystem.Play();
            PlayShootAudio(rightHandWeaponAudioSource, rightHandWeapon.shootAudioClip);

            rightHandWeapon.Shoot();
        }
    }

    public void ShootLeftWeapon()
    {

        if (Time.time > leftHandWeapon.firerate + leftHandLastShootTime)
        {
            leftHandLastShootTime = Time.time;

            leftHandWeaponParticleSystem.Play();
            PlayShootAudio(leftHandWeaponAudioSource, leftHandWeapon.shootAudioClip);

            leftHandWeapon.Shoot();
        }
    }

    #endregion

    #region Audio Functions

    private void PlayShootAudio(AudioSource audioSource, AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    #endregion

}