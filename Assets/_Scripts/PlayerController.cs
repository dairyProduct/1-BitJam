using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;

    public movementStates currentState;
    public movementStates prevState;

    [Header("Jumping")]
    public float exitForce = 10f;
    public LayerMask groundMask;
    bool isGrounded;

    Rigidbody2D rb;
    Vector2 movement;
    Vector2 initialVelocity;

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

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        prevState = currentState;

        if(isGrounded) {
            rb.gravityScale = 0f;
            rb.velocity /= 2f;
            currentState = movementStates.inWall;
        } else {
            rb.gravityScale = 1f;
            prevState = currentState;
            currentState = movementStates.falling;
        }
    }

    private void FixedUpdate() {
        rb.velocity = isGrounded ? new Vector2(movement.x, movement.y) * speed : new Vector2(movement.x * speed, rb.velocity.y);
    }
}
