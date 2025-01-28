using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// This static class handles object pooling by using UnityEngine.Pool
/// </summary>
public class WorldObjectPoolManager : MonoBehaviour
{
    public Player player;
    public static WorldObjectPoolManager instance = null;

    [Header("Damage Pop Up")]
    [SerializeField] TextMeshPro damageTextMesh;
    [SerializeField] Vector3 popUpSpread;

    [Header("Gun Drop")]
    [SerializeField] GameObject[] dropGuns;

    //Enemy Prefabs
    [Header("Enemy Prefabs")]
    [SerializeField] GameObject cyborgEnemyPrefab;
    [SerializeField] GameObject turretEnemyPrefab;
    [SerializeField] GameObject bossEnemyPrefab;

    [Header("Spawn References")]
    [SerializeField] Collider targetCollider;
    [SerializeField] LayerMask groundLayermask;

    [Header("Spawn Wave Properties")]
    [SerializeField] [Tooltip("After initial spawn, time before the next spawn")] float spawnInterval;
    [SerializeField] [Tooltip("Indexes are wave numbers while the values are spawn count")] int[] turrentSpawnCount;
    [SerializeField] [Tooltip("Indexes are wave numbers while the values are spawn count")] int[] cyborgSpawnCount;
    public int[] bossSpawnCount;

    //Object Pooling
    public ObjectPool<TextMeshPro> damagePopUpPool { get; private set; }
    public ObjectPool<TrailRenderer> bulletTrailPool { get; private set; }
    public ObjectPool<GameObject> cyborgEnemyPool { get; private set; }
    public ObjectPool<GameObject> turretPool { get; private set; }
    public ObjectPool<GameObject> bossPool { get; private set; }

    //Pool Holder
    private GameObject damagePopUpHolder;
    private GameObject bulletTrailHolder;
    private GameObject cyborgHolder;
    private GameObject turretHolder;
    private GameObject bossHolder;

    private float lastSpawnTime;

    #region Unity Callback Funtions

    /// <summary>
    /// Creating object pool references and object pool holders
    /// </summary>
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if (bulletTrailHolder != this)
        {
            Destroy(instance);
        }

        damagePopUpPool = new ObjectPool<TextMeshPro>(CreateDamagePopUp);
        damagePopUpHolder = new GameObject("Damage Pop Up Pool");

        bulletTrailPool = new ObjectPool<TrailRenderer>(CreateTrail);
        bulletTrailHolder = new GameObject("Bullet Trail Pool");

        cyborgEnemyPool = new ObjectPool<GameObject>(CreateBasicEnemy);
        cyborgHolder = new GameObject("Basic Enemy Pool");

        turretPool = new ObjectPool<GameObject>(CreateTurret);
        turretHolder = new GameObject("Turret Pool Holder");

