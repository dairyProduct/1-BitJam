using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioFadeIn
{
    public static IEnumerator FadeIn(AudioSource audioSource, float FadeTime)
    {
        float goalVolume = audioSource.volume;
        audioSource.volume = 0;

        while (audioSource.volume < goalVolume)
        {
            audioSource.volume += goalVolume * Time.deltaTime / FadeTime;

            yield return null;
        }
        
    }
}
