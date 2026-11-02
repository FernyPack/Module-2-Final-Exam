using System;
using UnityEngine;

// ScriptableObject-based health. The health value lives in this asset, not on the player.
// Anything that cares about health (the health bar, the GameManager) listens to its events
// instead of being called directly.
[CreateAssetMenu(fileName = "PlayerHealth", menuName = "DodgeRun/Health")]
public class HealthSO : ScriptableObject
{
    [SerializeField] private float maxHealth = 100f;

    // NonSerialized so play-mode damage never gets saved into the asset file.
    [NonSerialized] private float currentHealth;

    // (current, max) - raised every time health changes
    public event Action<float, float> HealthChanged;
    // raised once when health hits 0
    public event Action Depleted;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public float Fraction => maxHealth <= 0f ? 0f : currentHealth / maxHealth;
    public bool IsDepleted => currentHealth <= 0f;

    // ScriptableObjects keep their values between scene loads, so the player resets this on every run.
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (IsDepleted || amount <= 0f) return;

        currentHealth = Mathf.Max(0f, currentHealth - amount);
        HealthChanged?.Invoke(currentHealth, maxHealth);

        if (IsDepleted)
            Depleted?.Invoke();
    }

    public void Heal(float amount)
    {
        if (IsDepleted || amount <= 0f) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
