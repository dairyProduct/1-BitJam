using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallEnemyController : MonoBehaviour
{
    [SerializeField] GameObject attackPrefab;
    
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
        GameObject n = Instantiate(attackPrefab, transform.position, Quaternion.identity);
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            if(player.isDashing) {
                Instantiate(deathParticles, transform.position, Quaternion.identity);
                audioSource.PlayOneShot(death);
                player.DashReset();
                Destroy(gameObject);
            } else {
                player.StartCoroutine(player.PlayerDeath());
                audioSource.PlayOneShot(death);
                Destroy(gameObject);
            }
        }
    }
}
