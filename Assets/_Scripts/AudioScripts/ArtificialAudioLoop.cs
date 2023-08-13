using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtificialAudioLoop : MonoBehaviour
{
    //use this script on any audio that loops. Not on music.
    [SerializeField] AudioSource audioSource;

    void Update()
    {
        if(!audioSource.isPlaying){
            audioSource.Play();
        }
    }
}
