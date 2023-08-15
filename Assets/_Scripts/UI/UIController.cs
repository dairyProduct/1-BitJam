using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [Header("LightBar")]
    public Slider lightBar;
    public TMP_Text lightPercent;



    PlayerController playerController;
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerController.lightUpdate += UpdateLightBar;
    }

    public void UpdateLightBar() {
        lightBar.value = playerController.lightExposure / playerController.maxLightExposure;
        lightPercent.text = (playerController.lightExposure / playerController.maxLightExposure) * 100f + "%";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
