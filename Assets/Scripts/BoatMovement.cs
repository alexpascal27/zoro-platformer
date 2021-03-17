using System;
using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    [Range(0.5f, 50f)] [SerializeField] private float movementSpeed = 0.01f;	
    private Vector3 zero = Vector3.zero;
    // Boat only able to move when player jumps on it
    private bool canMove = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (canMove)
        {
            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(movementSpeed, rb.velocity.y);
            // And then smoothing it out and applying it to the character
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref zero, m_MovementSmoothing);
        }
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        GameObject collisionGameObject = other.gameObject;
        if (collisionGameObject.CompareTag("Player"))
        {
            if (!canMove) canMove = true;
        }
        else if (collisionGameObject.CompareTag("Pole"))
        {
            // Reverse movement
            movementSpeed = -movementSpeed;
        }
    }
}
