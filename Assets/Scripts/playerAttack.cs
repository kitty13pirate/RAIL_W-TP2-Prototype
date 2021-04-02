using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAttack : MonoBehaviour
{
    // Definie une attaque du joueur, soit un Slash ou un Stinger
    public playerAttack(float force, Vector3 center, float size)
    {
        //Recuperer tous les colliders
        Collider[] colliders = Physics.OverlapBox(center, new Vector3(size, size, size * 1.5f));
        
        //Regarde pour trouver les objets physiques
        foreach (Collider item in colliders)
        {
            // Regarder pour le Ragdoll de l'enemie
            Ragdoll ragdoll = item.GetComponentInParent<Ragdoll>();
            enemySwat enemy = item.GetComponentInParent<enemySwat>();
            if (ragdoll != null && ragdoll.tag != "Player")
            {
                ragdoll.Die();
                enemy.StartCoroutine(enemy.Die());
            }

            Rigidbody rb = item.GetComponent<Rigidbody>();

            //Pour empecher que le joueur se propulse lui meme
            if (rb != null && rb.tag != "Player")
            {
                //Appliquer un velocite aux objets
                rb.AddExplosionForce(force, center, 1f, 2f, ForceMode.Impulse);
            }
        }
    }
}
