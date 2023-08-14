using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BotMovementController))]
public class PlayerDetection : MonoBehaviour
{
    [Header("FlashLight")]
    public Transform flashlightSource;
    public LayerMask detectionLayer;
    public float detectionDistance = 20f;
    public int rayCount = 30;
    public float flashlightAngle = 30f;




    [Header("Detection")]
    [Tooltip("is player in FOV for long enough to be detected?")]
    [SerializeField] bool playerDetected = false;
    [Tooltip("How long for bot to detect player in FOV?")]
    [SerializeField] float detectionTime = 1.5f;
    [Tooltip("How long for bot to lose detection of playerafter leaving FOV?")]
    [SerializeField] float loseDetetcionTime = 2.5f;
    [SerializeField] Transform player, flashLightPivotPoint;

    private BotMovementController botMovementController;
    private Animator botAnimator;
    private bool BotHasLight = true; // if set to false, bot is unable to do any player detection
    private int rotationBias = 0; //1 for right, -1 for left
    private float changeBiasTimer, rotateTimer, lastSightingTime = 0;  //time between deciding to rotate left or right
    void Start()
    {
        botMovementController = GetComponent<BotMovementController>();
        botAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if(!BotHasLight) return; // maybenot just return, maybe do something intresting while the bot can't see

        if (PlayerInVision())
        {
            lastSightingTime = Time.time;
            playerDetected = true;
            
        }
        else{
            RandomFlashLightRotation();
        }
        float timeSinceInitialDetection = Time.time - lastSightingTime;
        
        // Check if it's time to stop patrolling and return to normal behavior
        
        if(playerDetected && timeSinceInitialDetection < loseDetetcionTime){
            BecomeAgressive();
        }
        else if (timeSinceInitialDetection >= loseDetetcionTime)
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
        playerDetected = true;
        botMovementController.IsPatrolling = false;
    }



    /// <summary>
    /// do a lil animation to show that the bot is no longer in agro mode. stop tracking player
    /// </summary>
    private void lostPlayer(){
        playerDetected = false;
        botMovementController.IsPatrolling = true;
        Debug.Log("Lost em");
        //botAttackController.IsAgressive = false;
    }



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
