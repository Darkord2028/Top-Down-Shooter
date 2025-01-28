using System.Collections;
using UnityEngine;
using static UnityEngine.ParticleSystem;

/// <summary>
/// This scriptable object inherts from base ItemScriptableObject and extends it to become a weapon class of its own.
/// </summary>
[CreateAssetMenu(fileName = "newWeaponItem", menuName = "Data/Items/Weapon Item")]
public class WeaponScriptableObject : ItemScriptableObject
{

    #region Weapon Information

    [Header("Weapon Information")]
    public bool isRightHandedWeapon;
    public bool isHitscanWeapon;

    #endregion

    #region Weapon IK

    [Header("Weapon IK Transform")]
    public Vector3 HandPosition;
    public Quaternion HandRotation;


    #endregion

    #region Weapon Shoot Configuration

    [Header("Weapon Shoot Configuration")]
    public LayerMask hitMask;
    public float firerate = 0.25f;
    public int bulletsPerShot = 1;
    public Vector3 shootSpread = new Vector3(0.1f, 0.1f, 0.1f);

    #endregion

    #region Weapon trail Configuration

    [Header("Weapon Trail Configuration")]
    public Material trailMaterial;
    public AnimationCurve trailWidthCurve;
    public float trailDuration = 0.5f;
    public float trailMinVertexDistance = 0.1f;
    public Gradient trailColor;
    public float trailMissDistance = 100;
    public float simulationSpeed = 100f;

    #endregion

    #region Weapon Damage Configuration

    [Header("Weapon Damage Configuration")]
    public MinMaxCurve damageCurve;

    #endregion

    #region Weapon Audio Configuration

    [Header("Weapon Audio Configuration")]
    public AudioClip shootAudioClip;

    #endregion

    #region Private Variables

    private MonoBehaviour activeMonobehaviour;
    private ParticleSystem shootParticleSystem;
    private AudioSource shootAudioSource;

    public int currentLevel { get; private set; }
    public float upgradeFirerate { get; private set; }
    private float upgradeMissDistance;
    private int upgradeBulletsPerShot;
    private int upgradeDamage;

    #endregion

    #region Initialize

    /// <summary>
    /// Initializing and storing reference for activeMonobehaviour which handles coroutines
    /// Also setting up upgrades to 0.
    /// </summary>
    /// <param name="monoBehaviour"></param>
    /// <param name="particleSystem"></param>
    /// <param name="audioSource"></param>
    public void Initialize(MonoBehaviour monoBehaviour, ParticleSystem particleSystem, AudioSource audioSource)
    {
        activeMonobehaviour = monoBehaviour;
        shootParticleSystem = particleSystem;
        shootAudioSource = audioSource;

        currentLevel = 0;
        upgradeBulletsPerShot = 0;
        upgradeFirerate = 0;
        upgradeMissDistance = 0;
    }

    #endregion

    #region Shoot Functions

    /// <summary>
    /// Handles shooting as well as particle system and shoot audio.
    /// Also handles raycasting,
    /// </summary>
    public void Shoot()
    {
        if(shootParticleSystem != null && shootParticleSystem.gameObject.activeSelf)
        {
            shootParticleSystem.Play();
        }
        if(shootAudioSource != null && shootAudioSource.gameObject.activeSelf)
        {
            PlayShootAudio(shootAudioSource, shootAudioClip);
        }

        for (int i = 0; i < bulletsPerShot + upgradeBulletsPerShot; i++)
        {
            Vector3 spreadAmount = GetSpread();
            Vector3 shootDirection;
            shootDirection = shootParticleSystem.transform.forward + spreadAmount;
            shootDirection.Normalize();

            if (Physics.Raycast(shootParticleSystem.transform.position, shootDirection, out RaycastHit hit, float.MaxValue, hitMask))
            {
                activeMonobehaviour.StartCoroutine(PlayTrail(shootParticleSystem.transform.position, hit.point, hit));
            }
            else
            {
                activeMonobehaviour.StartCoroutine(PlayTrail(shootParticleSystem.transform.position, shootParticleSystem.transform.position + (shootDirection * (trailMissDistance + upgradeMissDistance)), new RaycastHit()));
            }
        }
    }

