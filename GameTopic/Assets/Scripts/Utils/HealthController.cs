using System;

/// <summary>
/// Health controller which can manage the health of every thing that has health.
/// </summary>
/// <typeparam name="T"> The type of the health, generally is int or float. </typeparam>
public class HealthController<T>{
    
    public HealthController(T maxHealth) {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
    }

    public HealthController(T maxHealth, T currentHealth) {
        MaxHealth = maxHealth;
        CurrentHealth = currentHealth;
    }

    public HealthController(T maxHealth, T currentHealth, T minHealth) {
        MaxHealth = maxHealth;
        CurrentHealth = currentHealth;
        MinHealth = minHealth;
    }

    /// <summary>
    /// Event that will be invoked when the health controller takes damage.
    /// </summary>
    public event Action<T> OnTakenDamage;
    /// <summary>
    /// Event that will be invoked when the health controller takes heal.
    /// </summary>
    public event Action<T> OnTakenHeal;

    /// <summary>
    /// Event that will be invoked when the health controller is died.
    /// </summary>
    public event Action OnDied;
    /// <summary>
    /// Event that will be invoked when the health controller is healed full.
    /// </summary>
    public event Action OnHealedFull;


    public T MaxHealth { get; private set; }
    public T MinHealth { get; private set; } = (dynamic)0;
    public T CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth.Equals(MinHealth);

    /// <summary>
    /// Take damage to the health controller. If the health controller is dead, OnDied will be invoked.
    /// </summary>
    /// <param name="damage"> The damage to take. </param>
    public void TakeDamage(T damage) {
        CurrentHealth = (dynamic)CurrentHealth - damage;
        OnTakenDamage?.Invoke(damage);
        if((dynamic)CurrentHealth <= MinHealth) {
            CurrentHealth = MinHealth;
            OnDied?.Invoke();
        }
    }
    /// <summary>
    /// Take heal to the health controller. If the health controller is full, OnHealedFull will be invoked.
    /// </summary>
    /// <param name="heal"></param>
    public void TakeHeal(T heal) {
        CurrentHealth = (dynamic)CurrentHealth + heal;
        OnTakenHeal?.Invoke(heal);
        if((dynamic)CurrentHealth >= MaxHealth) {
            CurrentHealth = MaxHealth;
            OnHealedFull?.Invoke();
        }
    }
    /// <summary>
    /// Get the health percentage of the health controller.
    /// </summary>
    /// <returns> The health percentage. </returns>
    public double GetHealthPercentage() {
        return (dynamic)CurrentHealth / (double)(dynamic)MaxHealth;
    }
}