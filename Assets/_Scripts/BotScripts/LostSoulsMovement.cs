using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostSoulsMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float groundMoveSpeed = 5f;

    public GameObject deathParticles;
    Rigidbody2D rb;
    PlayerController player;

    private bool isGrounded;
    public LayerMask groundMask;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(transform.position, .25f, groundMask);

        if(isGrounded) {
            rb.velocity = (player.transform.position - transform.position).normalized * groundMoveSpeed;
        } else {
            rb.velocity = (player.transform.position - transform.position).normalized * moveSpeed;
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            if(player.isDashing) {
                Instantiate(deathParticles, transform.position, Quaternion.identity);
                Destroy(gameObject);
            } else {
                player.StartCoroutine(player.PlayerDeath());
                Destroy(gameObject);
            }
        }
    }

}