        bossPool = new ObjectPool<GameObject>(CreateBoss);
        bossHolder = new GameObject("Boss Pool Holder");
    }

    /// <summary>
    /// Deploying enemies at the start
    /// </summary>
    private void Start()
    {
        DeployEnemies(EnemyType.Turret, 2, 10);
        DeployEnemies(EnemyType.Cyborg, 3, 5);
        DropGun();
    }

    /// <summary>
    /// Deploying enemies in fixed intervals
    /// </summary>
    private void Update()
    {
        if(Time.time > spawnInterval + lastSpawnTime)
        {
            lastSpawnTime = Time.time;
            int currentWave = WorldUIManager.instance.currentWave;
            DeployEnemies(EnemyType.Cyborg, 2, cyborgSpawnCount[currentWave]);
            DeployEnemies(EnemyType.Turret, 4, turrentSpawnCount[currentWave]);
            DropGun();
        }
    }

    #endregion

    #region Object Pool Create Functions

    // These functions instantiates and stores them in object pool

    private TextMeshPro CreateDamagePopUp()
    {
        TextMeshPro instance = Instantiate(damageTextMesh);
        instance.transform.SetParent(damagePopUpHolder.transform, false);
        instance.gameObject.SetActive(false);
        return instance;
    }

    private TrailRenderer CreateTrail()
    {
        GameObject instance = new GameObject("Bullet Trail");
        instance.transform.SetParent(bulletTrailHolder.transform, true);
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        
        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }

    private GameObject CreateBasicEnemy()
    {
        GameObject instance = Instantiate(cyborgEnemyPrefab);
        instance.transform.SetParent(cyborgHolder.transform, false);
        return instance;
    }

    private GameObject CreateTurret()
    {
        GameObject instance = Instantiate(turretEnemyPrefab);
        instance.transform.SetParent(turretHolder.transform, false);
        return instance;
    }

    private GameObject CreateBoss()
    {
        GameObject instance = Instantiate(bossEnemyPrefab);
        instance.transform.SetParent(bossHolder.transform, false);
        return instance;
    }

    #endregion

    #region Deploy Enemy Functions

    /// <summary>
    /// Deploying numberOfEnemies from enemyPool after delay
    /// </summary>
    /// <param name="delay"></param>
    /// <param name="numberOfEnemies"></param>
    /// <param name="enemyPool"></param>
    /// <returns></returns>
    private IEnumerator Deploy(float delay, int numberOfEnemies, ObjectPool<GameObject> enemyPool)
    {
        yield return new WaitForSeconds(delay);

        for (int i = 0; i < numberOfEnemies; i++)
        {
            if (Physics.Raycast(GetRandomPointInCollider(targetCollider), Vector3.down, out RaycastHit hit, float.MaxValue, groundLayermask))
            {
                GameObject instance = enemyPool.Get();
                CharacterController character = instance.GetComponent<CharacterController>();

                if (character)
                {
                    character.enabled = false;
                    instance.transform.position = hit.point;
                    character.enabled = true;
                    instance.gameObject.SetActive(true);
                }
                else
                {
                    instance.transform.position = hit.point;
                    instance.gameObject.SetActive(true);
                }   
            }
        }
    }

    /// <summary>
    /// Check the type of enemy and gets the enemy pool
    /// </summary>
    /// <param name="enemyType"></param>
    /// <param name="deployDelay"></param>
    /// <param name="numberOfEnemies"></param>
    public void DeployEnemies(EnemyType enemyType, float deployDelay, int numberOfEnemies)
    {
        ObjectPool<GameObject> enemyPool = null;

        switch (enemyType)
        {
            case EnemyType.Cyborg:
                enemyPool = cyborgEnemyPool;
                break;

            case EnemyType.Turret:
                enemyPool = turretPool;
                break;

            case EnemyType.Boss:
                enemyPool = bossPool;
                break;
        }

        if(enemyPool != null)
        {
            StartCoroutine(Deploy(deployDelay, numberOfEnemies, enemyPool));
        }

    }

    /// <summary>
    /// Function that return enemy to pool
    /// </summary>
    /// <param name="enemyType"></param>
    /// <param name="enemy"></param>
    public void ReturnEnemyToPool(EnemyType enemyType, GameObject enemy)
    {
        switch (enemyType)
        {
            case EnemyType.None:
                break;

            case EnemyType.Turret:
                enemy.gameObject.SetActive(false);
                turretPool.Release(enemy);
                break;

            case EnemyType.Cyborg:
                enemy.gameObject.SetActive(false);
                cyborgEnemyPool.Release(enemy);
                break;

            case EnemyType.Boss:
                enemy.gameObject.SetActive(false);
                bossPool.Release(enemy);
                break;
        }
    }

    #endregion

    #region Damage Pop Up Functions

    /// <summary>
    /// This functions get damage text pop up and randomizes the position.
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="position"></param>
    /// <param name="DamageTaken"></param>
    public void GetDamagePopUp(float duration, Vector3 position, float DamageTaken)
    {
        TextMeshPro damageText = damagePopUpPool.Get();

        float PosX = popUpSpread.x;
        Vector3 popUpPosition = new Vector3(position.x +PosX, 2.5f, position.z);

        damageText.transform.position = popUpPosition;
        damageText.text = DamageTaken.ToString();
        damageText.gameObject.SetActive(true);
        StartCoroutine(DisableDamagePopUp(1f, damageText));
    }

    /// <summary>
    /// This function disables and returns damage text pop up to pool
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="damageText"></param>
    /// <returns></returns>
    private IEnumerator DisableDamagePopUp(float duration, TextMeshPro damageText)
    {
        yield return new WaitForSeconds(duration);
        damageText.gameObject.SetActive(false);
        damagePopUpPool.Release(damageText);
    }

    #endregion

    #region Gun Drop Functions

    /// <summary>
    /// This function shuffles the list of dropGun and set the position of first gun to a random point.
    /// </summary>

    private void DropGun()
    {
        for (int i = 0; i < dropGuns.Length; i++)
        {
            dropGuns[i].gameObject.SetActive(false);
        }

        Shuffle(dropGuns);
        Vector3 dropPosition = GetRandomPointInCollider(targetCollider);
        dropGuns[0].transform.position = new Vector3(dropPosition.x, 1, dropPosition.z);
        dropGuns[0].gameObject.SetActive(true);
    }

    #endregion

    #region Other Functions

    /// <summary>
    /// This function returns a random point in collider
    /// </summary>
    /// <param name="collider"></param>
    /// <returns></returns>
    Vector3 GetRandomPointInCollider(Collider collider)
    {
        Vector3 point;
        Bounds bounds = collider.bounds;

        do
        {
            // Generate a random point inside the collider's bounds
            point = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }
        while (!IsPointInsideCollider(collider, point)); // Ensure the point is inside the collider

        return point;
    }

    bool IsPointInsideCollider(Collider collider, Vector3 point)
    {
        // Check if the point is inside the collider
        return Physics.OverlapSphere(point, 1f, 1 << collider.gameObject.layer).Length > 0;
    }

    /// <summary>
    /// This function shuffles the contents of the list.
    /// </summary>
    /// <param name="array"></param>
    private void Shuffle(GameObject[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1); // Get a random index in the range [0, i]

            // Swap elements at i and randomIndex
            GameObject temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    #endregion

}
