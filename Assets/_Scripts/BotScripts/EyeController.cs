using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeController : MonoBehaviour
{
    [Header("Variables")]
    public float chargeTime = 3f;

    [Header("Components")]
    LineRenderer lr;
    Rigidbody2D rb;
    PlayerController playerController;

    float time;
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        lr = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(Fire());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Fire() {
        time = chargeTime;
        lr.enabled = true;
        while (time > 0)
        {
            //time -= 0.001f;

            //Set Line Renderer
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, playerController.transform.position);


            Vector3 direction = (playerController.transform.position - transform.position).normalized;
            Vector2 direction2D = new Vector2(direction.x, direction.y);
            
        }
        //Fire
        //lr.enabled = false;
        Debug.Log("Finished");
        yield return null;
    }
}
