using System;
using UnityEngine;

// SHould handle the basic movement of the fish (up down movement)
public class FishMovement : MonoBehaviour
{
    private Rigidbody2D fishRb;
    [SerializeField] private LayerMask waterLayerMask;
    [SerializeField] private LayerMask boatLayerMask;
    private BoxCollider2D boxCollider2D;

    private bool fishOnRightOfBoat = true;
    
    private bool atApex = false;
    [SerializeField] private float jumpVelocity = 100f;
    [SerializeField] private float playerUpwardsPush = 10f;
    
    // Biting
    [SerializeField] private int circleCastRadius = 5;
    private bool isBiting = false;
    private Vector2 biteRotationPoint = Vector2.zero;
    private Vector2 bitingPoint = Vector2.zero;
    [SerializeField] private float biteRotationSpeed = 10f;
    [SerializeField] private float biteSize = 1f;
    
    // Boat
    private GameObject boatGameObject = null;
    private BoxCollider2D boatBoxCollider2D = null;

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

    void FixedUpdate()
    {
        // Calculate speed of bite
        float speed = biteRotationSpeed * Time.deltaTime;

        if (isBiting)
        {
            // Rotate to 
            fishRb.transform.RotateAround(biteRotationPoint, new Vector3(0, 0, 1), speed);
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
            Physics2D.CircleCast(fishRb.position, circleCastRadius, Vector2.left, 1.5f , boatLayerMask);
        if (raycastHit2D.collider != null)
        {
            boatGameObject = raycastHit2D.collider.gameObject;
            boatBoxCollider2D = boatGameObject.GetComponent<BoxCollider2D>();
            Vector2 boatCenterPosition = boatBoxCollider2D.bounds.center;
            bitingPoint = FindNearestBitingPoint(boatBoxCollider2D, boatCenterPosition);
            Bite();

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

    private void Bite()
    {
        // Remove Gravity
        fishRb.gravityScale = 0;

        Vector3 fishPosition = fishRb.transform.position;
        // Find point to rotate around for the dive
        if (fishOnRightOfBoat)
        {
            // Fish position - bite position
            Vector2 midPoint = (fishPosition - (Vector3)bitingPoint)/2;
            // Add to bite position
            biteRotationPoint = bitingPoint + midPoint;
        }
        else
        {
            // Bite position - fish position
            Vector3 midPoint = ((Vector3)bitingPoint - fishPosition)/2;
            // Add to fish position
            biteRotationPoint =  fishPosition + midPoint;
        }

        Debug.Log("biteRotationPoint: " + biteRotationPoint);
        
        // Trigger move towards biting point
        isBiting = true;
    }

    private void ReduceBoatSizeAndKillFish()
    {
        float tileSizeX = boatBoxCollider2D.size.x;

        // Reduce sprite size
        // Change Scale
        Transform boatTransformParent= boatGameObject.transform.parent;
        boatGameObject.transform.parent = null;
        boatGameObject.transform.localScale *= new Vector2((tileSizeX - biteSize) / tileSizeX, 1);
        boatGameObject.transform.parent = boatTransformParent;
        // Move to look like bite
        // If on right then minus from offset if on left then add to offset
        boatGameObject.transform.position -= new Vector3((fishOnRightOfBoat ? biteSize/2 : -biteSize/2), 0, 0);
        
        // Kill fish
        Destroy(gameObject);
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
            
            
            // If player is above fish (jumping on fish) then kill fish
            if (playerRb.transform.position.y >= fishRb.transform.position.y)
            {
                Destroy(gameObject);
            }
            // Player 'dies' if fish dives on player
            else
            {
                Destroy(colliderGameObject);
            }
            
            
        }
        else if (colliderGameObject.CompareTag("Boat"))
        {
            ReduceBoatSizeAndKillFish();
        }
    }
}
