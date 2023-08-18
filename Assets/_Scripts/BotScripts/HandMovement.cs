using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float movementInterval = 3f;
    [SerializeField] float attackDistance = 5f;

    [Header("Particles")]
    public GameObject deathParticles;


    [SerializeField] GameObject flamePrefab;

    private PlayerController player;
    private Vector2 targetPosition;
    private Rigidbody2D rb;
    private float movementTimer;
    bool checking;

    private void Start()
    {
        movementTimer = movementInterval + 2f;
        player = FindObjectOfType<PlayerController>();
        
        rb = GetComponent<Rigidbody2D>();

        StartCoroutine(InRange());
    }

    private void Update()
    {

    }

    private void ChooseRandomTargetPosition()
    {
        float x = Random.Range(-5f, 5f);
        float y = Random.Range(-3f, 3f);
        targetPosition = new Vector2(x, y);
    }

    private void MoveToTargetPosition()
    {
        Vector2 direction = (targetPosition - rb.position).normalized;
        Vector2 movement = direction * moveSpeed * Time.deltaTime;

        rb.MovePosition(rb.position + movement);
    }

    private void HandAttack(){
        //play attackAnimation
        
    }

    IEnumerator InRange() {
        while(true) {
            if(Vector2.Distance(transform.position, player.transform.position) <= attackDistance ) {
                GameObject flame = Instantiate(flamePrefab, transform);
                yield return new WaitForSeconds(1f);
            }
            ChooseRandomTargetPosition();
            MoveToTargetPosition();
            yield return new WaitForSeconds(Random.Range(0.2f, 2f));
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            if(player.isDashing) {
                Instantiate(deathParticles, transform.position, Quaternion.identity);
                player.DashReset();
                Destroy(gameObject);
            }
        }

    }
}
