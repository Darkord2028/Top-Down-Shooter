using UnityEngine;

/// <summary>
/// Base class that is required for enemy object pooling.
/// Also handles death when health drops to zero
/// </summary>
[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour
{
    Health health;
    [SerializeField] EnemyType enemyType;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        health.OnDeath += ReturnToPool;
    }

    private void OnDisable()
    {
        health.OnDeath -= ReturnToPool;
    }

    private void ReturnToPool(Vector3 position)
    {
        WorldObjectPoolManager.instance.ReturnEnemyToPool(enemyType, gameObject);
    }

}

/// <summary>
/// Different types of enemies in the game
/// </summary>
public enum EnemyType
{
    None,
    Cyborg,
    Turret,
    Boss
}
