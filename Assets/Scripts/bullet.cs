using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    private Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.position += transform.forward * Time.deltaTime * 5f;
    }

    private void OnCollisionEnter(Collision other)
    {
        // Regarder pour le Ragdoll
        Ragdoll ragdoll = other.rigidbody.GetComponentInParent<Ragdoll>();
        if (ragdoll != null)
        {
            ragdoll.Die();
        }

        Rigidbody rb = other.rigidbody.GetComponent<Rigidbody>();

        if (rb != null)
        {
            //Appliquer un velocite aux objets
            rb.AddExplosionForce(10f, rb.position, 1f, 2f, ForceMode.Impulse);
        }

        Destroy(gameObject);
    }
}
