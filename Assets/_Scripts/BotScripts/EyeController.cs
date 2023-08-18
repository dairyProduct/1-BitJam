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

    public LayerMask impassableMask;

    [Header("Audio")]
    public AudioClip charge;
    public AudioClip shoot;
    public AudioClip death;

    AudioSource audioSource;

    [Header("Components")]
    public BoxCollider2D killZone;
    public CircleCollider2D bodyCollider;
    public LineRenderer laserLR;

    LineRenderer lr;
    Rigidbody2D rb;
    PlayerController playerController;
    bool charging;
    Vector2 currentLookatPoint;
    

    ParticleSystem currentParticle;
    Vector2 direction;
    bool hitWall;
    RaycastHit2D hit2D;
    RaycastHit2D playerHit;

    float time;
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        lr = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(Charge());
    }

    // Update is called once per frame
    void Update()
    {
        if(charging && time < chargeTime) {
            
            time += Time.deltaTime;

            currentLookatPoint = Vector2.Lerp(currentLookatPoint, playerController.transform.position, Time.deltaTime * lookSpeed);

            direction = playerController.transform.position - transform.position;

            //Look Direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, lookSpeed * Time.deltaTime);

            hit2D = Physics2D.Raycast(transform.position, transform.right, Mathf.Abs((playerController.transform.position - transform.position).magnitude), impassableMask);

            //Check if Ray hit wall or not
            Vector2 endPoint;
            if(hit2D.collider != null) {
                hitWall = true;
                endPoint = hit2D.point;
            } else {
                hitWall = false;
                endPoint = transform.right * 50f;
            }

            #region Line Renderers

            //Sight
            lr.SetPosition(1, new Vector3(Mathf.Abs(((Vector3)endPoint - transform.position).magnitude), 0, 0));

            //Beam
            laserLR.SetPosition(1, new Vector3(Mathf.Abs(((Vector3)endPoint - transform.position).magnitude), 0, 0));

            //Sight animation Speed
            lr.material.SetFloat("_Speed", time / chargeTime * -10f);

            #endregion

            //ChargeParticleSpeed
            //currentParticle.emission.rateOverTimeMultiplier = (chargeTime - time) * 10f;

            

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
        audioSource.PlayOneShot(shoot);
        if(killZone.IsTouching(playerController.GetComponent<Collider2D>()) && !hitWall) {
            if(!playerController.isDashing) {
                playerController.StartCoroutine(playerController.PlayerDeath());
            }
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
        audioSource.PlayOneShot(charge);
        yield return null;
    }

        private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            if(playerController.isDashing) {
                if(bodyCollider.IsTouching(other)) {
                    if(currentParticle != null) {
                    Destroy(currentParticle);
                    }
                    Instantiate(deathParticles, transform.position, Quaternion.identity);
                    audioSource.PlayOneShot(death);
                    playerController.DashReset();
                    Destroy(gameObject);
                }
                
            }
        }
    }
}
