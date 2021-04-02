using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RbCharacterMovements : MonoBehaviour
{
    public GameObject body;

    //Les vitesses pour la marche, le running et l'attaque stinger respectivement
    public float speedWalking;
    public float speedRunning;
    public float speedStinger;
    public float teleportDistance;

    //Pour verifier les inputs
    private float inputVertical;
    private float inputHorizontal;    

    //La direction du mouvement
    private Vector3 moveDirection;

    //Le centre de la hitbox des attaques
    public Transform hitBox;

    //La taille des attaques
    public float StingerAttackSize;
    public float SlashAttackSize;

    //Tous les Particle Systems
    public ParticleSystem psSmoke;
    public ParticleSystem psExplosion;
    public ParticleSystem psShieldRecharge;
    public ParticleSystem psShieldBroken;
    public ParticleSystem psFlameAttack;

    //Tout l'audio
    public AudioSource audioSource;
    public AudioClip clipFlameAttack;
    public AudioClip clipShieldRecharge;
    public AudioClip clipShieldBroken;
    public AudioClip clipExplosion;
    public AudioClip clipSmoke;

    //Le rigidbody
    private Rigidbody rb;

    //L'animator
    private Animator animatorPlayerCharacter;

    // les booleens pour le stinger et les habilites
    private bool isStinger;
    private bool shieldCooldown = false;
    private bool stingerCooldown = false;
    private bool teleportCooldown = false;

    // les vitesses pour les animations
    private float speed = 0.1f;
    private float animationSpeed = 1f;
    private float lerpSpeed = 0.08f;

    // Start is called before the first frame update
    void Awake()
    {
        // Assigner le Rigidbody
        rb = GetComponent<Rigidbody>();
        animatorPlayerCharacter = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        // Vérifier les inputs du joueur
        // Vertical (W, S et Joystick avant/arrière)
        inputVertical = Input.GetAxis("Vertical");
        // Horizontal (A, D et Joystick gauche/droite)
        inputHorizontal = Input.GetAxis("Horizontal");

        // Animation -----------------------------------
        //Par defaut, le personnage cour, shift permet alors de marcher
        if (Input.GetKey(KeyCode.LeftShift))
        {
            animationSpeed = Mathf.Lerp(animationSpeed, 1f, lerpSpeed);
            speed = Mathf.Lerp(speed, speedWalking, lerpSpeed);
        }

        //Lors de l'attaque stinger, le personnage se deplace plus rapidement et attaque continuellement
        else if (isStinger)
        {
            speed = Mathf.Lerp(speed, speedStinger, lerpSpeed);
            playerAttack playerAttack = new playerAttack(1f, hitBox.position, StingerAttackSize);
        }
        
        //Vitesse habituelle
        else
        {
            animationSpeed = Mathf.Lerp(animationSpeed, 2f, lerpSpeed);
            speed = Mathf.Lerp(speed, speedRunning, lerpSpeed);
        }

        //Les variables pour les animations
        animatorPlayerCharacter.SetFloat("Horizontal", inputHorizontal * animationSpeed);
        animatorPlayerCharacter.SetFloat("Vertical", inputVertical * animationSpeed);
        //----------------------------------------------

        // Vecteur de mouvements (Avant/arrière + Gauche/Droite)
        // Empeche le personnage d'aller dans d'autres directions que par l'avant durant le stinger
        if (isStinger)
        {
            moveDirection = transform.forward * 1;
        }
        //Sinon le deplacement est normal
        else
        {
            moveDirection = transform.forward * inputVertical + transform.right * inputHorizontal;
        } 
        
        // les touches pour la teleportation, l'attaque de base et le stinger respectivement
        if (Input.GetButtonDown("Jump") && !teleportCooldown)
        {
            teleport();
        }
        if (Input.GetMouseButtonDown(0))
        {
            animatorPlayerCharacter.SetTrigger("SlashAttack");
        }
        if (Input.GetMouseButtonDown(1) && !stingerCooldown)
        {
            animatorPlayerCharacter.SetTrigger("StingerAttack");
        }
    }

    private void FixedUpdate()
    {
        // Déplacer le personnage selon le vecteur de direction
        rb.MovePosition(rb.position + moveDirection.normalized * speed * Time.fixedDeltaTime);
    }

    //Pour le debugage/voir plus clairement les hitboxes
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(hitBox.position, Vector3.one * SlashAttackSize);
    }

    //Indique si le personnage est en attaque stinger
    public void stingerToggle(float stinger)
    {
        if (stinger == 0)
        {
            isStinger = true;
            stingerCooldown = true;
        }
        else
        {
            isStinger = false;
            StartCoroutine(stingerRecharge());
        }
            
    }

    //L'attaque de base
    public void slashAttack()
    {
        playerAttack playerAttack = new playerAttack(20f, hitBox.position, SlashAttackSize);
    }

    //Un teleport, utilise un Raycast pour eviter de passer a travers les murs.
    public void teleport()
    {
        RaycastHit hit;
        Vector3 teleportTarget = rb.position + moveDirection.normalized * teleportDistance;
        if (Physics.Raycast(rb.position, moveDirection.normalized, out hit, teleportDistance))
        {
            teleportTarget = hit.point;
            teleportTarget = Vector3.Lerp(rb.position, teleportTarget, .8f);
        }

        audioSource.PlayOneShot(clipSmoke);
        psSmoke.Emit(30);
        rb.position = teleportTarget;
        teleportCooldown = true;
        StartCoroutine(teleportRecharge());
    }

    //Pour activer et desactiver les particules de flameAttack
    public void flameToggle()
    {
        if (!psFlameAttack.isPlaying)
        {
            audioSource.PlayOneShot(clipFlameAttack);
            psFlameAttack.Play();
        }
        else
            psFlameAttack.Stop();
    }

    //La fonction qui gere lorsque le personnage est touche par une attaque
    public void Die()
    {
        // Si le bouclier magique est fonctionnel, le personnage est sauve
        if (!shieldCooldown)
        {
            audioSource.PlayOneShot(clipShieldBroken);
            psShieldBroken.Emit(1);
            shieldCooldown = true;
            StartCoroutine(shieldRecharge());
        }
        // Sinon
        else
        {
            //Le personnage explose, meurt et le jeu s'arrete
            audioSource.PlayOneShot(clipExplosion);
            psExplosion.Emit(50);
            body.SetActive(false);
            GameManager.singleton.GameOver();
        }
        
    }

    //Le cooldown pour le bouclier
    private IEnumerator shieldRecharge()
    {
        yield return new WaitForSeconds(5f);
        psShieldRecharge.Emit(1);
        audioSource.PlayOneShot(clipShieldRecharge);
        shieldCooldown = false;
    }

    //Le cooldown pour le stinger
    private IEnumerator stingerRecharge()
    {
        yield return new WaitForSeconds(3f);
        stingerCooldown = false;
    }

    //Le cooldown pour la teleportation
    private IEnumerator teleportRecharge()
    {
        yield return new WaitForSeconds(2f);
        teleportCooldown = false;
    }
}
