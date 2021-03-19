using System;
using UnityEngine;
using Random = System.Random;

public class FishSpawn : MonoBehaviour
{
    [SerializeField]private GameObject fishGameObject;
    
    private float biteSize;
    
    [Range(1,20)][SerializeField] int numberOfFishesAtTime = 3;
    private const float fishStartPositionY = -4.76f;
    [Range(1f, 20f)] [SerializeField] private float leftMinDistance = 11f;
    [Range(1f, 20f)] [SerializeField] private float rightMinDistance = 11f;
    [Range(1f, 20f)] [SerializeField] private float leftSpawnRange = 10f;
    [Range(1f, 20f)] [SerializeField] private float rightSpawnRange = 10f;

    private int numberOfActiveFishes = 0;

    private void Awake()
    {
        biteSize = fishGameObject.GetComponent<FishMovement>().biteSize;
    }

    void Update()
    {
        // Check if we need to spawn
        // If we need to spawn
        if (numberOfActiveFishes < numberOfFishesAtTime)
        {
            Debug.Log("Not enough fishes: "+ numberOfActiveFishes);
            // Check if we can spawn (either left or right are free or both)
            bool canSpawnLeft = CanSpawn(true);
            bool canSpawnRight = CanSpawn(false);

            // If not then we ignore
            if (canSpawnLeft || canSpawnRight)
            {
                // If we can - if both available then randomise side, if only one pick one
                bool spawnLeft = SpawnLeftSide(canSpawnLeft, canSpawnRight);
                // Calculate spawn location  = minDistance + random value in range
                Vector2 spawnLocation = GetSpawnLocation(spawnLeft);
                // Spawn and increase the no of active fishes accordingly
                fishGameObject.GetComponent<FishMovement>().fishOnRightOfBoat = !spawnLeft;
                fishGameObject.transform.position = spawnLocation;
                Instantiate(fishGameObject);
                Debug.Log("Spawned a fish, spawnLeft: "+ spawnLeft + ", position: " + spawnLocation);
                numberOfActiveFishes++;
            }
        }
    }

    private Vector2 GetBoatCenterPosition()
    {
        BoxCollider2D boatBoxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        return boatBoxCollider2D.bounds.center;
    }

    private bool CanSpawn(bool left)
    {
        RaycastHit2D raycastHit2D;
        Vector2 boatCenterPosition = GetBoatCenterPosition();
        if (left)
        {
            raycastHit2D =
                Physics2D.Raycast(boatCenterPosition, Vector2.left, leftMinDistance + leftSpawnRange, 1 << LayerMask.NameToLayer("Default"));
        }
        else
        {
            raycastHit2D =
                Physics2D.Raycast(boatCenterPosition, Vector2.right, rightMinDistance + rightSpawnRange, 1 << LayerMask.NameToLayer("Default"));
        }
        return raycastHit2D.collider == null;
    }

    private bool SpawnLeftSide(bool canSpawnLeft, bool canSpawnRight)
    {
        // Just left
        if (canSpawnLeft && !canSpawnRight)
        {
            return true;
        }
        // Just right
        else if (!canSpawnLeft && canSpawnRight)
        {
            return false;
        }
        // If both
        else
        {
            // Randomise 
            Random rand = new Random();
            return rand.Next(1) == 1;
        }
    }

    private Vector2 GetSpawnLocation(bool spawnLeft)
    {
        float bitingPointX = FindNearestBitingPoint(spawnLeft).x;

        Random rand = new Random();

        float fishStartPositionX;
        if (spawnLeft)
        {
            fishStartPositionX = bitingPointX - leftMinDistance;
            float posXInRange = rand.Next((int)leftSpawnRange);
            fishStartPositionX -= posXInRange;
        }
        else
        {
            fishStartPositionX = bitingPointX + rightMinDistance;
            float posXInRange = rand.Next((int)rightSpawnRange);
            fishStartPositionX += posXInRange;
        }

        return new Vector2(fishStartPositionX, fishStartPositionY);
    }
    
    private Vector2 FindNearestBitingPoint(bool spawnLeft)
    {
        Vector2 boatCenterPosition = GetBoatCenterPosition();
        
        // Find size
        float boatScale = gameObject.transform.localScale.x;
        // Find collider size
        float colliderSize = gameObject.GetComponent<BoxCollider2D>().size.x;
        
        float boatSize = colliderSize * boatScale;

        // Half size as we need one side
        float halfSize = boatSize / 2;
        
        // Center + half - bitesize/2 to find center of biting point
        float bitingPointXCenter;
        if (!spawnLeft)
        {
            bitingPointXCenter = boatCenterPosition.x + halfSize - biteSize/2;
        }
        else
        {
            bitingPointXCenter = boatCenterPosition.x - halfSize + biteSize/2;
        }

        // return
        return new Vector2(bitingPointXCenter, boatCenterPosition.y);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // If fish dives on boat
        if (other.gameObject.CompareTag("Fish"))
        {
            // Dont kill fish as it already dies based on another script
            // Just reduce active fishes
            numberOfActiveFishes--;
            Debug.Log("Reduced number of active fishes by 1, new number: " + numberOfActiveFishes);
        }
    }
}
