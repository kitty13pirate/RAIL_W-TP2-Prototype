using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemySwat : MonoBehaviour
{
    private bool Alerted;
    public bool isDead;
    private Rigidbody rb;
    private Animator enemyAnimator;
    NavMeshAgent NavMeshAgent;
    float speedWalking = 4f;
    bool isAgentBusy = false;
    Vector3 destination;
    public Transform lookTarget;
    public GameObject bulletPrefab;
    public Transform barrel;
    public ParticleSystem psMuzzleFlash;
    public AudioSource audio;
    public AudioClip clip;
    public AudioClip deathClip;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponent<Animator>();

        lookTarget = FindObjectOfType<RbCharacterMovements>().transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("delayedUpdate", 1f, 1f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Pour qu'il regarde toujours le joueur
        if (!isDead)
        {
            transform.LookAt(lookTarget);
        }

        if (isAgentBusy == false && !isDead)
        {
            destination = GetRandomDestination();
            StartCoroutine(reposition(destination, speedWalking));
        }
            
        //Les variables pour son animation
        enemyAnimator.SetFloat("Horizontal", NavMeshAgent.velocity.x);
        enemyAnimator.SetFloat("Vertical", NavMeshAgent.velocity.z);
    }

    //Retourne un coordonnes dans les limites de la scene
    Vector3 GetRandomDestination()
    {
        float xLimit = Random.Range(rb.position.x -4f, rb.position.x +4f);
        float zLimit = Random.Range(rb.position.y - 4f, rb.position.y + 4f);

        return new Vector3(xLimit, 0f, zLimit);
    }

    //Coroutine
    IEnumerator reposition(Vector3 destination, float speed)
    {
        if (!isDead)
        {
            isAgentBusy = true;

            //modifier la vitesse du pnj
            NavMeshAgent.speed = speed;

            // Se deplacer vers la destination
            NavMeshAgent.SetDestination(destination);

            // Tant que l'enemie n'est pas rendu a la destination il ne fais rien d'autre
            while (NavMeshAgent.pathPending && !isDead | NavMeshAgent.remainingDistance > 0.5f && !isDead)
            {
                yield return null;
            }

            // Rendu a destination, il prend une pause avant de tirer
            yield return new WaitForSeconds(Random.Range(2f, 3.5f));
            if (Alerted)
            {
                enemyAnimator.SetTrigger("Fire");
            }
            // Il se repositionne pour un nouveau tir
            isAgentBusy = false;
        }
    }

    //Il tente de detecter le joueur
    void tryDetectPlayer()
    {
        //Creer un rayon 
        RaycastHit hit;

        //S'il est obstrue par un collider autre que le personnage
        if (Physics.Linecast(rb.position, lookTarget.position, out hit))
        {
            // verification a savoir s'il s'agit du joueur
            if (hit.collider.CompareTag("Player"))
            {
                Alerted = true;
            }
            //Sinon, il tente de retrouver le joueur
            else
            {
                Alerted = false;
                reposition(lookTarget.position, speedWalking);
            }
        }
    }

    //La fonction pour tirer, instancie une balle en direction du joueur
    public void fireRifle()
    {
        audio.PlayOneShot(clip);
        psMuzzleFlash.Emit(1);
        barrel.transform.LookAt(lookTarget);
        GameObject firedBullet = Instantiate(bulletPrefab, barrel.position, Quaternion.identity);
    }

    //sera appele 1x/sec
    private void delayedUpdate()
    {
        tryDetectPlayer();

    }

    //L'enemie meurt
    public IEnumerator Die()
    {
        if (!isDead)
        {
            //Il crie avant d'etre detruit
            audio.PlayOneShot(deathClip);
            isDead = true;
            NavMeshAgent.enabled = false;
            yield return new WaitForSeconds(10f);
            Destroy(gameObject);
        }
            
    }
}
