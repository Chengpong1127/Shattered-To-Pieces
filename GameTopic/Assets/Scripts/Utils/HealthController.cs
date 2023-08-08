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

    /// <summary>
    /// The max health of the health controller.
    /// </summary>
    public T MaxHealth { get; private set; }
    /// <summary>
    /// The min health of the health controller. Default is 0.
    /// </summary>
    public T MinHealth { get; private set; } = (dynamic)0;
    /// <summary>
    /// The current health of the health controller.
    /// </summary>
    public T CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth.Equals(MinHealth);

    /// <summary>
    /// Set the current health of the health controller. If the current health is less than min health or greater than max health, an ArgumentException will be thrown.
    /// </summary>
    /// <param name="currentHealth"> The current health to set. </param>
    public void SetCurrentHealth(T currentHealth) {
        if((dynamic)currentHealth < MinHealth || (dynamic)currentHealth > MaxHealth) {
            throw new ArgumentException("Current health cannot be less than min health or greater than max health.");
        }
        CurrentHealth = currentHealth;
    }
    /// <summary>
    /// Reset the current health of the health controller to max health.
    /// </summary>
    public void ResetToMaxHealth() {
        CurrentHealth = MaxHealth;
    }

    /// <summary>
    /// Take damage to the health controller. If the health controller is dead, OnDied will be invoked.
    /// </summary>
    /// <param name="damage"> The damage to take. </param>
    public void TakeDamage(T damage) {
        if(damage < (dynamic)0) {
            throw new ArgumentException("Damage cannot be negative.");
        }
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
        if(heal < (dynamic)0) {
            throw new ArgumentException("Heal cannot be negative.");
        }
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