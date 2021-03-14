
using System;
using UnityEngine;
using Random = System.Random;

public class PlayerMovement : MonoBehaviour
{
    // Movement
    public CharacterController2D controller;
    
    const float runSpeed = 50f;
    float _horizontalMove = 0f;
    bool jump = false;
    bool crouch = false;
    private bool right = true;

    // Sleep counter
    public bool sleepCounterOn = true;
    public int ccRageDuration = 5;
    
    private bool cc = false;
    private float whenCCStops = 0f;
    
    private bool raging = false;
    private float whenRageStops = 0f;

    private float sleepCounter = 50f;
    private const float SleepChange = 0.1f;

    // Control object
    private bool touchingControllableObject = false;
    private GameObject controllableGameObject = null;
    private bool pressingControl = false;
    
    
    
    // Update is called once per frame
    void Update()
    {
        /*
        Debug.Log("SleepCounter: " + sleepCounter);
        Debug.Log("CC: " + cc);
        Debug.Log("Raging: " + raging);
        */

        if (sleepCounterOn)
        {
            DealWithCcCooldown();
            DealWithRageCooldown();
            DealWithSleepCounter();
        }

        // Checking if control is pressed down
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)) pressingControl = true;
        else pressingControl = false;
        

        if (!cc)
        {
            if (raging) HandleRage();
            else
            {
                _horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
                float verticalMove = Input.GetAxisRaw("Vertical");
                if (sleepCounter > SleepChange && sleepCounter < (100f - SleepChange))
                {
                    if (_horizontalMove != 0 || verticalMove != 0) sleepCounter -= SleepChange;
                    else sleepCounter += SleepChange;
                }
                HandleJumpAndCrouch(verticalMove);
            }
        }
        else
        {
            _horizontalMove = 0f;
            jump = false;
            crouch = false;
        }
        
    }

    void FixedUpdate()
    {
        controller.Move(_horizontalMove * Time.fixedDeltaTime, crouch, jump);
        if (touchingControllableObject && pressingControl) MoveControllableObject(_horizontalMove * Time.deltaTime);
        crouch = false;
    }

    public void OnLanding()
    {
        jump = false;
    }

    private void MoveControllableObject(float objectHorizontalMove)
    {
        Debug.Log("Moving controllable Object");
        Rigidbody2D objectRb = controllableGameObject.GetComponent<Rigidbody2D>();
        Vector3 objectVelocity = objectRb.velocity;
        Vector3 zero = Vector3.zero;
        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(objectHorizontalMove * 10f, objectVelocity.y);
        // And then smoothing it out and applying it to the character
        objectRb.velocity = Vector3.SmoothDamp(objectVelocity, targetVelocity, ref zero, 0.05f);
    }

    private void HandleJumpAndCrouch(float verticalMove)
    {
        // If negative then down key, so crouch
        if (verticalMove < 0) 
        {
            crouch = true;
        }
        // Else, then up key so jump
        else if (verticalMove > 0) 
        {
            jump = true;
        }
    }

    private void DealWithCcCooldown()
    {
        // If the timer has already ended ensure the cc value is false
        if (cc)
        {
            if (whenCCStops < Time.time)
            {
                cc = false;
            }
        }
    }

    private void DealWithRageCooldown()
    {
        // If the timer has already ended ensure the cc value is false
        if (raging)
        {
            if (whenRageStops < Time.time)
            {
                raging = false;
            }
        }
    }

    private void HandleRage()
    {
        Random random = new Random();
        jump = random.Next(2) == 0;
        
        _horizontalMove = (right ? 1 : -1) * runSpeed;
    }

    private void DealWithSleepCounter()
    {
        if (sleepCounter > SleepChange && sleepCounter < (100f - SleepChange))
        {
            if (raging) sleepCounter -= SleepChange;
            if (cc) sleepCounter += SleepChange;
        }

        if (sleepCounter > 90f && !raging)
        {
            whenRageStops = Time.time + ccRageDuration;
            raging = true;
            Random random = new Random();
            right = random.Next(2) == 0;
        }

        if (sleepCounter < 10f && !cc)
        {
            whenCCStops = Time.time + ccRageDuration;
            cc = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        GameObject gameObject = other.gameObject;
        if (other.gameObject.CompareTag("ControllableObject"))
        {
            touchingControllableObject = true;
            controllableGameObject = gameObject;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("ControllableObject"))
        {
            touchingControllableObject = false;
            controllableGameObject = null;
        }
    }
}
