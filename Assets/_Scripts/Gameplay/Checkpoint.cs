using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    GameController gameController;
    bool alreadyChecked;

    // Start is called before the first frame update
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    public void OnTriggerEnter2D() {
        if(gameController == null || alreadyChecked) return;
        gameController.SetCheckPoint(transform.position);
        alreadyChecked = true;
    }
}
