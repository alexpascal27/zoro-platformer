using System;
using UnityEngine;

public class TurretShooting : MonoBehaviour
{
    private Rigidbody2D rb;
    private TurretMovement _turretMovement;
    private bool followPlayerIfInSight;
    private float startingZAngle;
    
    [SerializeField]private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [Range(0.1f, 500f)] [SerializeField] private float bulletForce = 20f;
    [Range(0.1f, 500f)] [SerializeField] private float lockOnDuration = 2f;
    private float shootCountDown = 0f;
    private bool lockedOn = false;
    
    [Range(0.1f, 20f)] [SerializeField] private float raycastDistance = 10f;
    private bool inSightOfPlayer = false;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _turretMovement = GetComponent<TurretMovement>();
        followPlayerIfInSight = _turretMovement.followPlayerIfInSight;
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
                    PlayerInSight(raycastCollider2D);
                }
                // If already in sight of player then update player position
                else
                {
                    _turretMovement.playerGameObject = raycastCollider2D.gameObject;
                }
                
            }
            else
            {
                PlayerOutOfSight();
            }
        }
        else
        {
            PlayerOutOfSight();
        }
        
    }

    private Collider2D GetColliderInRaycast()
    {
        Vector3 direction = Vector2.down;
        float zAngleToRotateBy = rb.transform.rotation.eulerAngles.z - startingZAngle;
        direction = Quaternion.Euler(0, 0, zAngleToRotateBy) * direction;
        RaycastHit2D raycastHit2D = Physics2D.Raycast(firePoint.position, direction, raycastDistance);
        return raycastHit2D.collider;
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
    }

    private void PlayerInSight(Collider2D playerCollider2D)
    {
        if (followPlayerIfInSight)
        {
            _turretMovement.standBy = false;
            _turretMovement.playerGameObject = playerCollider2D.gameObject;

            if (shootCountDown <= 0f)
            {
                if (!lockedOn)
                {
                    shootCountDown = lockOnDuration;
                    lockedOn = true;
                }
                else
                {
                    Shoot();
                    lockedOn = false;
                    shootCountDown = 0f;
                }
            }
            else
            {
                shootCountDown -= Time.deltaTime;
            }
        }
    }

    private void PlayerOutOfSight()
    {
        if (followPlayerIfInSight)
        {
            _turretMovement.standBy = true;
            _turretMovement.playerGameObject = null;

            shootCountDown = 0f;
            lockedOn = false;
        }
    }
}
