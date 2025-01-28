using UnityEngine;

/// <summary>
/// Base health class for all characters in the game
/// </summary>

[DisallowMultipleComponent]
public class Health : MonoBehaviour, IDamageable
{
    public int _MaxHealth = 100;
    public int _Health;

    public bool canDamage = true;

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

        if (!canDamage)
        {
            DamageTaken = 0;
        }

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

    public void Heal(int HealAmount)
    {
        CurrentHealth += HealAmount;
        CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);
        WorldUIManager.instance.IncreasePlayerHealth(HealAmount);
    }

    public void HealthUpgrade(int upgradeAmount)
    {
        MaxHealth += upgradeAmount;
        CurrentHealth += upgradeAmount;
        WorldUIManager.instance.InitializePlayerHealth(MaxHealth, CurrentHealth);
    }

}
