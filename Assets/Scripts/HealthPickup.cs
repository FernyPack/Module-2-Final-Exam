using UnityEngine;

// Heals the player through an event.
public class HealthPickup : MonoBehaviour
{
    [Range(0f, 1f)] public float healFraction = 0.25f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameEvents.RaisePickupCollected(healFraction);
        Destroy(gameObject);
    }
}
