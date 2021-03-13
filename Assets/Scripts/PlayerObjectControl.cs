using System;
using UnityEngine;

// If we are holding control and the touchingControllableObject is true, then change the position of the touchedObject as 

public class PlayerObjectControl : MonoBehaviour
{
    private bool touchingControllableObject = false;
    private GameObject controllableObject = null;
    private Vector3 positionalOffset = Vector3.zero;
    private bool attached = false;

    void Update()
    {
        // If we press the ctrl keys and the player is touching a controllable object
        if ((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)) && touchingControllableObject)
        {
            attached = true;
            Debug.Log("Attached");
        }
        else
        {
            if (attached)
            {
                attached = false;
            }
        }

        if (attached)
        {
            controllableObject.transform.position = this.transform.position - positionalOffset;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        GameObject gameObject = other.gameObject;
        if (other.gameObject.CompareTag("ControllableObject"))
        {
            touchingControllableObject = true;
            controllableObject = gameObject;
            positionalOffset = this.transform.position - gameObject.transform.position;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("ControllableObject"))
        {
            touchingControllableObject = false;
            controllableObject = null;
        }
    }
}

    
