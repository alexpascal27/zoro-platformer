using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ChaserMovement : MonoBehaviour
{
    private Rigidbody2D rb;
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
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
