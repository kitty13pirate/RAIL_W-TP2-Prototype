using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion
{
    public Explosion(float force, Vector3 position, float radius, float michaelBay)
    {
        //Recuperer tous les colliders a proximite
        Collider[] colliders = Physics.OverlapSphere(position, radius);

        //Regarde pour trouver les objets physiques
        foreach (Collider item in colliders)
        {
            // Regarder pour le Ragdoll
            Ragdoll ragdoll = item.GetComponentInParent<Ragdoll>();
            if (ragdoll != null)
            {
                ragdoll.Die();
            }

            Rigidbody rb = item.GetComponent<Rigidbody>();

            if (rb != null)
            {
                //Appliquer un velocite aux objets
                rb.AddExplosionForce(force, position, radius, michaelBay, ForceMode.Impulse);
            }
        }
    }
}
