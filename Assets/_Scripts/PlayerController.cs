using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;

    [Header("Jumping")]
    public float exitForce = 10f;
    public LayerMask groundMask;
    bool isGrounded;

    Rigidbody2D rb;
    Vector2 movement;

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

        rb.gravityScale = isGrounded ? 0 : 1; //Changes gravity when player is in the air or ground
    }

    private void FixedUpdate() {
        //rb.velocity = isGrounded ? new Vector2(movement.x, movement.y) * speed : new Vector2(movement.x * speed, rb.velocity.y);
    }
}
