using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    public float bumpForce = 10f;

    [HideInInspector]public float massValue;

    public MMF_Player feedback_bump;
    public MMF_Player feedback_breakable;
    public MMF_Player feedback_pickup;

    private Collider2D collie;

    private void Start()
    {
        massValue = transform.localScale.x * transform.localScale.y;

        GameController.Instance.AddBumper(this);

        collie = GetComponent<Collider2D>();
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(Player.Instance.CanCollectBumper(this) && Player.Instance.GetCurrentMass() >= massValue)
            {
                RemoveBumper();

                Player.Instance.Pickup(this);
                
                feedback_pickup.PlayFeedbacks();
            }
            
            //DO we also bump or nah?
            
            feedback_bump?.PlayFeedbacks();
            
            Vector2 bumpDirection = collision.GetContact(0).point - (Vector2)collision.transform.position;
            bumpDirection = -bumpDirection.normalized;
            
            Player.Instance.Bumped(this, bumpDirection * bumpForce);
        }
    }

    public void RemoveBumper()
    {
        collie.enabled = false;

        GameController.Instance.RemoveBumper(this);
    }

    public void ShowBreakableVisuals()
    {
        feedback_breakable.PlayFeedbacks();
    }
}
