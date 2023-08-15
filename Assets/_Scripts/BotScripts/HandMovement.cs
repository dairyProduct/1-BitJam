using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float movementInterval = 3f;
    [SerializeField] float attackDistance = 5f;
    [SerializeField] GameObject flamePrefab;

    private Transform player;
    private Vector2 targetPosition;
    private Rigidbody2D rb;
    private float timer;

    private void Start()
    {
        player = GameObject.Find("Player(Clone)").transform;
        rb = GetComponent<Rigidbody2D>();
        ChooseRandomTargetPosition();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= movementInterval)
        {
            ChooseRandomTargetPosition();
            timer = 0f;
            Vector3 playerPos = player.position;

            if(Vector2.Distance(transform.position, playerPos) <= attackDistance ){
                HandAttack();
            }
                
        }

        MoveToTargetPosition();
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
        GameObject flame = Instantiate(flamePrefab, transform);
    }

}
