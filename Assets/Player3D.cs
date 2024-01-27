using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class Player3D : MonoBehaviour
{
    public static Player3D Instance;
    
    public float movementStrength = 10f;

    public Transform attachmentsParent;
    
    public MMF_Player hitFeedback;
    
    private Rigidbody rb;

    private void Start()
    {
        Instance = this;
        
        rb = GetComponent<Rigidbody>();
    }
    
    private void FixedUpdate()
    {
        Vector3 movementVector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        
        rb.AddForce(movementVector * movementStrength);
    }

    public void BumpAway(Vector3 direction, float magnitude)
    {
        hitFeedback.PlayFeedbacks();
        
        rb.AddForce(direction * magnitude, ForceMode.Impulse);
    }

    public void HitByEnemy(Enemy3D enemy3D)
    {
        enemy3D.AttachToPlayer();
        
        enemy3D.transform.SetParent(attachmentsParent);
    }
}
