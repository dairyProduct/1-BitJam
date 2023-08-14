using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float wallSurfSpeed = 5f;
    public float playerSize = .3f;

    public float acceleration = 13f;
    public float decceleration = 0f;
    public float velPower = .95f;
    public float maxSpeed = 24f;

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
    //public ParticleSystem deathParticles3;


    [Header("Private")]
    private movementStates currentState;
    private movementStates prevState;
    private Rigidbody2D rb;
    private Vector2 input;
    private Vector2 lastGroundedVelocity;
    private Vector2 lastAirVelocity;
    private bool canMove;
    private bool canDie;
    private bool died = false;
    private bool canDash = true;
    private bool isDashing;

    [HideInInspector]
    public GameController gameController;

    public enum movementStates {
        inWall,
        falling,
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(died) return;
        isGrounded = Physics2D.OverlapCircle(transform.position, playerSize, groundMask);

        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        input.Normalize();

        prevState = currentState;

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
        }

        //Adds force when Exiting or entering a Wall
        if(prevState == movementStates.falling && currentState == movementStates.inWall) {
            StartCoroutine(EnterWall());
            canDash = true;
        }
        if(prevState == movementStates.inWall && currentState == movementStates.falling) {
            StartCoroutine(ExitWall());
        }

        //Clamp Max Speed
        Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed);
        Mathf.Clamp(rb.velocity.y, -maxSpeed, maxSpeed);
    }

    private void FixedUpdate() {
        if(died) return;
        if(isGrounded) {
            //Wall Surfing Movement
            Vector2 targetSpeed = input * wallSurfSpeed;
            Vector2 speedDiff = targetSpeed - rb.velocity;
            Vector2 accelRate = new Vector2(Mathf.Abs(targetSpeed.x) > 0.01f ? acceleration : decceleration, Mathf.Abs(targetSpeed.y) > 0.01f ? acceleration : decceleration);
            Vector2 movement = new Vector2(Mathf.Pow(Mathf.Abs(speedDiff.x) * accelRate.x, velPower) * Mathf.Sign(speedDiff.x), Mathf.Pow(Mathf.Abs(speedDiff.y) * accelRate.y, velPower) * Mathf.Sign(speedDiff.y));
            rb.AddForce(movement);
        } else {
            //Falling Movement
            float targetSpeed = input.x * wallSurfSpeed;
            float speedDiff = targetSpeed - rb.velocity.x;
            float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : decceleration;
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

    private void PlayDeathParticles(){
        deathParticles1.Play();
        deathParticles2.Play();
        //deathParticles3.Play();
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
        isDashing = true;
        canDash = false;
        canMove = false;
        rb.gravityScale = 0;
        rb.AddForce(input * dashForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(dashTime);
        if(!isGrounded) {
            rb.gravityScale = 1;
        }
        isDashing = false;
        canMove = true;
    }

    private IEnumerator PlayerDeath(){
        
        died = true;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        yield return new WaitForSeconds(0.5f);

        PlayDeathParticles();
        yield return new WaitForSeconds(0.5f);

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<TrailRenderer>().enabled = false;
        yield return new WaitForSeconds(1f);

        
        transform.position = gameController.lastCheckPoint;
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<TrailRenderer>().enabled = true;
        died = false;
        
    }


}
