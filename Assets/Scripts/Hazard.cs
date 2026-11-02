using UnityEngine;

// Dangerous obstacle. On contact with the player it raises an event and disappears.
// It does NOT call any method on the player - PlayerHealth hears the event and applies damage.
public class Hazard : MonoBehaviour
{
    [Range(0f, 1f)] public float damageFraction = 0.25f;   // a quarter of the bar per hit

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameEvents.RaiseObstacleHit(damageFraction);
        Destroy(gameObject);
    }
}
