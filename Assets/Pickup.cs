using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

//Obsolete

public class Pickup : MonoBehaviour
{
    // public float massThresholdForCollection = 1;
    public float massValue = 1f;
    
    public MMF_Player feedback_pickup;

    private void Start()
    {
        massValue = transform.localScale.x * transform.localScale.y;
    }
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            // float forceOfCollision = col.relativeVelocity.magnitude;
            
            // if(Player.Instance.CanCollectBumper(this) && Player.Instance.GetCurrentMass() >= massValue)
            // {
            //     // Player.Instance.Pickup(this);
            //     
            //     feedback_pickup.PlayFeedbacks();
            // }
        }
    }
}
