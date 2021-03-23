using UnityEngine;

public class TurretMovement : MonoBehaviour
{
    // Move turret 
    private Rigidbody2D rb;
    private bool left = true;

    [Range(0f, 500f)][SerializeField] private float rotationSpeed = 100f;

    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float speed = rotationSpeed * Time.deltaTime;
        if (left)
        {
            // Rotate left till at 90 degrees
            if (rb.transform.rotation.eulerAngles.z < 90) left = false;
            else
            {
                rb.transform.RotateAround(rb.transform.position, new Vector3(0, 0, -1), speed);
            }
        }
        else
        {
            // Rotate right till at 270 degrees
            if (rb.transform.rotation.eulerAngles.z > 270) left = true;
            else
            {
                rb.transform.RotateAround(rb.transform.position, new Vector3(0, 0, 1), speed);
            }
        }
    }
}
