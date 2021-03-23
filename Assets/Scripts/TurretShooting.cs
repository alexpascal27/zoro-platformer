using System;
using UnityEngine;

public class TurretShooting : MonoBehaviour
{
    private Rigidbody2D rb;
    private float startingZAngle;
    [SerializeField]private Transform tipCheck;
    [Range(0.1f, 20f)] [SerializeField] private float raycastDistance = 10f;
    private bool inSightOfPlayer = false;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startingZAngle = rb.transform.rotation.eulerAngles.z;
    }

    private void Update()
    {
        // Raycast and return collider
        Collider2D raycastCollider2D = GetColliderInRaycast();
        if (raycastCollider2D != null)
        {
            // If collider is player
            if (raycastCollider2D.gameObject.CompareTag("Player"))
            {
                // Check if we are already in sight of player
                if (!inSightOfPlayer)
                {
                    // Shoot logic
                    Shoot();
                    inSightOfPlayer = true;
                }
                
            }
            else
            {
                inSightOfPlayer = false;
            }
        }
        else
        {
            inSightOfPlayer = false;
        }
        
    }

    private Collider2D GetColliderInRaycast()
    {
        Vector3 direction = Vector2.down;
        float zAngleToRotateBy = rb.transform.rotation.eulerAngles.z - startingZAngle;
        direction = Quaternion.Euler(0, 0, zAngleToRotateBy) * direction;
        RaycastHit2D raycastHit2D = Physics2D.Raycast(tipCheck.position, direction, raycastDistance);
        return raycastHit2D.collider;
    }

    private void Shoot()
    {
        Debug.Log("Player In Sight");
    }
}
