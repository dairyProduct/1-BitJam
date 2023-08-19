using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float wallSurfSpeed = 5f;
    public float playerSize = .3f;

    public float airAcceleration = 13f;
    public float airDecceleration = 0f;
    public float wallAcceleration = 13f;
    public float wallDecceleration = 0f;
    public float velPower = .95f;
    public float maxSpeed = 24f;

    [Header("Sounds")]
    public AudioClip dash;
    public AudioClip enterWall;
    public AudioClip exitWall;
    public AudioClip death;
    public AudioClip dashRecover;

    AudioSource audioSource;

    [Header("Jumping")]
    public float enterForce = 7f;
    public float exitForce = 15f;
    public LayerMask groundMask;
    public bool isGrounded;

    [Header("Dash")]
    public float dashForce = 8f;
    public float dashTime = 0.5f;

    [Header("Particles")]
    public ParticleSystem deathParticles1;
    public ParticleSystem deathParticles2;
    public ParticleSystem playerSparkleParticles;
    public GameObject EnterParticles;
    public GameObject dashParticles1, dashParticles2;
    
    //public ParticleSystem deathParticles3;

    [Header("References")]
    private CameraShake shake;
    private Animator animator;

    [Header("Private")]
    private movementStates currentState;
    private movementStates prevState;
    
    private Vector2 input;
    private Vector2 lastGroundedVelocity;
    private Vector2 lastAirVelocity;
    private bool canMove;
    private bool canDie;
    private bool died = false;
    private bool canDash = true;
    public bool isDashing;
    public delegate void callback();
    private Coroutine dashRoutine;
    private bool usingKeyboard = true;

    [HideInInspector]
    public GameController gameController;
    [HideInInspector]
    public Rigidbody2D rb;
    Vector2 lastInput;
    private int scoreMultiplier = 1;


    public enum movementStates {
        inWall,
        falling,
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        shake = FindObjectOfType<CameraShake>();
        animator = GetComponent<Animator>();
        if(PlayerPrefs.GetInt("Controls") == 0) {
            SetControls(true);
        } else {
            SetControls(false);
        }
        
    }

    void Update()
    {
        if(died) return;
        isGrounded = Physics2D.OverlapCircle(transform.position, playerSize, groundMask);

        if(usingKeyboard) {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
            input.Normalize();

            if(input.magnitude != 0) {
            lastInput = input;
            }
        } else {
            Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            lastInput = direction.normalized;
            if(Input.GetKey(KeyCode.Mouse0)) {
                input = direction.normalized;
            } else {
                input = Vector2.zero;
            }
        }
        

        animator.SetBool("CanDash", canDash);
        

        prevState = currentState;

        //Grounded or ungrounded states
        if(isGrounded) {
            rb.gravityScale = 0f;
            currentState = movementStates.inWall;
            canDie = false;
            lastGroundedVelocity = rb.velocity;
        } else if(!isDashing) {
            rb.gravityScale = 1f;
            currentState = movementStates.falling;
            lastAirVelocity = rb.velocity;
        }

        //Dash Input
        if(usingKeyboard) {
            if(Input.GetButtonDown("Jump") && !isGrounded && canDash) {
            if(dashRoutine != null) {
                StopCoroutine(dashRoutine);
            }
            dashRoutine = StartCoroutine(Dash());
            playerSparkleParticles.Stop();
        }
        } else {
            if(Input.GetKeyDown(KeyCode.Mouse1) && !isGrounded && canDash) {
            if(dashRoutine != null) {
                StopCoroutine(dashRoutine);
            }
            dashRoutine = StartCoroutine(Dash());
            playerSparkleParticles.Stop();
        }
        }
        

        //Adds force when Exiting or entering a Wall
        if(prevState == movementStates.falling && currentState == movementStates.inWall) {
            StartCoroutine(EnterWall());
            audioSource.PlayOneShot(enterWall);
            canDash = true;
            playerSparkleParticles.Play();
            Instantiate(EnterParticles, transform.position, Quaternion.identity);
        }
        if(prevState == movementStates.inWall && currentState == movementStates.falling) {
            StartCoroutine(ExitWall());
            audioSource.PlayOneShot(exitWall);
        }

        //Clamp Max Speed
        Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed);
        Mathf.Clamp(rb.velocity.y, -maxSpeed, maxSpeed);
    }

    private void FixedUpdate() {
        if(died || !canMove) return;
        if(isGrounded) {
            //Wall Surfing Movement
            Vector2 targetSpeed = input * wallSurfSpeed;
            Vector2 speedDiff = targetSpeed - rb.velocity;
            Vector2 accelRate = new Vector2(Mathf.Abs(targetSpeed.x) > 0.01f ? wallAcceleration : wallDecceleration, Mathf.Abs(targetSpeed.y) > 0.01f ? wallAcceleration : wallDecceleration);
            Vector2 movement = new Vector2(Mathf.Pow(Mathf.Abs(speedDiff.x) * accelRate.x, velPower) * Mathf.Sign(speedDiff.x), Mathf.Pow(Mathf.Abs(speedDiff.y) * accelRate.y, velPower) * Mathf.Sign(speedDiff.y));
            rb.AddForce(movement);
        } else {
            //Falling Movement
            float targetSpeed = input.x * wallSurfSpeed;
            float speedDiff = targetSpeed - rb.velocity.x;
            float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? airAcceleration : airDecceleration;
            float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, velPower) * Mathf.Sign(speedDiff);
            rb.AddForce(movement * Vector2.right);
        }
        float angle = Mathf.Atan2(lastInput.y, lastInput.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = rotation;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(!canDie || died) return;
        if(other.tag == "Death") {
            StartCoroutine(PlayerDeath());
        }
    }

    IEnumerator EnterWall() { //Enter Wall Force
        scoreMultiplier = 1;
        gameController.gameObject.GetComponent<UIController>().UpdateScoreMultiplier(scoreMultiplier);
        canMove = false;
        rb.AddForce(lastAirVelocity.normalized * enterForce, ForceMode2D.Impulse);
        //rb.AddForce(input * enterForce, ForceMode2D.Impulse);
        /*if(input.magnitude != 0) {
            rb.AddForce(input * enterForce, ForceMode2D.Impulse);
        } else {
            rb.AddForce(lastAirVelocity.normalized * enterForce, ForceMode2D.Impulse);
        }
        */
        yield return new WaitForSeconds(.1f);
        //rb.velocity = Vector2.zero; Remove Commenet to make lose momentum on enter wall
        canMove = true;
    }

    IEnumerator ExitWall() { //Exit Wall Force
        canDie = false;
        rb.AddForce(lastGroundedVelocity.normalized * exitForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(.1f);
        canDie = true;
    }

    IEnumerator Dash() {
        Instantiate(dashParticles1, transform.position, Quaternion.identity);
        Instantiate(dashParticles2, transform.position, Quaternion.identity);
        shake.StartCoroutine(shake.Shake(.1f, .25f));
        animator.SetTrigger("Dash");
        isDashing = true;
        canDash = false;
        canMove = false;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        rb.AddForce(lastInput * dashForce, ForceMode2D.Impulse);
        audioSource.PlayOneShot(dash);
        yield return new WaitForSeconds(dashTime);
        rb.velocity = input * wallSurfSpeed;
        if(!isGrounded) {
            rb.gravityScale = 1;
        }
        
        canMove = true;
        yield return new WaitForSeconds(0.33f); //invinsability frames
        isDashing = false;
    }

    public IEnumerator PlayerDeath(){
        if(died || isDashing) yield break;
        gameController.GameOver();
        died = true;
        shake.StartCoroutine(shake.Shake(.1f, .5f));
        
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<TrailRenderer>().enabled = false;
        deathParticles1.Play();
        deathParticles2.Play();
        audioSource.PlayOneShot(death);

        yield return new WaitForSeconds(1f);
        gameController.gameObject.GetComponent<UIController>().PlayFadeOut();
        yield return new WaitForSeconds(4f);
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<TrailRenderer>().enabled = true;
        SceneManager.LoadScene("Main_01"); //resets the level
        died = false;
        
    }

    public IEnumerator PlayerStopMovementForTime(float duration) {
        if(dashRoutine != null) {
            StopCoroutine(dashRoutine);
        }
        canMove = false;
        yield return new WaitForSeconds(duration);
        rb.velocity = Vector2.zero;
        canMove = true;
    }

    public void DashReset(bool increaseMultiplier) {
        if(increaseMultiplier) {
            scoreMultiplier += 1;
        }
        audioSource.PlayOneShot(dashRecover);
        gameController.gameObject.GetComponent<UIController>().UpdateScoreMultiplier(scoreMultiplier);
        canDash = true;
    }

    public void IncreaseScore(int amount){
        gameController.gameObject.GetComponent<UIController>().UpdateScoreUI(amount);
    }
    public void SetControls(bool keyboard) {
        if(keyboard) {
            usingKeyboard = true;
        } else {
            usingKeyboard = false;
        }
    }


}
