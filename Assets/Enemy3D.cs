using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class Enemy3D : MonoBehaviour
{
    public MMF_Player attachFeedback;
    
    private Rigidbody rb;
    private Collider collie;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        collie = GetComponent<Collider>();
    }
    
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Enemy hit");

            Player3D.Instance.HitByEnemy(this);
        }
    }

    public void AttachToPlayer()
    {
        Destroy(rb);
        
        attachFeedback.PlayFeedbacks();

        float distanceTowardsPlayer = collie.bounds.extents.y * 1.1f;
        Vector3 newPosition = Vector3.MoveTowards(transform.position, Player3D.Instance.transform.position, distanceTowardsPlayer);
        transform.position = newPosition;
        
        //rotate transform so y is parallel to player
        Vector3 direction = Player3D.Instance.transform.position - transform.position;
        direction = -direction.normalized;
        transform.rotation = Quaternion.LookRotation(direction);
    }
}
