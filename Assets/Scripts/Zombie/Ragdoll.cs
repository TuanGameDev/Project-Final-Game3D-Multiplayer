using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    [SerializeField] private Rigidbody[] rigidbodies; 
    [SerializeField] private Collider[] colliders; 
    [SerializeField] private Animator animator; 
    [SerializeField] private bool isRagdollEnabled = false; 

    [SerializeField] private float forceMultiplier = 10.0f; 

    private void Awake()
    {
        
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = true;
        }

        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
    }

    public void EnableRagdoll()
    {
        isRagdollEnabled = true;
        animator.enabled = false; 

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        foreach (Collider collider in colliders)
        {
            collider.enabled = true;
        }
    }

    private void Update()
    {
        
        if (isRagdollEnabled)
        {
          
            Vector3 forceDirection = Vector3.zero;

            
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    forceDirection = hit.point - transform.position;
                    forceDirection = forceDirection.normalized;
                }
            }

            
            Rigidbody torsoRb = rigidbodies[0]; 
            torsoRb.AddForce(forceDirection * forceMultiplier, ForceMode.Impulse);
        }
    }
}