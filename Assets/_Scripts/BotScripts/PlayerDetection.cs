using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Animator))]
public class PlayerDetection : MonoBehaviour
{
    [Header("FlashLight")]
    public Transform flashlightSource;
    public float detectionDistance = 20f;
    public int rayCount = 30;
    public float flashlightAngle = 30f;

    [Header("Detection")]
    [Tooltip("is player in FOV for long enough to be detected?")]
    [SerializeField] public bool playerDetected = false;
    [Tooltip("How long for bot to lose detection of playerafter leaving FOV?")]
    [SerializeField] float loseDetetcionTime = 2.5f;
    [SerializeField] Transform flashLightPivotPoint;


    private Transform player;
    private BotMovementController botMovementController;
    private Animator botAnimator, exclimationMark;
    private bool BotHasLight = true; // if set to false, bot is unable to do any player detection
    private int rotationBias = 0; //1 for right, -1 for left
    private float changeBiasTimer, rotateTimer, lastSightingTime = 0;  //time between deciding to rotate left or right

    private PlayerController playerController;
    void Start()
    {
        player = GameObject.Find("Player(Clone)").transform;
        botMovementController = GetComponent<BotMovementController>();
        botAnimator = GetComponent<Animator>();
        exclimationMark = GameObject.Find("Exclimation Mark").GetComponent<Animator>();
        playerController = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        if(!BotHasLight) return; // maybenot just return, maybe do something intresting while the bot can't see

        if (PlayerInVision())
        {
            lastSightingTime = Time.time;
            bool justFoundPlayer = true;
            if(playerDetected) justFoundPlayer = false;
            playerDetected = true;
            if(justFoundPlayer) exclimationMark.SetTrigger("Alert");
            playerController.inLight = true;
            
        }
        else{
            RandomFlashLightRotation();
            playerController.inLight = false;
        }
        float timeSinceInitialDetection = Time.time - lastSightingTime;
        


        if (timeSinceInitialDetection >= loseDetetcionTime)
        {
            lostPlayer();
        }
    }



    /// <summary>
    /// checks to see if the player is in the bots field of view with raycasts and returns true if yes
    /// </summary>
    private bool PlayerInVision(){

        float angleIncrement = flashlightAngle / (rayCount - 1);
        float startAngle = flashlightSource.eulerAngles.z - 15;
        bool playerInSight = false;
        for (int i = 0; i < rayCount; i++)
        {
            float angle = startAngle + i * angleIncrement;
            
            Vector3 rayDirection = Quaternion.Euler(0, 0, angle) * Vector3.right;
            RaycastHit2D hit = Physics2D.Raycast(flashlightSource.position, rayDirection, detectionDistance);
            Debug.DrawRay(flashlightSource.position, rayDirection * detectionDistance, Color.red);

            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                flashLightPivotPoint.rotation = Quaternion.Lerp(flashLightPivotPoint.rotation, Quaternion.Euler(0, 0, angle), 10 * Time.deltaTime);
                playerInSight = true;
            }
        }

        return playerInSight;
    }



    /// <summary>
    /// if player is detected, stop patrolling, start attacking, maybe do a lil animation first to show they're now aggressive
    /// </summary>
    private void BecomeAgressive(){
        //play an animation
        botMovementController.IsPatrolling = false;
        //Set up new movement option
    }



    /// <summary>
    /// do a lil animation to show that the bot is no longer in agro mode. stop tracking player
    /// </summary>
    private void lostPlayer(){
        //play an animation
        playerDetected = false;
        botMovementController.IsPatrolling = true;
        Debug.Log("Lost em");
        
        //if we cant dialogue, wait like 2 seconds then continue any dialogue left off on if the bots buddy is still near by
    }

    /// <summary>d
    /// Rotates flashlight left or right based on a bias value
    /// </summary>
    private void RandomFlashLightRotation(){
        changeBiasTimer -= Time.deltaTime;
        rotateTimer -= Time.deltaTime;
        
        if(changeBiasTimer <= 0f){
            rotationBias = Random.Range(-1, 1); //set rotation bias to left or right
            if(rotationBias == 0){
                rotationBias = 1;
            }
            changeBiasTimer = Random.Range(4f, 6f); // time between potentially changing rotation directions
        }

        if(rotateTimer <= 0f){
            flashLightPivotPoint.rotation = Quaternion.Euler(0, 0, flashLightPivotPoint.eulerAngles.z + 1 * rotationBias); //it's in fixed update so no need to call this every set amount of time
            rotateTimer = 0.01f;
        }
    }

}
