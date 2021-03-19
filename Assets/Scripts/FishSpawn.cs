using System;
using UnityEngine;

public class FishSpawn : MonoBehaviour
{
    [SerializeField]private GameObject fishGameObject;
    
    [Range(1,20)][SerializeField] int numberOfFishesAtTime = 3;
    [Range(5f, 20f)] [SerializeField] private float leftMinDistance = 11f;
    [Range(5f, 20f)] [SerializeField] private float rightMinDistance = 11f;
    [Range(5f, 20f)] [SerializeField] private float leftSpawnRange = 10f;
    [Range(5f, 20f)] [SerializeField] private float rightSpawnRange = 10f;

    private int numberOfActiveFishes = 0;

    void Update()
    {
        // Check if we need to spawn
        // If we need to spawn
        if (numberOfActiveFishes < numberOfActiveFishes)
        {
            // Check if we can spawn (either left or right are free or both)
            bool canSpawnLeft = CanSpawn(true);
            bool canSpawnRight = CanSpawn(false);

            // If not then we ignore
            if (canSpawnLeft || canSpawnRight)
            {
                // If we can - if both available then randomise side, if only one pick one
                bool spawnLeft = SpawnLeftSide(canSpawnLeft, canSpawnRight);
                // Calculate spawn location  = minDistance + random value in range
                Vector2 spawnLocation = getSpawnLocation(spawnLeft);
                // Spawn and increase the no of active fishes accordingly
                fishGameObject.GetComponent<FishMovement>().fishOnRightOfBoat = !spawnLeft;
                Instantiate(fishGameObject);
                numberOfActiveFishes++;
            }
        }
    }

    bool CanSpawn(bool left)
    {
        if (left)
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast() 
        }
        else
        {
            
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // If fish dives on boat
        if (other.gameObject.CompareTag("Fish"))
        {
            // Dont kill fish as it already dies based on another script
            // Just reduce active fishes
            numberOfActiveFishes--;
        }
    }
}
