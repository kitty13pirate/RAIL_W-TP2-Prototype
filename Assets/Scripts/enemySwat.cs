using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemySwat : MonoBehaviour
{
    private Rigidbody rb;
    private Animator enemyAnimator;
    NavMeshAgent NavMeshAgent;
    float speedWalking = 4f;
    bool isAgentBusy = false;
    Vector3 destination;
    Vector3 direction;
    public Transform lookTarget;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(lookTarget);

        if (isAgentBusy == false)
            destination = GetRandomDestination();
            StartCoroutine(patrol(destination, speedWalking));

        direction = (rb.position - destination).normalized;

        enemyAnimator.SetFloat("Horizontal", NavMeshAgent.velocity.x);
        enemyAnimator.SetFloat("Vertical", NavMeshAgent.velocity.z);
    }

    //Retourne un coordonnes dans les limites de la scene
    Vector3 GetRandomDestination()
    {
        float xLimit = Random.Range(-11f, 7f);
        float zLimit = Random.Range(-11f, 11f);

        return new Vector3(xLimit, 0f, zLimit);
    }

    //Coroutine
    IEnumerator patrol(Vector3 destination, float speed)
    {
        isAgentBusy = true;

        //modifier la vitesse du pnj
        NavMeshAgent.speed = speed;

        // Me deplacer vers la destination
        NavMeshAgent.SetDestination(destination);

        // Tant que je ne suis pas rendu a la destination je ne fais rien d'autre
        while (NavMeshAgent.pathPending | NavMeshAgent.remainingDistance > 0.5f)
        {
            yield return null;
        }

        // Rendu a destination, je prend une pause (bien meritee)
        yield return new WaitForSeconds(2f);

        // Je demarre une nouvelle patrouille
        isAgentBusy = false;
    }
}
