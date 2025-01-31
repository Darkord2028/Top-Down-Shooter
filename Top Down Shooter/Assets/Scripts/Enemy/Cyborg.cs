using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Base script for enemies who shoots
/// </summary>
public class Cyborg : MonoBehaviour
{
    [SerializeField] bool isMovingEnemy;
    [SerializeField] bool useNavMesh;
    [SerializeField] bool canShoot;

    [Header("Cyborg info")]
    [SerializeField] float moveSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float stoppingDistance;
    [SerializeField] float gravity;

    [Header("Shoot Config")]
    [SerializeField] float firerate;
    [SerializeField] Vector3 shootSpread;
    [SerializeField] Transform[] firepointTransform;
    [SerializeField] LayerMask hitMask;

    [Header("Trail Config")]
    [SerializeField] Material trailMaterial;
    [SerializeField] Gradient trailColor;
    [SerializeField] AnimationCurve trailWidthCurve;
    [SerializeField] float trailDuration;
    [SerializeField] float trailMinVertexDistance;
    [SerializeField] float trailSimulationSpeed;
    [SerializeField] float trailMissDistance;

    [Header("Damage Config")]
    [SerializeField] ParticleSystem.MinMaxCurve damageCurve;

    private Player player;
    public CharacterController characterController { get; private set; }
    private NavMeshAgent Agent;
    private Animator animator;

    private float CurrentFirerate;
    [SerializeField] Vector3 velocity;

    #region Unity Callback Functions

    /// <summary>
    /// Storing up the references for moving player as well as non moving spheres
    /// </summary>
    private void Start()
    {
        player = WorldObjectPoolManager.instance.player;
        animator = GetComponent<Animator>();

        if (isMovingEnemy)
        {
            characterController = GetComponent<CharacterController>();
        }
        if(useNavMesh)
        {
            Agent = GetComponent<NavMeshAgent>();
            Agent.speed = moveSpeed;
            Agent.stoppingDistance = stoppingDistance;
        }
    }

    /// <summary>
    /// Checking if enemies are close to player.
    /// If not then move towards player.
    /// Also handling shooting for non moving player.
    /// </summary>
    private void Update()
    {
        if (isMovingEnemy)
        {
            if(useNavMesh)
            {
                SetEnemyMovement(Agent);
                animator.SetFloat("moveX", Agent.velocity.x);
            }
            else
            {
                animator.SetFloat("moveX", characterController.velocity.x);

                // Direction and distance to player
                Vector3 direction = (player.transform.position - transform.position);
                float distanceToPlayer = direction.magnitude;

                if (characterController.velocity.magnitude < 0.4f && distanceToPlayer <= stoppingDistance + 0.1f)
                {
                    if (Time.time > firerate + CurrentFirerate)
                    {
                        CurrentFirerate = Time.time;
                        Shoot();
                    }
                }
                if (!characterController.isGrounded)
                {
                    velocity.y += gravity * Time.deltaTime;
                }
                else
                {
                    velocity.y = 0;
                }
            }

        }
        else
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer < stoppingDistance)
            {
                if(Time.time > firerate + CurrentFirerate)
                {
                    CurrentFirerate = Time.time;
                    Shoot();
                }
            }
        }
    }

    #endregion

    #region Shoot Functions

    /// <summary>
    /// Base shooting function using bullet trail pool.
    /// </summary>
    private void Shoot()
    {
        for (int i = 0; i < firepointTransform.Length; i++)
        {
            Vector3 spreadAmount = GetSpread();
            Vector3 shootDirection;
            shootDirection = firepointTransform[i].transform.forward + spreadAmount;
            shootDirection.Normalize();

            if (Physics.Raycast(firepointTransform[i].transform.position, shootDirection, out RaycastHit hit, float.MaxValue, hitMask))
            {
                StartCoroutine(PlayTrail(firepointTransform[i].transform.position, hit.point, hit));
            }
            else
            {
                StartCoroutine(PlayTrail(firepointTransform[i].transform.position, firepointTransform[i].transform.position + (shootDirection * (trailMissDistance)), new RaycastHit()));
            }
        }
    }


    /// <summary>
    /// Playing trail pool from start to hit position
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
            remainingDistance -= trailSimulationSpeed * Time.deltaTime;

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

    /// <summary>
    /// Handling Damage text pop up as well as damaging enemies after bullet imapcts.
    /// </summary>
    /// <param name="DistanceTraveled"></param>
    /// <param name="HitLocation"></param>
    /// <param name="HitNormal"></param>
    /// <param name="HitCollider"></param>
    private void HandleBulletImpact(float DistanceTraveled, Vector3 HitLocation, Vector3 HitNormal, Collider HitCollider)
    {
        if (HitCollider.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            DropDamagePopUp(GetDamage(DistanceTraveled), HitLocation);
            damageable.TakeDamage(GetDamage(DistanceTraveled));
        }
    }

    #endregion

    #region Damage Functions

    /// <summary>
    /// Position offset for better visuals of damage text
    /// </summary>
    /// <param name="DamageTaken"></param>
    /// <param name="position"></param>
    private void DropDamagePopUp(float DamageTaken, Vector3 position)
    {
        position.y = 2.5f;
        position.x += Random.Range(-0.01f, 0.01f);

        WorldObjectPoolManager.instance.GetDamagePopUp(1f, position, DamageTaken);
    }

    /// <summary>
    /// Damage evaluation from ParticleSystem.MinMaxCuve.
    /// </summary>
    /// <param name="Distance"></param>
    /// <returns></returns>
    public int GetDamage(float Distance = 0)
    {
        return Mathf.CeilToInt(damageCurve.Evaluate(Distance, Random.value));
    }

    #endregion

    #region Other Functions

    /// <summary>
    /// Setting up trail visuals for bullet
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

    /// <summary>
    /// Randomizing the bullet spread.
    /// </summary>
    /// <param name="shootTime"></param>
    /// <returns></returns>
    public Vector3 GetSpread(float shootTime = 0)
    {
        Vector3 spread = Vector3.zero;
        spread = new Vector3(Random.Range(-shootSpread.x, shootSpread.x), Random.Range(-shootSpread.y, shootSpread.y), Random.Range(-shootSpread.z, shootSpread.z));
        return spread;
    }

    #endregion

    #region Move Functions

    /// <summary>
    /// Function for handling and roation towards player
    /// </summary>
    void MoveAndRotateTowardsPlayer()
    {
        if (player == null) return;

        // Direction and distance to player
        Vector3 direction = (player.transform.position - transform.position);
        float distanceToPlayer = direction.magnitude;

        // Stop movement if within stopping distance
        if (distanceToPlayer <= stoppingDistance + 0.1f)
        {
            velocity = Vector3.zero;
            return;
        }

        direction.Normalize();

        // Rotate smoothly towards the player
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        // Dampen speed as the enemy approaches stopping distance
        float speedFactor = Mathf.Clamp01((distanceToPlayer - stoppingDistance) / stoppingDistance);
        Vector3 move = direction * moveSpeed * speedFactor;

        // Move the enemy
        characterController.Move((move + velocity) * Time.deltaTime);
    }

    #endregion

    #region NavMesh Functions

    private void SetEnemyMovement(NavMeshAgent agent)
    {
        agent.SetDestination(player.transform.position);

        if(agent.velocity.magnitude < 0.2f && canShoot)
        {
            if (Time.time > firerate + CurrentFirerate)
            {
                CurrentFirerate = Time.time;
                Shoot();
            }
        }
    }

    #endregion

}
