using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PusherController : MonoBehaviour
{
    public float pushForce = 10f;

    PlayerController player;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        //animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player") {
            player = other.GetComponent<PlayerController>();
            player.PlayerStopMovementForTime(.2f);
            //animator.SetTrigger("Push");
            Vector2 incVelocity = (player.transform.position - transform.position).normalized;
            Vector2 newVelocity = incVelocity * pushForce;
            
            player.rb.velocity = newVelocity;
            player.DashReset(false);
        }
    }
}
