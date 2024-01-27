using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    public float bumpForce = 10f;

    public float massRequiredToBreak = 1f;
    public float massGivenForBreaking = 1f;

    // [HideInInspector]public float massValue;

    public MMF_Player feedback_bump;
    public MMF_Player feedback_breakable;
    public MMF_Player feedback_unbreakable;
    public MMF_Player feedback_pickup;

    private Collider2D collie;
    
    private bool breakable = false;

    private void Start()
    {
        // massValue = transform.localScale.x * transform.localScale.y;

        GameController.Instance.AddBumper(this);

        collie = GetComponent<Collider2D>();
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Debug.Log("Hit with speed: " + Player.Instance.GetLastVelocity());
            
            feedback_bump?.PlayFeedbacks();
        
            Vector2 bumpDirection = collision.GetContact(0).point - (Vector2)collision.transform.position;
            bumpDirection = -bumpDirection.normalized;
        
            Player.Instance.Bumped(this, bumpDirection * bumpForce);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.CompareTag("Player"))
        {
            RemoveBumper();

            Player.Instance.Pickup(this);
                
            feedback_pickup?.PlayFeedbacks();
        }
    }

    public void SetBreakable(bool nowBreakable)
    {
        if ((nowBreakable && !breakable) || (!nowBreakable && breakable))
        {
            UpdateBreakability(nowBreakable);
        }
    }

    public void UpdateBreakability(bool nowBreakable)
    {
        breakable = nowBreakable;
        SetBreakableVisuals();
        collie.isTrigger = breakable;
    }

    public void RemoveBumper()
    {
        collie.enabled = false;

        GameController.Instance.RemoveBumper(this);
    }
    
    public void SetBreakableVisuals()
    {
        if (breakable)
        {
            feedback_breakable?.PlayFeedbacks();
        }
        else 
        {
            feedback_unbreakable?.PlayFeedbacks();
        }
    }
}
