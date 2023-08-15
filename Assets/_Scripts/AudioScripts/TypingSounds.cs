using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypingSounds : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] clickSounds;
    // Start is called before the first frame update
    public void PlaySound(){
        int randomSound = Random.Range(0, 3);
        audioSource.clip = clickSounds[randomSound];
        audioSource.Play();
    }
}
