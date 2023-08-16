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
    public GameObject ChargeParticles;

    [Header("Components")]
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

            //Set Line Renderer
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, direction * 15f);

            lr.material.SetFloat("Speed", time / chargeTime * -10f);

            //ChargeParticleSpeed
            //currentParticle.emission.rateOverTimeMultiplier = (chargeTime - time) * 10f;
            
            transform.right = direction;
        } else if(charging && time >= chargeTime){
            charging = false;
            lr.enabled = false;
            if(currentParticle != null){
                Destroy(currentParticle.gameObject);
            }

            //Fire
            rb.AddForce(-direction * kickBackForce);
        }
        
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
}
