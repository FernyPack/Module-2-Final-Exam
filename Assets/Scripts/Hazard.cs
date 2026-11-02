using UnityEngine;

// Obstacle that damages the player through an event.
public class Hazard : MonoBehaviour
{
    [Range(0f, 1f)] public float damageFraction = 0.25f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameEvents.RaiseObstacleHit(damageFraction);
        Destroy(gameObject);
    }
}
