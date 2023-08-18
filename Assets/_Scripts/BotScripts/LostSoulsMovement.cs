using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostSoulsMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float groundMoveSpeed = 5f;

    [Header("Sound")]
    public AudioClip chase;
    public AudioClip close;
    public AudioClip death;

    public AudioSource audioSourceLoop;
    public AudioSource audioSourceOne;

    public GameObject deathParticles;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    PlayerController player;
    CameraShake shake;

    private bool isGrounded;
    public LayerMask groundMask;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        shake = FindObjectOfType<CameraShake>();
        audioSourceLoop.clip = chase;
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null) return;
        isGrounded = Physics2D.OverlapCircle(transform.position, .25f, groundMask);

        if(isGrounded) {
            rb.velocity = (player.transform.position - transform.position).normalized * groundMoveSpeed;
            spriteRenderer.material.SetInt("_Invert", 0);
        } else {
            rb.velocity = (player.transform.position - transform.position).normalized * moveSpeed;
            spriteRenderer.material.SetInt("_Invert", 1);
        }

        Vector2 direction = (player.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 10f * Time.deltaTime);

        if(angle > 90 || angle < -90) {
            transform.localScale = new Vector3(1, -1, 1);
        } else {
            transform.localScale = new Vector3(1, 1, 1);
        }


        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            if(player.isDashing) {
                Instantiate(deathParticles, transform.position, Quaternion.identity);
                audioSourceOne.PlayOneShot(death);
                shake.StartCoroutine(shake.Shake(.1f, .5f));
                player.DashReset();
                Destroy(gameObject);
            } else {
                player.StartCoroutine(player.PlayerDeath());
                audioSourceOne.PlayOneShot(death);
                Destroy(gameObject);
            }
        }
    }

}
