using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerDetection))]
public class BotMovementController : MonoBehaviour
{
    [SerializeField] Vector3 positionA, positionB;
    [SerializeField] Transform player;
    [SerializeField] float botMoveSpeed, botJumpoForce, moveTimer = 3f;

    private Rigidbody2D botRigidbody;

    [Tooltip("Is the bot actively chasing/attacking/aknowledging the players existance")]
    private bool isPatrolling = false;
    [Tooltip("Is the bot actively chasing/attacking/aknowledging the players existance")]
    public bool IsPatrolling
    {
    set { isPatrolling = value; }  // we want events that occur in the world around the bot to determine whether it is patrolling or not(write only)
    }

    private float jumpTimer = 3f;



    void Start()
    {
        botRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if(isPatrolling){
            PatrolBetweenPoints();
        }
        else{

        }
    }
    private void PatrolBetweenPoints(){
        // move between patrol points on x-Axis

        if(transform.position.x < positionA.x){
            botRigidbody.AddForce(Vector2.right * Time.deltaTime * botMoveSpeed/10, ForceMode2D.Force);
        }
        else if(transform.position.x > positionB.x){
            botRigidbody.AddForce(Vector2.left * Time.deltaTime * botMoveSpeed/10, ForceMode2D.Force);
        } 

        jumpTimer -= Time.deltaTime;
        moveTimer -= Time.deltaTime;
        if(moveTimer <= 0f){
            moveTimer = Random.Range(0.25f, 1f);
            int moveDirection = Random.Range(-1, 2);
            botRigidbody.AddForce(Vector2.right * Time.deltaTime * botMoveSpeed * moveDirection, ForceMode2D.Impulse);
        }

        if(jumpTimer <= 0f){
            jumpTimer = Random.Range(1f,3f);
            BotJump(botJumpoForce);
        }
        
        
    }
    

    //depending on the situation, the bot can jump
    private void BotJump(float botJumpForce){
        botRigidbody.AddForce(Vector3.up * botJumpForce);
    }
}
