using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float acceleration = 13f;
    public float decceleration = 16f;
    public float velpower = 0.5f;

    public movementStates currentState;
    public movementStates prevState;

    [Header("Jumping")]
    public float exitForce = 10f;
    public LayerMask groundMask;
    bool isGrounded;

    Rigidbody2D rb;
    Vector2 input;
    Vector2 initialVelocity;
    bool canMove;

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
        isGrounded = Physics2D.OverlapCircle(transform.position, 0.5f, groundMask);

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
        rb.AddForce(initialVelocity * exitForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(.2f);
        canMove = true;
    }
}
