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
    private bool touchingPlayer = false;
    private GameObject playerGameObject = null;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CheckIfBoatBroken();
        
        if (canMove)
        {
            if (touchingPlayer) MovePlayer();
            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(movementSpeed, rb.velocity.y);
            // And then smoothing it out and applying it to the character
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref zero, m_MovementSmoothing);
        }
    }

    void CheckIfBoatBroken()
    {
        float boatSize = GetBoatSize();
        if(boatSize < 2) Destroy(gameObject);
    }
    
    private float GetBoatSize()
    {
        float boatScale = gameObject.transform.localScale.x;
        // Find collider size
        BoxCollider2D boatBoxCollider2D = GetComponent<BoxCollider2D>();
        float colliderSize = boatBoxCollider2D.size.x;
        return colliderSize * boatScale;
    }

    private void MovePlayer()
    {
        CharacterController2D characterController = playerGameObject.GetComponent<CharacterController2D>();
        characterController.Move(movementSpeed/5, false, false);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        GameObject collisionGameObject = other.gameObject;
        if (collisionGameObject.CompareTag("Player"))
        {
            if (!canMove) canMove = true;
            touchingPlayer = true;
            playerGameObject = collisionGameObject;
        }
        else if (collisionGameObject.CompareTag("Pole"))
        {
            // Reverse movement
            movementSpeed = -movementSpeed;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        GameObject collisionGameObject = other.gameObject;
        if (collisionGameObject.CompareTag("Player"))
        {
            touchingPlayer = false;
            playerGameObject = null;
        }
    }
}