    /// <summary>
    /// Handles trail travel from firepoint to hit point
    /// </summary>
    /// <param name="StartPoint"></param>
    /// <param name="EndPoint"></param>
    /// <param name="hit"></param>
    /// <returns></returns>
    private IEnumerator PlayTrail(Vector3 StartPoint, Vector3 EndPoint, RaycastHit hit)
    {
        TrailRenderer instance = WorldObjectPoolManager.instance.bulletTrailPool.Get();
        SetTrailSettings(instance);

        instance.transform.position = StartPoint;
        instance.gameObject.SetActive(true);
        yield return null; // It avoids the position carry on from last frame if it is used again.

        instance.emitting = true;

        float distance = Vector3.Distance(StartPoint, EndPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            instance.transform.position = Vector3.Lerp(StartPoint, EndPoint, Mathf.Clamp01(1 - (remainingDistance / distance)));
            remainingDistance -= simulationSpeed * Time.deltaTime;

            yield return null;
        }

        instance.transform.position = EndPoint;

        if (hit.collider != null)
        {
            HandleBulletImpact(distance, EndPoint, hit.normal, hit.collider);
        }

        yield return new WaitForSeconds(trailDuration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        WorldObjectPoolManager.instance.bulletTrailPool.Release(instance);
    }

    #endregion

    #region Damage Functions

    /// <summary>
    /// Hadles bullet collision, such as damagine the player, gaining exp points, and showing damage text pop up
    /// </summary>
    /// <param name="DistanceTraveled"></param>
    /// <param name="HitLocation"></param>
    /// <param name="HitNormal"></param>
    /// <param name="HitCollider"></param>
    private void HandleBulletImpact(float DistanceTraveled, Vector3 HitLocation, Vector3 HitNormal, Collider HitCollider)
    {
        if (HitCollider.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            Player player = activeMonobehaviour.gameObject.GetComponent<Player>();

            if(player != null)
            {
                player.SetAbilityPoint(5);
            }

            WorldObjectPoolManager.instance.GetDamagePopUp(1f, HitLocation, GetDamage(DistanceTraveled));
            damageable.TakeDamage(GetDamage(DistanceTraveled));
        }
    }

    public int GetDamage(float Distance = 0)
    {
        return Mathf.CeilToInt(damageCurve.Evaluate(Distance, Random.value)) + upgradeDamage;
    }

    #endregion

    #region Other Functions

    /// <summary>
    /// Randomizing bullet spread while firing
    /// </summary>
    /// <param name="shootTime"></param>
    /// <returns></returns>
    public Vector3 GetSpread(float shootTime = 0)
    {
        Vector3 spread = Vector3.zero;
        spread = new Vector3(Random.Range(-shootSpread.x, shootSpread.x), Random.Range(-shootSpread.y, shootSpread.y), Random.Range(-shootSpread.z, shootSpread.z));
        return spread;
    }

    /// <summary>
    /// Setting up trail visuals
    /// </summary>
    /// <param name="trail"></param>
    public void SetTrailSettings(TrailRenderer trail)
    {
        trail.colorGradient = trailColor;
        trail.material = trailMaterial;
        trail.widthCurve = trailWidthCurve;
        trail.time = trailDuration;
        trail.minVertexDistance = trailMinVertexDistance;
    }

    #endregion

    #region Audio Functions

    /// <summary>
    /// Function that handles playing shoot audio
    /// </summary>
    /// <param name="audioSource"></param>
    /// <param name="audioClip"></param>
    private void PlayShootAudio(AudioSource audioSource, AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    #endregion

    /// <summary>
    /// Check and upgrades weapon
    /// </summary>
    public void UpgradeWeapon()
    {
        if(currentLevel <= 5)
        {
            currentLevel += 1;
        }
        
        switch (currentLevel)
        {
            case 1:
                upgradeFirerate -= 0.05f;
                break;

            case 2:
                upgradeFirerate -= 0.05f;
                break;

            case 3:
                upgradeMissDistance = 20;
                break;

            case 4:
                upgradeMissDistance = 20;
                break;

            case 5:
                upgradeDamage += 10;
                upgradeBulletsPerShot += 1;
                break;

            case 6:
                upgradeFirerate = 0.05f;
                break;

            case 7:
                upgradeDamage += 10;
                break;

            case 8:
                upgradeDamage += 10;
                break;

            case 9:
                upgradeDamage += 10;
                break;

            case 10:
                upgradeBulletsPerShot += 1;
                break;
        }

    }
}
