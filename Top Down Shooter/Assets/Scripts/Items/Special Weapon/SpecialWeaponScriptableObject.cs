using UnityEngine;
using UnityEngine.Pool;


/// <summary>
/// This Scriptable object inherites from ItemScriptableObject and stroes information regarding special weapons
/// </summary>
[CreateAssetMenu(fileName = "newSpecialWeaponItem", menuName = "Data/Items/Special Weapon Item")]
public class SpecialWeaponScriptableObject : ItemScriptableObject
{
    [Header("Weapon Information")]
    public int MaxLevel;
    public WeaponProjectileType ProjectileType;

    [Header("Collision Event")]
    public bool disableOnCollision;

    [Header("Weapon Upgrade")]
    [SerializeField] int initialWeaponLevel;

    [Header("Weapon Shoot Config")]
    public Projectile projectile; 
    public float firerate;
    public float projectileSpawnForce;
    public Vector3 shootSpread;

    [Header("Weapon Damage Config")]
    public ParticleSystem.MinMaxCurve damageCurve;

    [SerializeField] Transform[] firepoints;

    private ObjectPool<Projectile> projectilePool;
    private GameObject projectileHolder;

    private float lastShootTime;
    public int currentLevel { get; private set; }

    private Player player;

    /// <summary>
    /// Initializing the weapon by setting up references to firepoint and player as well as creating the object pool
    /// </summary>
    /// <param name="firepoints"></param>
    /// <param name="player"></param>
    public void Initialize(Transform[] firepoints, Player player)
    {
        this.firepoints = firepoints;
        this.player = player;

        projectilePool = new ObjectPool<Projectile>(CreateProjectile);
        projectileHolder = new GameObject("Projectile Holder");

        currentLevel = initialWeaponLevel;
        lastShootTime = 0;
    }

    /// <summary>
    /// This function handles the shooting of special weapon
    /// </summary>
    public void Shoot()
    {
        if (Time.time > lastShootTime + firerate)
        {
            lastShootTime = Time.time;

            for (int i = 0; i < firepoints.Length; i++)
            {
                if(i == currentLevel) break;

                Vector3 spreadAmount = GetSpread();
                Vector3 shootDirection;
                
                Projectile projectile = projectilePool.Get();
                projectile.transform.SetParent(projectileHolder.transform, false);
                projectile.gameObject.SetActive(true);
                projectile.OnCollision += HandleBulletCollision;
                projectile.transform.position = firepoints[i].transform.position;
                projectile.transform.rotation = firepoints[i].transform.rotation;

                shootDirection = firepoints[i].transform.forward + spreadAmount;
                shootDirection.Normalize();
                projectile.Spawn(shootDirection * projectileSpawnForce, projectilePool);

            }
        }
    }

    /// <summary>
    /// Updating the Level of special weapon
    /// </summary>
    public void UpdateWeapon()
    {
        if(currentLevel < MaxLevel)
        {
            currentLevel += 1;
        }
    }

    /// <summary>
    /// Gaining exp from hitting enemies, showing damage pop up text
    /// Damaging enemies
    /// </summary>
    /// <param name="projectile"></param>
    /// <param name="collision"></param>
    private void HandleBulletCollision(Projectile projectile, Collision collision)
    {
        projectile.gameObject.SetActive(false);
        projectilePool.Release(projectile);

        if (collision != null)
        {
            if(collision.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                player.SetAbilityPoint(5);
                damageable.TakeDamage(GetDamage());
                WorldObjectPoolManager.instance.GetDamagePopUp(1f, collision.transform.position, GetDamage());
            }
        }

    }

    public int GetDamage(float Distance = 0)
    {
        return Mathf.CeilToInt(damageCurve.Evaluate(Distance, Random.value));
    }

    public Vector3 GetSpread(float shootTime = 0)
    {
        Vector3 spread = Vector3.zero;
        spread = new Vector3(Random.Range(-shootSpread.x, shootSpread.x), Random.Range(-shootSpread.y, shootSpread.y), Random.Range(-shootSpread.z, shootSpread.z));
        return spread;
    }

    private Projectile CreateProjectile()
    {
        return Instantiate(projectile);
    }

}
