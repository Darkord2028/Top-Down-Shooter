using System.Collections;
using UnityEngine;
using static UnityEngine.ParticleSystem;

[CreateAssetMenu(fileName = "newWeaponItem", menuName = "Data/Items/Weapon Item")]
public class WeaponScriptableObject : ItemScriptableObject
{
    /// <summary>
    /// This scriptable object inherts from base ItemScriptableObject and extends it to become a weapon class of its own.
    /// </summary>

    #region Weapon Information

    [Header("Weapon Information")]
    public bool isRightHandedWeapon;
    public bool isHitscanWeapon;

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

    #endregion

    #region Initialize

    public void Initialize(MonoBehaviour monoBehaviour, ParticleSystem particleSystem, AudioSource audioSource)
    {
        activeMonobehaviour = monoBehaviour;
        shootParticleSystem = particleSystem;
        shootAudioSource = audioSource;
    }

    #endregion

    #region Shoot Functions

    public void Shoot()
    {
        for (int i = 0; i < bulletsPerShot; i++)
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
                activeMonobehaviour.StartCoroutine(PlayTrail(shootParticleSystem.transform.position, shootParticleSystem.transform.position + (shootDirection * trailMissDistance), new RaycastHit()));
            }
        }
    }

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

        yield return new WaitForSeconds(trailDuration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        WorldObjectPoolManager.instance.bulletTrailPool.Release(instance);
    }

    #endregion

    #region Other Functions

    public Vector3 GetSpread(float shootTime = 0)
    {
        Vector3 spread = Vector3.zero;
        spread = new Vector3(Random.Range(-shootSpread.x, shootSpread.x), Random.Range(-shootSpread.y, shootSpread.y), Random.Range(-shootSpread.z, shootSpread.z));
        return spread;
    }

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

    private void PlayShootAudio(AudioSource audioSource, AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    #endregion

}
