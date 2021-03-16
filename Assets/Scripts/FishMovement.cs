using System;
using UnityEngine;

// SHould handle the basic movement of the fish (up down movement)
public class FishMovement : MonoBehaviour
{
    private Rigidbody2D fishRb;
    [SerializeField] private LayerMask waterLayerMask;
    [SerializeField] private LayerMask boatLayerMask;
    private BoxCollider2D boxCollider2D;
    private bool atApex = false;
    [SerializeField] private float jumpVelocity = 100f;
    [SerializeField] private float playerUpwardsPush = 10f;
    [SerializeField] private int circleCastRadius = 5;

    [SerializeField] private float biteSize = 1f;

    void Awake()
    {
        fishRb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        // Check if we are approximately at apex
        if (fishRb.velocity.y <= 0 && this.atApex == false)
        {
            this.atApex = true;
            ProcessAtApex();
        }
        
        if (isGrounded())
        {
            // Jump
            fishRb.velocity = Vector2.up * jumpVelocity;
            // Set high point to false as we are definitely not at apex of jump
            this.atApex = false;
        }
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f,
            Vector2.down, .1f, waterLayerMask);
        return raycastHit2D.collider != null;
    }

    private void ProcessAtApex()
    {
        // Find the boat
        // Cast a CircleCast
        RaycastHit2D raycastHit2D =
            Physics2D.CircleCast(fishRb.position, circleCastRadius, Vector2.left, 2f , boatLayerMask);
        if (raycastHit2D.collider != null)
        {
            GameObject boatGameObject = raycastHit2D.collider.gameObject;
            BoxCollider2D boatBoxCollider2D = boatGameObject.GetComponent<BoxCollider2D>();
            Vector2 boatCenterPosition = boatBoxCollider2D.bounds.center;
            Vector2 nearestBitingBlockPosition = FindNearestBitingPoint(boatBoxCollider2D, boatCenterPosition);
            Debug.Log("NearestBitingPoint: " + nearestBitingBlockPosition);

            // Less Expensive way of doing it but more manual and error prone
            /*
            Vector2 boatPosition = raycastHit2D.collider.transform.position;
            boatPosition = ApplyBoatPositionOffset(boatPosition);
            Debug.Log(boatPosition);
            */
        }
        
    }

    // At the moment its all based on the idea that the fish is to the RIGHT of the boat (boat is on the left of the fish)
    private Vector2 FindNearestBitingPoint(BoxCollider2D boatBoxCollider, Vector2 boatCenterPosition)
    {
        // Find size
        float boatSize = boatBoxCollider.size.x;

        // Half size as we need one side
        float halfSize = boatSize / 2;

        // Center + half - bitesize/2 to find center of biting point
        float bitingPointXCenter = boatCenterPosition.x + halfSize - biteSize/2;

        // return
        return new Vector2(bitingPointXCenter, boatCenterPosition.y);
    }

    private Vector2 ApplyBoatPositionOffset(Vector2 outputBoatPosition)
    {
        float offsetX = -3.14f;
        float offsetY = -5.4f;
        outputBoatPosition.x += offsetX;
        outputBoatPosition.y += offsetY;
        return outputBoatPosition;
    }

    // If we collide with a player, send the player jumping up to help get on the boat
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
