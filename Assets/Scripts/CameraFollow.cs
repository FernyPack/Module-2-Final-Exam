using UnityEngine;

// Follows the player from behind.
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 6f, -9f);
    public float followSharpness = 6f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = target.position + offset;

        // Frame-rate independent smoothing
        float t = 1f - Mathf.Exp(-followSharpness * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, desired, t);
        transform.LookAt(target.position + Vector3.forward * 4f);
    }
}
