using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RbCharacterMovements : MonoBehaviour
{

    public float speedWalking;
    public float speedRunning;
    
    public float jumpHeight = 1f;

    // Transform de la position des pieds
    public Transform feetPosition;

    private float inputVertical;
    private float inputHorizontal;    

    private Vector3 moveDirection;

    private Rigidbody rb;

    private bool isGrounded = true;

    private Animator animatorPlayerCharacter;

    bool isMoving;

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
        // Vérifier si l'on touche le sol
        isGrounded = Physics.CheckSphere(feetPosition.position, 0.15f, 1, QueryTriggerInteraction.Ignore);

        // Vérifier les inputs du joueur
        // Vertical (W, S et Joystick avant/arrière)
        inputVertical = Input.GetAxis("Vertical");
        // Horizontal (A, D et Joystick gauche/droite)
        inputHorizontal = Input.GetAxis("Horizontal");

        //Verifier les deadzones

        isMoving = Mathf.Abs(inputHorizontal) + Mathf.Abs(inputVertical) > 0f;

        animatorPlayerCharacter.SetBool("isMoving", isMoving);

        // Animation -----------------------------------
        if (Input.GetKey(KeyCode.LeftShift))
        {
            animationSpeed = Mathf.Lerp(animationSpeed, 1f, lerpSpeed);
            speed = Mathf.Lerp(speed, speedRunning, lerpSpeed);
        }
        else
        {
            animationSpeed = Mathf.Lerp(animationSpeed, 2f, lerpSpeed);
            speed = Mathf.Lerp(speed, speedWalking, lerpSpeed);
        }

        animatorPlayerCharacter.SetFloat("Horizontal", inputHorizontal * animationSpeed);
        animatorPlayerCharacter.SetFloat("Vertical", inputVertical * animationSpeed);
        //----------------------------------------------

        // Vecteur de mouvements (Avant/arrière + Gauche/Droite)
        moveDirection = transform.forward * inputVertical + transform.right * inputHorizontal;  
        
        // Sauter
        if (Input.GetButtonDown("Jump") && isGrounded == true)
        {
            animatorPlayerCharacter.SetTrigger("Jump");
            rb.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
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
}
