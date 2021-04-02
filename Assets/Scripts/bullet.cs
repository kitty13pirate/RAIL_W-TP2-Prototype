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

    //La trajectoire est l'emplacement initial du joueur lorsque la balle est instanciee
    private void Start()
    {
        transform.LookAt(lookTarget);
    }


    // Update is called once per frame
    private void FixedUpdate()
    {
        rb.transform.position += rb.transform.forward * Time.deltaTime * 10f;
    }

    //Appelle lorsque la balle touche quelque chose
    private void OnCollisionEnter(Collision other)
    {
        // Regarder si le script du joueur existe
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
            //Rien ne se passe. Le catch est seulement ici pour eviter les erreurs
        }
        
        //La balle est detruite lors d'une collision
        Destroy(gameObject);
    }
}
