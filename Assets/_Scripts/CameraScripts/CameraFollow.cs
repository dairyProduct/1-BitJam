using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    
    
    [Tooltip("The player - or whatever target we want to look at/follow")]
    public Transform target;
    [Tooltip("Follow Speed, smaller = smoother smooth but slower")]
    public float smoothSpeed = 0.125f; 
    [Tooltip("Distance camera will zoom in/out")]
    public float setCameraSize; 
    [Tooltip("The offset of the camera to the target. Follow Distance")]
    public Vector3 offset;

    private Rigidbody2D targetRB;

    private void Start(){
        targetRB = target.GetComponent<Rigidbody2D>();

    }
    private void Update()
    {
        if(target == null) return;
        //simple lerp from the camera's current pos to the new pos of the player
        float speedZoomDistance = targetRB.velocity.magnitude / 8;
        Camera.main.orthographicSize = setCameraSize + speedZoomDistance;
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed* Time.deltaTime);
        transform.position = smoothedPosition;
    }

}
