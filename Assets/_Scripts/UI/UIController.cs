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
    public TMP_Text yLevel;


    private Transform player;
    PlayerController playerController;
    void Start()
    {
        player = GameObject.Find("Player(Clone)").transform;
        //playerController = FindObjectOfType<PlayerController>();
        //playerController.lightUpdate += UpdateLightBar;
    }

    public void UpdateLightBar() {
        lightBar.value = playerController.lightExposure / playerController.maxLightExposure;
        lightPercent.text = Mathf.RoundToInt((playerController.lightExposure / playerController.maxLightExposure) * 100) + "%";
    }

    // Update is called once per frame
    void Update()
    {
        yLevel.text = ((int)player.transform.position.y).ToString();
    }
}
