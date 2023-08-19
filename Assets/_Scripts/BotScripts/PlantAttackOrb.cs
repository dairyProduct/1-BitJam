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
    private UIController uIController;
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        uIController = FindObjectOfType<UIController>();
        //audioSource.Play();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            if(player.isDashing) {
                uIController.UpdateScoreUI(3);
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
