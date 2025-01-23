using UnityEngine;

[DisallowMultipleComponent]
public class Health : MonoBehaviour, IDamageable
{
    [SerializeField]
    private int _MaxHealth = 100;
    [SerializeField]
    private int _Health;

    public int CurrentHealth { get => _Health; private set => _Health = value; }

    public int MaxHealth { get => _MaxHealth; private set => _MaxHealth = value; }

    public event IDamageable.TakeDamageEvent OnTakeDamage;
    public event IDamageable.DeathEvent OnDeath;

    private void OnEnable()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(int damage)
    {
        int DamageTaken = Mathf.Clamp(damage, 0, CurrentHealth);

        CurrentHealth -= DamageTaken;

        if (DamageTaken != 0)
        {
            OnTakeDamage?.Invoke(DamageTaken);
        }

        if (CurrentHealth == 0 && DamageTaken != 0)
        {
            OnDeath?.Invoke(transform.position);
        }

    }
}
