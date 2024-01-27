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
    public MMF_Player feedback_pickup;

    private void Start()
    {
        massValue = transform.localScale.x * transform.localScale.y;
    }
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if(Player.Instance.CanCollectBumper(this) && Player.Instance.GetCurrentMass() >= massValue)
            {
                Player.Instance.Pickup(this);
                
                feedback_pickup.PlayFeedbacks();
            }
            
            //DO we also bump or nah?
            
            feedback_bump?.PlayFeedbacks();
            
            Vector2 bumpDirection = col.GetContact(0).point - (Vector2)col.transform.position;
            bumpDirection = -bumpDirection.normalized;
            
            Player.Instance.Bumped(this, bumpDirection * bumpForce);
        }
    }
}
