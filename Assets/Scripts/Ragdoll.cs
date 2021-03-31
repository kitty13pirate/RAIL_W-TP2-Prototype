using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    Rigidbody rb;
    Rigidbody[] ragdollRBs;
    Animator animator;
    bool isDead;

    public bool debugKill;

    // Start is called before the first frame update
    void Awake()
    {
        //Lister tous les Rbs
        rb = GetComponent<Rigidbody>();
        ragdollRBs = GetComponentsInChildren<Rigidbody>();

        animator = GetComponent<Animator>();

        //Desactiver le Ragdoll
        toggleRagdoll(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (debugKill)
        {
            Die();
            debugKill = false;
        }   
    }

    public void Die()
    {
        if (isDead)
            return;
        else
        {
            toggleRagdoll(true);
            isDead = true;
        }
    }

    void toggleRagdoll(bool value)
    {
        //Mettre le Kinematic a !value
        foreach (Rigidbody rb in ragdollRBs)
        {
            rb.isKinematic = !value;
        }

        // Animator
        animator.enabled = !value;
    }
}
