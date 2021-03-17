using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ChaserMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private float verticalRaycastDistance = 10f;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    [Range(0.5f, 50f)] [SerializeField] private float movementSpeed = 0.01f;	
    private Vector3 zero = Vector3.zero;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(movementSpeed, rb.velocity.y);
        // And then smoothing it out and applying it to the character
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref zero, m_MovementSmoothing);

        CheckAboveAndBelow();
    }

    private void CheckAboveAndBelow()
    {
        Vector2 currentChaserPosition = rb.transform.position;
        // Raycast up
        RaycastHit2D upwardsRaycastHit2D = Physics2D.Raycast(currentChaserPosition, Vector2.up, verticalRaycastDistance, playerLayerMask);

        // Raycast down
        RaycastHit2D downwardsRaycastHit2D = Physics2D.Raycast(currentChaserPosition, Vector2.down, verticalRaycastDistance, playerLayerMask);
        
        
        // If either have a collider with tag player then reload scene
        if (upwardsRaycastHit2D.collider != null || downwardsRaycastHit2D.collider != null)
        {
            ReloadScene();
        }
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ReloadScene();
        }
        else
        {
            Physics2D.IgnoreCollision(other.collider, gameObject.GetComponent<Collider2D>());
        }
    }
}
