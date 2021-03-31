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
            Ragdoll ragdoll = other.rigidbody.GetComponentInParent<Ragdoll>();
            if (ragdoll != null)
            {
                ragdoll.Die();
            }
        }
        catch
        {

        }

        try
        {
            Rigidbody rb = other.rigidbody.GetComponent<Rigidbody>();

            if (rb != null)
            {
                //Appliquer un velocite aux objets
                rb.AddExplosionForce(10f, rb.position, 1f, 2f, ForceMode.Impulse);
            }
        }
        catch
        {

        }
        

        Destroy(gameObject);
    }
}
