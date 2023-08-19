using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [Header("Sources")]
    public AudioSource source1;
    public AudioSource source2;
    public AudioSource source3;
    public AudioSource source4;
    public AudioSource source5;

    [Header("Tracks")]
    public AudioClip piano;
    public AudioClip drums1;
    public AudioClip drums2;
    public AudioClip strings;
    public AudioClip synth;

    public float fadeDuration = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddTrack(int difficulty) {
        if(difficulty == 0) {
            source1.clip = piano;
            source5.clip = synth;
            source1.Play();
            source5.time = source1.time;
            source5.Play();
        } else if(difficulty == 1) {
            StartCoroutine(FadeInTrack(source2, strings));
        } else if(difficulty == 2) {
            StartCoroutine(FadeInTrack(source3, drums2));
        }else if(difficulty == 3) {
            StartCoroutine(FadeInTrack(source4, drums1));
        }
    }

    IEnumerator FadeInTrack(AudioSource source, AudioClip track) {
        float originalVolume = source.volume;

        source.clip = track;
        source.volume = 0;
        source.Play();
        source.time = source1.time;
        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(0f, originalVolume, time / fadeDuration);
            yield return null;
        }

        source.volume = originalVolume;
        yield break;
    }
}
