using UnityEngine;

// Connects the collision events to the Health ScriptableObject.
// Sits on the Player. No other script calls into this one - it only listens.
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private HealthSO health;

    void Awake()
    {
        health.ResetHealth();   // fresh, full bar at the start of every run
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
