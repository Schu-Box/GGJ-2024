using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player Instance;
    
    public float movementSpeed = 10;
    public float massAbsorbtionRate = 0.25f;

    public float linearDragWhenMoving = 1f;
    public float linearDragWhenGliding = 0.3f;

    public float startingMass = 1;
    [HideInInspector] public float currentMass;

    public MMF_Player feedback_bump;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    
    private bool activated = false;
    private Bumper lastBumperHit = null;

    private void Start()
    {
        Instance = this;
        
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();

        currentMass = startingMass;
    }

    private void FixedUpdate()
    {
        Vector2 movementVector = new Vector3(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
        rb.AddForce(movementVector * (movementSpeed * Time.deltaTime));
        
        // bool playerGivingInput = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
        if(movementVector.magnitude > 0)
        {
            Deactivate();
            
            ToggleMovementType(true);
        }
        else
        {
            ToggleMovementType(false);
        }
    }

    private bool moving = false;
    private void ToggleMovementType(bool isMoving)
    {
        if (!moving && isMoving) //started moving
        {
            rb.drag = linearDragWhenMoving;
        } 
        else if (moving && !isMoving)
        {
            rb.drag = linearDragWhenGliding;
        }

        moving = isMoving;
    }

    public float GetCurrentMass()
    {
        return transform.localScale.x * transform.localScale.y;
    }

    public bool CanCollectBumper(Bumper bumper)
    {
        if(bumper == lastBumperHit || !activated)
        {
            return false;
        }

        return true;
    }

    public void Bumped(Bumper bumper, Vector2 force)
    {
        feedback_bump?.PlayFeedbacks();

        lastBumperHit = bumper;

        rb.AddForce(force);

        activated = true;

        sr.color = Color.yellow;
    }

    public void Deactivate()
    {
        activated = false;

        sr.color = Color.white;
    }

    public void Pickup(Bumper pickup)
    {
        currentMass += massAbsorbtionRate;
        
        transform.localScale = Vector3.one * currentMass;

        GameController.Instance.UpdateBumperVisuals();
    }
}
