using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAimer : MonoBehaviour
{
    [SerializeField] float speed, trackingTime = 10f; 
    [SerializeField] ParticleSystem ps1, ps2;

    private Transform player;
    private PlayerController playerController; 
    private bool isTracking = true; 
    private bool collided = false;
    private Vector3 direction;
    //public AudioClip[] sounds;

    void Start()
    {
        player = GameObject.Find("Player(Clone)").transform;
        playerController = player.GetComponent<PlayerController>();
        //GetComponent<AudioSource>().clip = sounds[Random.Range(0, sounds.Length-1)];
        //GetComponent<AudioSource>().Play(0);
        Invoke("StopTracking", trackingTime);
    }

    void FixedUpdate()
    {
        if(collided) return;
        if (!isTracking)
        {
            GetComponent<Rigidbody2D>().velocity = direction * speed;
        }
        else{
            direction = (player.position - transform.position).normalized;
            GetComponent<Rigidbody2D>().velocity = direction * speed;
        }
        transform.up = -direction;
    }

    void StopTracking()
    {
        isTracking = false;
    }

    private void OnTriggerEnter2D(Collider2D col){
        /*
        if(col.gameObject.tag == "Player" || ){ //if we wanna make the bullet break on ground cliision. for now it's off idk 
            playerController.lightExposure = playerController.maxLightExposure;
            StartCoroutine(DestroyEnergyBall());
        }
        else if(col.gameObject.tag == "Ground"){
            StartCoroutine(DestroyEnergyBall());
        }

        */
    }

    private IEnumerator DestroyEnergyBall(){
        ps1.Play();
        ps2.Play();
        collided = true;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        yield return new WaitForSeconds(0.1f);
        transform.GetChild(0).gameObject.SetActive(false); // delete this line when we get art for the attack
        yield return new WaitForSeconds(0.9f);
        Destroy(gameObject);
    }


    
}
