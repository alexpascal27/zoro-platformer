using System;
using UnityEngine;

// If we are holding control and the touchingControllableObject is true, then change the position of the touchedObject as 

public class PlayerObjectControl : MonoBehaviour
{
    [Range(1, 300)] [SerializeField] private int playerRunSpeed = 50;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    private Vector3 zero = Vector3.zero;
    
    private bool touchingControllableObject = false;
    private Rigidbody2D controllableObjectRigidBody2D = null;

    void Update()
    {
        // If we press the 
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            Debug.Log("TouchingControllableObject: " + touchingControllableObject);
            if (touchingControllableObject)
            {
                float horizontalMove = Input.GetAxisRaw("Horizontal");
                if (horizontalMove != 0)
                {
                    Debug.Log("HorizontalMove is not 0");
                    // Move the character by finding the target velocity
                    Vector3 targetVelocity = new Vector2(horizontalMove * playerRunSpeed / 10f, controllableObjectRigidBody2D.velocity.y);
                    // And then smoothing it out and applying it to the character
                    controllableObjectRigidBody2D.velocity = Vector3.SmoothDamp(controllableObjectRigidBody2D.velocity, targetVelocity, ref zero, m_MovementSmoothing);
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("ControllableObject"))
        {
            touchingControllableObject = true;
            controllableObjectRigidBody2D = other.gameObject.GetComponent<Rigidbody2D>();
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("ControllableObject"))
        {
            touchingControllableObject = false;
            controllableObjectRigidBody2D = null;
        }
    }
}

    
