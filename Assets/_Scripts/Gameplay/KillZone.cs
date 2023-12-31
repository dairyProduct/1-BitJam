using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other == null) return;
        if(other.tag == "Player") {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            StartCoroutine(player.PlayerDeath(true));
        }
        if(other.tag == "Enemy") {
            Destroy(other.gameObject);
        }
    }
}
