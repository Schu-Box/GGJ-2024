using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using Random = System.Random;

public class Bumper : MonoBehaviour
{
    public float bumpForce = 10f;
    
    public float massRequiredToBreak = 1f;
    public float massGivenForBreaking = 1f;

    public bool canBeTarget = false;
    public bool isTrigger = false;

    // [HideInInspector]public float massValue;

    public MMF_Player feedback_bump;
    public MMF_Player feedback_target;
    public MMF_Player feedback_breakable;
    public MMF_Player feedback_unbreakable;
    public MMF_Player feedback_pickup;

    public Transform blobParent;
    private List<Blob> blobList = new List<Blob>();

    private Collider2D collie;
    
    private bool breakable = false;
    private bool isTarget = false;

    private void Start()
    {
        // massValue = transform.localScale.x * transform.localScale.y;

        if (gameObject.activeInHierarchy)
        {
            GameController.Instance.AddBumper(this);
        }

        collie = GetComponent<Collider2D>();

        if (isTrigger)
        {
            collie.isTrigger = true;
        }
        
        foreach (Transform child in blobParent)
        {
            blobList.Add(child.GetComponent<Blob>());
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Debug.Log("Hit with speed: " + Player.Instance.GetLastVelocity());

            if (isTarget)
            {
                HitTarget();
            }
            
            feedback_bump?.PlayFeedbacks();
        
            Vector2 bumpDirection = collision.GetContact(0).point - (Vector2)collision.transform.position;
            bumpDirection = -bumpDirection.normalized;
        
            Player.Instance.Bumped(this, bumpDirection * bumpForce);
            
            //TODO: Chance of displaying blob

            float chanceOfBlob = 0.3f;
            
            if(UnityEngine.Random.value < chanceOfBlob)
            {
                Blob randomBlob = blobList[UnityEngine.Random.Range(0, blobList.Count)];
                if (!randomBlob.displayed)
                {
                    randomBlob.Display();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.CompareTag("Player"))
        {
            RemoveBumper();

            Player.Instance.Pickup(this);
                
            feedback_pickup?.PlayFeedbacks();
            
            GameController.Instance.AddScore(1);
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
        // breakable = nowBreakable;
        // SetBreakableVisuals();
        // collie.isTrigger = breakable;
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

    public void SetAsTarget()
    {
        isTarget = true;
        
        Debug.Log("Setting new target: " + gameObject.name);
        
        feedback_target?.PlayFeedbacks();
    }

    public void HitTarget()
    {
        isTarget = false;
        
        //TODO: No longer target feedback
        feedback_unbreakable?.PlayFeedbacks();
        
        GameController.Instance.SetRandomTarget();
        
        GameController.Instance.AddScore(10);
    }
}
