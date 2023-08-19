using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallEnemyController : MonoBehaviour
{
    [SerializeField] GameObject attackPrefab;
    [Tooltip("How fast the attack will move. initial velocity")]
    [SerializeField] float attackSpeed;
    [Header("Particles")]
    [SerializeField] GameObject deathParticles;
    [Header("Audio")]
    [SerializeField] AudioClip mouthOpen;
    [SerializeField] AudioClip mouthClose;
    [SerializeField] AudioClip death;
    private AudioSource audioSource;
    private PlayerController player;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    public void SpawnAttack(){
        GameObject n = (GameObject)Instantiate(attackPrefab, transform.position, Quaternion.identity);
        //there's probably a way better way to give the attack a direction but i'm dum idk
        if(transform.rotation.z == 0f){
            n.GetComponent<Rigidbody2D>().velocity = Vector2.left * attackSpeed;
        }
        else if(transform.rotation.z == 0.7071068f){
            Debug.Log("Bruh");
            n.GetComponent<Rigidbody2D>().velocity = Vector2.down * attackSpeed;
        }
        else if(transform.rotation.z == 1f){
            n.GetComponent<Rigidbody2D>().velocity = Vector2.right * attackSpeed;
        }

        
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            if(player.isDashing) {
                Instantiate(deathParticles, transform.position, Quaternion.identity);
                //audioSource.PlayOneShot(death);
                player.DashReset();
                Destroy(gameObject);
            } else {
                player.StartCoroutine(player.PlayerDeath());
                //audioSource.PlayOneShot(death);
                Destroy(gameObject);
            }
        }
    }
}
