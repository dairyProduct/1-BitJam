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
    public Image gameOverPanel;


    private Transform cam;
    PlayerController playerController;
    void Start()
    {
        cam = GameObject.Find("CameraHolder").transform;
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
        yLevel.text = ((int)cam.transform.position.y).ToString();
    }

    private void DisplayEndGameUI(){
        gameOverPanel.gameObject.SetActive(true);
        gameOverPanel.GetComponent<Animator>().SetTrigger("GameOver");
        


    }
}
