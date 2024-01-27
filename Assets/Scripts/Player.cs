using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public GameObject startingAnimationGameObject;
    
    [Header("Movement")]
    public float movementStrength = 1000;
    public float linearDragWhenMoving = 1f;
    public float linearDragWhenGliding = 0.3f;

    public float maxSpeedFromMovement = 10f;
    public float requiredSpeedToBreakBumper = 15f;

    [Header("Mass")]
    public float massAbsorbtionRate = 1f;
    public float scaleIncreasePerMass = 0.25f;

    public float startingMass = 1;
    
    public float currentMass;
    private float lastVelocity;

    [Header("Feedbacks")] 
    public MMF_Player feedback_launch;
    public MMF_Player feedback_bump;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    
    private bool activated = false;
    private Bumper lastBumperHit = null;

    private bool isRolling = false;

    private bool onCooldown = false;
    private float cooldownDuration = 1f;
    private float cooldownTimer = 0f;

    private void Start()
    {
        Instance = this;
        
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();

        animator.Play("Idle");

        currentMass = startingMass;
        
        startingAnimationGameObject.SetActive(true);
    }

    private void Update()
    {
        //WASD Movement
        /*
        Vector2 movementVector = new Vector3(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
        
        if (movementVector.x == 1f && rb.velocity.x > maxSpeedFromMovement) //Ignore movement if they're already at max
        {
            movementVector.x = 0;
        }
        else if(movementVector.x == -1f && rb.velocity.x < -maxSpeedFromMovement)
        {
            movementVector.x = 0;
        }
        
        if (movementVector.y == 1f && rb.velocity.y > maxSpeedFromMovement)
        {
            movementVector.y = 0;
        }
        else if(movementVector.y == -1f && rb.velocity.y < -maxSpeedFromMovement)
        {
            movementVector.y = 0;
        }
        
        Vector3 forceToAdd = movementVector * (movementStrength * Time.deltaTime);

        rb.AddForce(forceToAdd);
        
        if(movementVector.magnitude > 0)
        {
            ToggleMovementType(true);
        }
        else
        {
            ToggleMovementType(false);
        }
        */

        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
        else
        {
            if (onCooldown)
            {
                onCooldown = false;
                // Camera.main.backgroundColor = Color.gray;
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                Launch();
            }
        }
        
        if(!isRolling && GetCurrentSpeed() > 0)
        {
            animator.Play("Roll");
            isRolling = true;
        }
        else if(isRolling && GetCurrentSpeed() <= 0)
        {
            animator.Play("Idle");
            isRolling = false;
        }
    }

    private void Launch()
    {
        feedback_launch?.PlayFeedbacks();
        
        //get the mousePosition on the screen
        Vector3 test = Input.mousePosition;
        test.z = 10f;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(test);
           
        Vector2 direction = mousePosition - (Vector2)transform.position;
        direction.Normalize();
        rb.AddForce(direction * movementStrength, ForceMode2D.Impulse);

        onCooldown = true;
        cooldownTimer = cooldownDuration;
        // Camera.main.backgroundColor = Color.black;
    }

    private void LateUpdate()
    {
        lastVelocity = GetCurrentSpeed();
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
        return currentMass;
    }
    
    public float GetCurrentSpeed()
    {
        return rb.velocity.magnitude;
    }
    
    public float GetLastVelocity()
    {
        return lastVelocity;
    }

    public bool CanCollectBumper(Bumper bumper)
    {
        if(bumper == lastBumperHit || !activated)
        {
            return false;
        }

        return true;
    }

    private FMOD.Studio.EventInstance fmodStudioEvent;

    public void Bumped(Bumper bumper, Vector2 force)
    {
        feedback_bump?.PlayFeedbacks();

        lastBumperHit = bumper;

        rb.AddForce(force);

        fmodStudioEvent = FMODUnity.RuntimeManager.CreateInstance("event:/Bounce");
        fmodStudioEvent.start();
        fmodStudioEvent.release();

        // activated = true;
        //
        // sr.color = Color.yellow;
    }

    // public void Deactivate()
    // {
    //     activated = false;
    //
    //     sr.color = Color.white;
    // }

    public void Pickup(Bumper pickup)
    {
        float massIncrease = massAbsorbtionRate * pickup.massGivenForBreaking;
        currentMass += massIncrease;
        
        Debug.Log("Added mass: " + massIncrease);
        
        transform.localScale = Vector3.one + (Vector3.one * (scaleIncreasePerMass * currentMass));
    }
}
