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
        if (!isDead)
        {
            transform.LookAt(lookTarget);
        }

        if (isAgentBusy == false && !isDead)
        {
            destination = GetRandomDestination();
            StartCoroutine(reposition(destination, speedWalking));
        }
            

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

            // Me deplacer vers la destination
            NavMeshAgent.SetDestination(destination);

            // Tant que je ne suis pas rendu a la destination je ne fais rien d'autre
            while (NavMeshAgent.pathPending && !isDead | NavMeshAgent.remainingDistance > 0.5f && !isDead)
            {
                yield return null;
            }

            // Rendu a destination, je prend une pause avant de tirer
            yield return new WaitForSeconds(Random.Range(2f, 3.5f));
            if (Alerted)
            {
                enemyAnimator.SetTrigger("Fire");
            }
            // Je demarre une nouvelle patrouille
            isAgentBusy = false;
        }
    }

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
            else
            {
                Alerted = false;
                reposition(lookTarget.position, speedWalking);
            }
        }
    }

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

    public IEnumerator Die()
    {
        isDead = true;
        NavMeshAgent.enabled = false;
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }
}
