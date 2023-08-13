using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    
    
    [Tooltip("The player - or whatever target we want to look at/follow")]
    public Transform target;
    [Tooltip("Follow Speed, smaller = smoother smooth but slower")]
    public float smoothSpeed = 0.125f; 
    [Tooltip("The offset of the camera to the target. Follow Distance")]
    public Vector3 offset;

 


    private void Update()
    {
        //simple lerp from the camera's current pos to the new pos of the player
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed* Time.deltaTime);
        transform.position = smoothedPosition;
    }

}
