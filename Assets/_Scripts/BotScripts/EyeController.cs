using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeController : MonoBehaviour
{
    [Header("Variables")]
    public float chargeTime = 3f;
    public float lookSpeed = 2f;
    public float kickBackForce = 25f;

    [Header("Particles")]
    public GameObject deathParticles;
    public GameObject ChargeParticles;

    [Header("Components")]
    public BoxCollider2D killZone;
    public LineRenderer laserLR;

    LineRenderer lr;
    Rigidbody2D rb;
    PlayerController playerController;
    bool charging;
    Vector2 currentLookatPoint;

    ParticleSystem currentParticle;
    Vector2 direction;

    float time;
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        lr = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(Charge());
    }

    // Update is called once per frame
    void Update()
    {
        if(charging && time < chargeTime) {
            time += Time.deltaTime;

            currentLookatPoint = Vector2.Lerp(currentLookatPoint, playerController.transform.position, Time.deltaTime * lookSpeed);

            direction = (currentLookatPoint - (Vector2)transform.position).normalized;

            lr.material.SetFloat("_Speed", time / chargeTime * -10f);

            //ChargeParticleSpeed
            //currentParticle.emission.rateOverTimeMultiplier = (chargeTime - time) * 10f;

            //Look Direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, lookSpeed * Time.deltaTime);

        } else if(charging && time >= chargeTime){
            charging = false;
            lr.enabled = false;
            if(currentParticle != null){
                Destroy(currentParticle.gameObject);
            }
            StartCoroutine(FireLaser());
            
        }
        
    }

    IEnumerator FireLaser() {
        laserLR.enabled = true;
        if(killZone.IsTouching(playerController.GetComponent<Collider2D>())) {
            playerController.StartCoroutine(playerController.PlayerDeath());
        }
        yield return new WaitForSeconds(.1f);
        laserLR.enabled = false;
        rb.AddForce(-transform.right * kickBackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(1);
        StartCoroutine(Charge());
    }

    IEnumerator Charge() {
        time = 0f;
        lr.enabled = true;
        currentLookatPoint = playerController.transform.position;
        charging = true;
        GameObject go = Instantiate(ChargeParticles, transform.position, ChargeParticles.transform.rotation);
        currentParticle = go.GetComponent<ParticleSystem>();
        
        //Fire
        //lr.enabled = false;
        yield return null;
    }

        private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            if(playerController.isDashing) {
                if(currentParticle != null) {
                    Destroy(currentParticle);
                }
                Instantiate(deathParticles, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}
