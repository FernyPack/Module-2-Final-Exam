using UnityEngine;
using UnityEngine.InputSystem;

// Player movement.
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 9f;
    public float xLimit = 9f;
    public float zMin = -2f;
    public float zMax = 4f;

    void Update()
    {
        float h = 0f, v = 0f;
        Keyboard kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)  h -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) h += 1f;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed)  v -= 1f;
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed)    v += 1f;
        }
        Vector3 move = new Vector3(h, 0f, v).normalized;

        transform.position += move * moveSpeed * Time.deltaTime;

        Vector3 p = transform.position;
        p.x = Mathf.Clamp(p.x, -xLimit, xLimit);
        p.z = Mathf.Clamp(p.z, zMin, zMax);
        transform.position = p;
    }
}