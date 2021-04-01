using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    private Rigidbody rb;
    public Transform lookTarget;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        lookTarget = FindObjectOfType<RbCharacterMovements>().transform;
    }

    private void Start()
    {
        transform.LookAt(lookTarget);
    }


    // Update is called once per frame
    private void FixedUpdate()
    {
        rb.transform.position += rb.transform.forward * Time.deltaTime * 10f;
    }

    private void OnCollisionEnter(Collision other)
    {
        // Regarder pour le Ragdoll
        try
        {
            RbCharacterMovements rbCharacterMovements = other.rigidbody.GetComponentInParent<RbCharacterMovements>();
            if (rbCharacterMovements != null)
            {
                rbCharacterMovements.Die();
            }
        }
        catch
        {
            //Rien ne se passe.
        }
        

        Destroy(gameObject);
    }
}
