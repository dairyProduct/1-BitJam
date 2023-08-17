using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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

    AudioSource audioSource;

    [Header("Jumping")]
    public float enterForce = 7f;
    public float exitForce = 15f;
    public LayerMask groundMask;
    bool isGrounded;

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

    [Header("Light")]
    public float detectionSpeed = 1f;
    public float lightExposure;
    public float maxLightExposure = 100f;
    public bool inLight;
    public callback lightUpdate;


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

    [HideInInspector]
    public GameController gameController;
    [HideInInspector]
    public Rigidbody2D rb;

    public enum movementStates {
        inWall,
        falling,
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        //lightUpdate();
    }

    void Update()
    {
        if(died) return;
        isGrounded = Physics2D.OverlapCircle(transform.position, playerSize, groundMask);

        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        input.Normalize();

        prevState = currentState;

        //Light exposure
        if(inLight) {
            lightExposure += detectionSpeed;
            lightExposure = Mathf.Clamp(lightExposure, 0f, maxLightExposure);
            lightUpdate();
        } else if(!inLight && isGrounded){
            lightExposure -= detectionSpeed;
            lightExposure = Mathf.Clamp(lightExposure, 0f, maxLightExposure);
            //lightUpdate();
        }
        if(lightExposure >= maxLightExposure) {
            died = true;
            StartCoroutine(PlayerDeath());
        }


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
        if(Input.GetButtonDown("Jump") && !isGrounded && canDash) {
            StartCoroutine(Dash());
            playerSparkleParticles.Stop();
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
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(!canDie || died) return;
        if(other.tag == "Death") {
            StartCoroutine(PlayerDeath());
        }
    }

    IEnumerator EnterWall() { //Enter Wall Force
        canMove = false;
        if(input.magnitude != 0) {
            rb.AddForce(input * enterForce, ForceMode2D.Impulse);
        } else {
            rb.AddForce(lastAirVelocity.normalized * enterForce, ForceMode2D.Impulse);
        }
        yield return new WaitForSeconds(.1f);
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
        isDashing = true;
        canDash = false;
        canMove = false;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        rb.AddForce(input * dashForce, ForceMode2D.Impulse);
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
        if(died) yield return null;
        died = true;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<TrailRenderer>().enabled = false;
        deathParticles1.Play();
        deathParticles2.Play();
        audioSource.PlayOneShot(death);

        yield return new WaitForSeconds(1f);
        
        //transform.position = gameController.lastCheckPoint;
        //lightExposure = 0;
        //lightUpdate();
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<TrailRenderer>().enabled = true;
        SceneManager.LoadScene("Main_01");
        died = false;
        
    }

    public void DashReset() {
        canDash = true;
    }


}
