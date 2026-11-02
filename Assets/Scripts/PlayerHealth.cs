using UnityEngine;

// Applies collision events to the Health ScriptableObject.
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private HealthSO health;

    void Awake()
    {
        health.ResetHealth();
    }

    void OnEnable()
    {
        GameEvents.ObstacleHit += HandleObstacleHit;
        GameEvents.PickupCollected += HandlePickupCollected;
    }

    void OnDisable()
    {
        GameEvents.ObstacleHit -= HandleObstacleHit;
        GameEvents.PickupCollected -= HandlePickupCollected;
    }

    void HandleObstacleHit(float damageFraction)
    {
        health.TakeDamage(health.MaxHealth * damageFraction);
    }

    void HandlePickupCollected(float healFraction)
    {
        health.Heal(health.MaxHealth * healFraction);
    }
}
