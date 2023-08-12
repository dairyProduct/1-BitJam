using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public enum movementStates {
        inWall,
        falling
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
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
            
        } else {
            rb.gravityScale = 1f;
            currentState = movementStates.falling;
        }

        if(prevState == movementStates.falling && currentState == movementStates.inWall) {
            StartCoroutine(EnterWall());
        }
        if(prevState == movementStates.inWall && currentState == movementStates.falling) {
            rb.AddForce(initialVelocity.normalized * exitForce, ForceMode2D.Impulse);
        }
    }

    private void FixedUpdate() {
        if(!canMove) return;
        if(isGrounded) {
            rb.velocity = new Vector2(input.x, input.y) * speed;
        } else {
            rb.velocity = new Vector2(input.x * speed, rb.velocity.y);
        }
    }

    IEnumerator EnterWall() {
        canMove = false;
        if(input.magnitude > 0) {
            rb.AddForce(input * enterForce, ForceMode2D.Impulse);
        } else {
            rb.AddForce(initialVelocity.normalized * enterForce, ForceMode2D.Impulse);
        }
        
        yield return new WaitForSeconds(.1f);
        canMove = true;
    }
}
