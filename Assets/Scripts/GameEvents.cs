using System;

// Central event channel for collisions. Hazards and pickups only RAISE these events;
// they never touch the player or its health directly. PlayerHealth listens and reacts.
public static class GameEvents
{
    // float = share of max health to remove (0.25 = a quarter)
    public static event Action<float> ObstacleHit;

    // float = share of max health to restore
    public static event Action<float> PickupCollected;

    public static void RaiseObstacleHit(float damageFraction) => ObstacleHit?.Invoke(damageFraction);
    public static void RaisePickupCollected(float healFraction) => PickupCollected?.Invoke(healFraction);
}
