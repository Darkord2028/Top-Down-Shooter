using UnityEngine;

/// <summary>
/// This is a base class for hit boxes that finds and apply damage to gameobject with tag == hitTag
/// </summary>
public class HurtBox : MonoBehaviour
{
    [SerializeField] ParticleSystem.MinMaxCurve damageCurve;
    [SerializeField] bool isZombieHurtBox;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(other.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                damageable.TakeDamage(GetDamage());

                if(isZombieHurtBox)
                {
                    WorldObjectPoolManager.instance.zombiePool.Release(transform.root.gameObject);
                    transform.root.gameObject.SetActive(false);
                }

            }
        }
    }

    public int GetDamage(float Distance = 0)
    {
        return Mathf.CeilToInt(damageCurve.Evaluate(Distance, Random.value));
    }

}
