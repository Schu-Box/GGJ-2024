using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bumper3D : MonoBehaviour
{
    public MMF_Player bumpFeedback;

    public bool ignoreYForce = true;

    public float bumpForce = 100f;

    private void OnCollisionEnter(Collision collision)
    {
        bumpFeedback.PlayFeedbacks();

        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 direction = collision.GetContact(0).point - collision.transform.position;
            direction = -direction.normalized;

            if (ignoreYForce)
            {
                direction.y = 0;
            }
            
            Player3D.Instance.BumpAway(direction, bumpForce);
        }
    }
}
