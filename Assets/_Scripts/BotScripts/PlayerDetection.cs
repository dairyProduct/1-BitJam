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
    [SerializeField] Transform player;

    private BotMovementController botMovementController;
    private Animator botAnimator;
    private bool BotHasLight = true;
    void Start()
    {
        botMovementController = GetComponent<BotMovementController>();
        botAnimator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if(!BotHasLight) return; // maybenot just return, maybe do something intresting while the bot can't see



        float lastSightingTime = 0;

        if (PlayerInVision())
        {
            lastSightingTime = Time.time;
            playerDetected = true;
        }
        
        // Check if it's time to stop patrolling and return to normal behavior
        float timeSinceInitialDetection = Time.time - lastSightingTime;
        
        if(playerDetected && timeSinceInitialDetection > detectionTime){
            BecomeAgressive();
        }
        else if (!playerDetected && timeSinceInitialDetection > loseDetetcionTime)
        {
            lostPlayer();
        }
    }

    /// <summary>
    /// checks to see if the player is in the bots field of view with raycasts and returns true if yes
    /// </summary>
    private bool PlayerInVision(){

        float angleIncrement = flashlightAngle / (rayCount - 1);
        float startAngle = flashlightSource.rotation.z;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = startAngle + i * angleIncrement - ((startAngle + i * angleIncrement) / 2);
            Vector3 rayDirection = Quaternion.Euler(0, 0, angle) * transform.right;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, detectionDistance);
            Debug.DrawRay(transform.position, rayDirection * detectionDistance, Color.red);
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                Debug.Log("Player is in FOV!");
                break; // Exit loop once player is detected
            }
            else if(hit.collider != null ){
                //Debug.Log("Player is in FOV!");
                break;
            }
        }
        
        return false;
    }

    /// <summary>
    /// if player is detected, stop patrolling, start attacking, maybe do a lil animation first to show they're now aggressive
    /// </summary>
    private void BecomeAgressive(){
        playerDetected = true;
        botMovementController.IsPatrolling = false;
        //botAttackController.IsAgressive = true;
        AimForPlayer();
    }

    /// <summary>
    /// do a lil animation to show that the bot is no longer in agro mode
    /// </summary>
    private void lostPlayer(){
        playerDetected = false;
        botMovementController.IsPatrolling = true;
        //botAttackController.IsAgressive = false;
    }
    /// <summary>
    /// Make the bot do a lil aiming to stay focused on the player
    /// </summary>
    private void AimForPlayer(){
        
    }
}
