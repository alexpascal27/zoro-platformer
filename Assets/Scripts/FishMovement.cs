using System;
using UnityEngine;

// SHould handle the basic movement of the fish (up down movement)
public class FishMovement : MonoBehaviour
{
    private Rigidbody2D fishRb;
    [SerializeField] private LayerMask waterLayerMask;
    private BoxCollider2D boxCollider2D;
    private bool isJumping = false;
    [SerializeField] private float jumpVelocity = 100f;
    [SerializeField] private float playerUpwardsPush = 10f;

    void Awake()
    {
        fishRb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (isGrounded())
        {
            fishRb.velocity = Vector2.up * jumpVelocity;
        }
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f,
            Vector2.down, .1f, waterLayerMask);
        return raycastHit2D.collider != null;
    }

    // If we collide with a player, send the player jumping up
    private void OnCollisionEnter2D(Collision2D other)
    {
        GameObject colliderGameObject = other.gameObject;
        if (colliderGameObject.CompareTag("Player"))
        {
            // Get player rb
            Rigidbody2D playerRb = colliderGameObject.GetComponent<Rigidbody2D>();
            // Push the player up
            playerRb.velocity = Vector2.up * playerUpwardsPush;
        }
    }
}
