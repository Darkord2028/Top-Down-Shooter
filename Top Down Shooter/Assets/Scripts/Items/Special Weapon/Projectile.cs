using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// This script applies force to the special weapon projectiles in the facing direction
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    private Rigidbody rigidBody;

    [SerializeField] float projectileDisableTime;
    [SerializeField] public Vector3 SpawnLocation { get; private set; }

    public delegate void CollisionEvent(Projectile Projectile, Collision collision);
    public event CollisionEvent OnCollision;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// As projectile spawns from the pool, apply spawnForce.
    /// </summary>
    /// <param name="spawnForce"></param>
    /// <param name="projectilePool"></param>
    public void Spawn(Vector3 spawnForce, ObjectPool<Projectile> projectilePool)
    {
        SpawnLocation = transform.position;
        transform.forward = spawnForce.normalized;
        rigidBody.AddForce(spawnForce);
        StartCoroutine(ProjectileDisableTime(projectileDisableTime, projectilePool));
    }

    /// <summary>
    /// Releasing the projectile to the object pool after delay and on collision
    /// </summary>
    /// <param name="time"></param>
    /// <param name="projectilePool"></param>
    /// <returns></returns>
    private IEnumerator ProjectileDisableTime(float time, ObjectPool<Projectile> projectilePool)
    {
        yield return new WaitForSeconds(time);
        OnCollisionEnter(null);
        yield return null;

        projectilePool.Release(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnCollision?.Invoke(this, collision);
    }

    /// <summary>
    /// As projectile is returned to pool, all coroutines are disabled and velocity is set back to zero.
    /// </summary>
    private void OnDisable()
    {
        StopAllCoroutines();
        rigidBody.linearVelocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        OnCollision = null;
    }

}
