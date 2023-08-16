using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeController : MonoBehaviour
{
    [Header("Variables")]
    public float chargeTime = 3f;
    public float lookSpeed = 2f;

    [Header("Components")]
    LineRenderer lr;
    Rigidbody2D rb;
    PlayerController playerController;
    bool charging;
    Vector2 currentLookatPoint;

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
        if(charging && time > 0) {
            time -= Time.deltaTime;

            currentLookatPoint = Vector2.Lerp(currentLookatPoint, playerController.transform.position, Time.deltaTime * lookSpeed);

            Vector2 direction = (currentLookatPoint - (Vector2)transform.position).normalized;

            //Set Line Renderer
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, direction * 15f);

            
            transform.right = direction;
        } else if(charging && time <= 0){
            charging = false;
            lr.enabled = false;

            //Fire
        }
        
    }

    IEnumerator Charge() {
        time = chargeTime;
        lr.enabled = true;
        currentLookatPoint = playerController.transform.position;
        charging = true;
        
        //Fire
        //lr.enabled = false;
        yield return null;
    }
}
