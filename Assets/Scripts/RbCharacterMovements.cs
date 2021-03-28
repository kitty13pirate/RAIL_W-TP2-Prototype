using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RbCharacterMovements : MonoBehaviour
{
    private Transform transform;
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
     

    private Rigidbody rb;

    private Animator animatorPlayerCharacter;

    bool isMoving;
    bool isStinger;

    private float speed = 0.1f;
    private float animationSpeed = 1f;
    private float lerpSpeed = 0.08f;

    // Start is called before the first frame update
    void Awake()
    {
        transform = gameObject.transform;
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

        //Verifier les deadzones

        isMoving = Mathf.Abs(inputHorizontal) + Mathf.Abs(inputVertical) > 0f;

        animatorPlayerCharacter.SetBool("isMoving", isMoving);

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

        animatorPlayerCharacter.SetFloat("Horizontal", inputHorizontal * animationSpeed);
        animatorPlayerCharacter.SetFloat("Vertical", inputVertical * animationSpeed);
        //----------------------------------------------

        // Vecteur de mouvements (Avant/arrière + Gauche/Droite)
        // Empeche le personnage d'aller dans d'autres directions que par l'avant durant le stinger
        if (isStinger)
        {
            moveDirection = transform.forward * 1;
        }
        else
        {
            moveDirection = transform.forward * inputVertical + transform.right * inputHorizontal;
        } 
        
        // L'attaque de base et le stinger respectivement
        if (Input.GetButtonDown("Jump"))
        {
            teleport();
        }
        if (Input.GetMouseButtonDown(0))
        {
            animatorPlayerCharacter.SetTrigger("SlashAttack");
        }
        if (Input.GetMouseButtonDown(1))
        {
            animatorPlayerCharacter.SetTrigger("StingerAttack");
        }
    }

    private void FixedUpdate()
    {
        // Déplacer le personnage selon le vecteur de direction
        rb.MovePosition(rb.position + moveDirection.normalized * speed * Time.fixedDeltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(hitBox.position, Vector3.one * SlashAttackSize);//new Vector3(SlashAttackSize, SlashAttackSize, SlashAttackSize));
    }

    public void stingerToggle(float stinger)
    {
        if (stinger == 0)
            isStinger = true;
        else
            isStinger = false;
    }

    public void slashAttack()
    {
        playerAttack playerAttack = new playerAttack(20f, hitBox.position, SlashAttackSize);
    }

    public void teleport()
    {
        RaycastHit hit;
        Vector3 teleportTarget = rb.position + moveDirection.normalized * teleportDistance;
        if (Physics.Raycast(rb.position, moveDirection.normalized, out hit, teleportDistance))
        {
            teleportTarget = hit.point;
            teleportTarget = Vector3.Lerp(rb.position, teleportTarget, .8f);
        }

        rb.position = teleportTarget;
        
    }
}
