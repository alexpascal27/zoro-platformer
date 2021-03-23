using System;
using UnityEngine;

public class TurretMovement : MonoBehaviour
{
    // Move turret 
    private Rigidbody2D rb;
    private bool left = true;

    [SerializeField] public bool followPlayerIfInSight = false;
    public GameObject playerGameObject = null;
    // If on standby just do the surveillance otherwise follow player
    public bool standBy = true;
    [Range(0f, 360f)] [SerializeField] private float leftMaxBound = 270f; 
    [Range(0f, 360f)] [SerializeField] private float rightMaxBound = 90f;
    
    [Range(0f, 500f)][SerializeField] private float rotationSpeed = 100f;
    
    private 

    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (standBy)
        {
            RotateLeftAndRight();
        }
        else
        {
            FollowPlayer();
        }
        
    }

    private void RotateLeftAndRight()
    {
        float speed = rotationSpeed * Time.deltaTime;
        if (left)
        {
            // Rotate left till at 90 degrees
            if (rb.transform.rotation.eulerAngles.z < rightMaxBound) left = false;
            else
            {
                rb.transform.RotateAround(rb.transform.position, new Vector3(0, 0, -1), speed);
            }
        }
        else
        {
            // Rotate right till at 270 degrees
            if (rb.transform.rotation.eulerAngles.z > leftMaxBound) left = true;
            else
            {
                rb.transform.RotateAround(rb.transform.position, new Vector3(0, 0, 1), speed);
            }
        }
    }

    private void FollowPlayer()
    {
        if (playerGameObject != null)
        {
            // Angle between player and turret
            float angle = Vector2.Angle(rb.transform.position, playerGameObject.transform.position);
            angle -= 45;
            
            // Move turret by angle
            rb.transform.RotateAround(rb.position, new Vector3(0, 0, 1), angle);
            Debug.Log("Angle: " + angle);
        }
        else
        {
            standBy = true;
        }
    }
}
