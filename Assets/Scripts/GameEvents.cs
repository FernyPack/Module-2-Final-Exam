using System;

// Collision events. Hazards and pickups raise them, PlayerHealth listens.
public static class GameEvents
{
    public static event Action<float> ObstacleHit;

    public static event Action<float> PickupCollected;

    public static void RaiseObstacleHit(float damageFraction) => ObstacleHit?.Invoke(damageFraction);
    public static void RaisePickupCollected(float healFraction) => PickupCollected?.Invoke(healFraction);
}
