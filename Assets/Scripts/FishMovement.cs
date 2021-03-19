using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// SHould handle the basic movement of the fish (up down movement)
public class FishMovement : MonoBehaviour
{
    private Rigidbody2D fishRb;
    [SerializeField] private LayerMask waterLayerMask;
    [SerializeField] private LayerMask boatLayerMask;
    private BoxCollider2D boxCollider2D;

    public bool fishOnRightOfBoat = true;
    private float leftRaycastDistance;
    [SerializeField] private float rightRaycastDistance = 3f;
    
    private bool atApex = false;
    [SerializeField] private float jumpVelocity = 100f;
    [SerializeField] private float playerUpwardsPush = 10f;
    
    // Biting
    [SerializeField] private int circleCastRadius = 5;
    private bool isBiting = false;
    private Vector2 biteRotationPoint = Vector2.zero;
    private Vector2 bitingPoint = Vector2.zero;
    [SerializeField] private float biteRotationSpeed = 10f;
    [SerializeField] public float biteSize = 1f;
    
    // Boat
    [SerializeField] private GameObject staticBoatGameObject;
    private GameObject boatGameObject = null;
    private BoxCollider2D boatBoxCollider2D = null;

    void Awake()
    {
        fishRb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        FishSpawn fishSpawn = staticBoatGameObject.GetComponent<FishSpawn>();
        leftRaycastDistance = fishSpawn.leftMinDistance;
    }

    void Update()
    {
        // Check if we are approximately at apex
        if (fishRb.velocity.y <= 0 && this.atApex == false)
        {
            this.atApex = true;
            ProcessAtApex();
        }
        
        CheckIfFishNeedToDie();
        
        if (isGrounded())
        {
            // Jump
            fishRb.velocity = Vector2.up * jumpVelocity;
            // Set high point to false as we are definitely not at apex of jump
            this.atApex = false;
        }

        
    }

    private void CheckIfFishNeedToDie()
    {
        // fish dies if rotation is over 180
        if (fishRb.transform.rotation.z < -179f || fishRb.transform.rotation.z > 179f)
        {
            Destroy(gameObject);
        }
        
    }

    void FixedUpdate()
    {
        // Calculate speed of bite
        float speed = biteRotationSpeed * Time.deltaTime;

        if (isBiting)
        {
            // Rotate to 
            if (fishOnRightOfBoat)
            {
                fishRb.transform.RotateAround(biteRotationPoint, new Vector3(0, 0, 1), speed);
            }
            else
            {
                fishRb.transform.RotateAround(biteRotationPoint, new Vector3(0, 0, -1), speed);
            }
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
        RaycastHit2D raycastHit2D;
        if (fishOnRightOfBoat)
        {
            raycastHit2D = Physics2D.CircleCast(fishRb.position, circleCastRadius, Vector2.left, leftRaycastDistance , boatLayerMask);
        }
        else
        {
            raycastHit2D = Physics2D.CircleCast(fishRb.position, circleCastRadius, Vector2.right, rightRaycastDistance , boatLayerMask);
        }
        
        if (raycastHit2D.collider != null)
        {
            boatGameObject = raycastHit2D.collider.gameObject;
            boatBoxCollider2D = boatGameObject.GetComponent<BoxCollider2D>();
            Vector2 boatCenterPosition = boatBoxCollider2D.bounds.center;
            bitingPoint = FindNearestBitingPoint(boatCenterPosition);
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
    private Vector2 FindNearestBitingPoint(Vector2 boatCenterPosition)
    {
        float boatVelocityX = boatGameObject.GetComponent<Rigidbody2D>().velocity.x;
        
        float boatScale = boatGameObject.transform.localScale.x;
        // Find collider size
        float colliderSize = boatBoxCollider2D.size.x;
        float boatSize = colliderSize * boatScale;

        // Half size as we need one side
        float halfSize = boatSize / 2;
        
        // Center + half - bitesize/2 to find center of biting point
        float bitingPointXCenter;
        if (fishOnRightOfBoat)
        {
            bitingPointXCenter = boatCenterPosition.x + halfSize - biteSize/2;
        }
        else
        {
            bitingPointXCenter = boatCenterPosition.x - halfSize + biteSize/2;
        }
         bitingPointXCenter += GetBitingOffset(boatVelocityX);
            

        // return
        return new Vector2(bitingPointXCenter, boatCenterPosition.y);
    }

    private float GetBitingOffset(float boatVelocityX)
    {
        Debug.Log(boatVelocityX);
        // We expect a travel distance of 180 degrees, we work out how the fish dives (how many degrees per time)
        float degreesPerTime = biteRotationSpeed / 50f;
        // Calculate the time it would take us to bite
        float secondsToBitingPoint = (180f / degreesPerTime)/50f;
        // Calculate boat travel during that time
        float boatTravelX =  boatVelocityX * secondsToBitingPoint;

       // Debug.Log("Degrees Per time: " + degreesPerTime + ", SecondsToBitingPoint: "+ secondsToBitingPoint +", BoatTravelX: " + boatTravelX);
        return boatTravelX;
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
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            
            
        }
        else if (colliderGameObject.CompareTag("Boat"))
        {
            ReduceBoatSizeAndKillFish();
        }
    }

    private void OnDestroy()
    {
        // Find boats in scene
        GameObject[] boats = GameObject.FindGameObjectsWithTag("Boat");

        // Identify Closest boat
        GameObject closestBoat = GetClosestBoat(boats);
        // If found any boat
        if (closestBoat != null)
        {
            // Deduct 1 from active fishes in the FishSpawn script
            FishSpawn fishSpawn = closestBoat.GetComponent<FishSpawn>();
            fishSpawn.numberOfActiveFishes--;
        }
    }

    private GameObject GetClosestBoat(GameObject[] boats)
    {
        float smallestDistanceX = Single.PositiveInfinity;
        // Only care about X
        float fishPositionX = gameObject.transform.position.x;
        foreach (GameObject boat in boats)
        {
            float distance = GetDistanceBetween(fishPositionX, boat.transform.position.x);
            if (distance < smallestDistanceX) return boat;
        }
        return null;
    }

    private float GetDistanceBetween(float x1, float x2)
    {
        return Math.Abs(x1 - x2);
    }
}
