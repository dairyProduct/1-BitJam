using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform, playerTransform;  // Reference to the camera's transform
    [SerializeField] private Vector3 originalPosition;   // Store the camera's original position
    private Quaternion originalRotation; // Store the camera's original rotation

    [SerializeField] private float shakeDuration = 0f;    // Duration of the shake effect
    [SerializeField] private float shakeMagnitude = 0.7f; // Magnitude of the shake (how intense it is)
    [SerializeField] private float dampingSpeed = 1.0f;   // How quickly the shake fades out


    // Call this method to trigger the camera shake
    


    private void FixedUpdate()
    {
        originalPosition = transform.position;
        if (shakeDuration <= -5){
            ShakeCamera(3, shakeMagnitude);
        }
        shakeDuration -= Time.deltaTime * dampingSpeed;
        if (shakeDuration > 0)
        {
            transform.position = originalPosition * shakeMagnitude * Random.Range(-10f,-1);
            
        }
        else
        {
            shakeDuration = 0f;
            transform.position = originalPosition;
        }
    }

    public void ShakeCamera(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }
}
