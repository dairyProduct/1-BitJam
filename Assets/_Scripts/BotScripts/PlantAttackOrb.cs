using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantAttackOrb : MonoBehaviour
{

    [Header("Particles")]
    [SerializeField] GameObject deathParticles;
    [Header("Audio")]
    [SerializeField] AudioClip OrbSound;
    private AudioSource audioSource;
    private PlayerController player;
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        //audioSource.Play();
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
