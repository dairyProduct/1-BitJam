using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float playerSize = .3f;

    [Header("Jumping")]
    public float enterForce = 7f;
    public float exitForce = 15f;
    public LayerMask groundMask;
    bool isGrounded;

    [Header("Private")]
    private movementStates currentState;
    private movementStates prevState;
    private Rigidbody2D rb;
    private Vector2 input;
    private Vector2 initialVelocity;
    private bool canMove;
    private bool canDie;

    public enum movementStates {
        inWall,
        falling
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(transform.position, playerSize, groundMask);

        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        input.Normalize();

        prevState = currentState;

        if(isGrounded) {
            initialVelocity = rb.velocity;
            rb.gravityScale = 0f;
            currentState = movementStates.inWall;
            canDie = false;
        } else {
            rb.gravityScale = 1f;
            currentState = movementStates.falling;
        }

        //Adds force when Exiting or entering a Wall
        if(prevState == movementStates.falling && currentState == movementStates.inWall) {
            StartCoroutine(EnterWall());
        }
        if(prevState == movementStates.inWall && currentState == movementStates.falling) {
            StartCoroutine(ExitWall());
        }
    }

    private void FixedUpdate() {
        if(!canMove) return;
        if(isGrounded) {
            //Wall Surfing Movement
            rb.velocity = new Vector2(input.x, input.y) * speed;
        } else {
            //Falling
            rb.velocity = new Vector2(input.x * speed, rb.velocity.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(!canDie) return;
        SceneManager.LoadScene("DanTest"); //Debug Respawn
    }

    IEnumerator EnterWall() { //Enter Wall Force
        canMove = false;
        if(input.magnitude > 0) {
            rb.AddForce(input * enterForce, ForceMode2D.Impulse);
        } else {
            rb.AddForce(initialVelocity.normalized * enterForce, ForceMode2D.Impulse);
        }
        yield return new WaitForSeconds(.1f);
        canMove = true;
    }

    IEnumerator ExitWall() {
        canDie = false;
        rb.AddForce(initialVelocity.normalized * exitForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(.1f);
        canDie = true;
    }


}
