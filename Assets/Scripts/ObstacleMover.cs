using UnityEngine;

// Moves objects toward the player.
public class ObstacleMover : MonoBehaviour
{
    public float speed = 18f;
    public float despawnZ = -15f;

    void Update()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);

        if (transform.position.z < despawnZ)
            Destroy(gameObject);
    }
}
