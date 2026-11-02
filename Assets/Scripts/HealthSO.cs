using System;
using UnityEngine;

// Health stored in a ScriptableObject.
[CreateAssetMenu(fileName = "PlayerHealth", menuName = "DodgeRun/Health")]
public class HealthSO : ScriptableObject
{
    [SerializeField] private float maxHealth = 100f;

    [NonSerialized] private float currentHealth;

    public event Action<float, float> HealthChanged;
    public event Action Depleted;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public float Fraction => maxHealth <= 0f ? 0f : currentHealth / maxHealth;
    public bool IsDepleted => currentHealth <= 0f;

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
