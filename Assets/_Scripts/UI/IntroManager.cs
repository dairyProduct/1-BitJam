using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.TextCore.Text;

public class IntroManager : MonoBehaviour
{
    public TMP_Text text1;
    public TMP_Text text2;
    public TMP_Text text3;

    public AudioClip sound;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        text1.enabled = false;
        text2.enabled = false;
        text3.enabled = false;
        StartCoroutine(Intro());
    }

    IEnumerator Intro() {
        yield return new WaitForSeconds(1);
        text1.enabled = true;
        audioSource.PlayOneShot(sound);
        yield return new WaitForSeconds(1);
        text1.fontStyle = TMPro.FontStyles.Strikethrough;
        yield return new WaitForSeconds(1);
        text2.enabled = true;
        audioSource.PlayOneShot(sound);
        yield return new WaitForSeconds(2);
        text3.enabled = true;
        audioSource.PlayOneShot(sound);
        yield return new WaitForSeconds(2);
        text1.enabled = false;
        audioSource.PlayOneShot(sound);
        yield return new WaitForSeconds(.25f);
        text2.enabled = false;
        audioSource.PlayOneShot(sound);
        yield return new WaitForSeconds(.25f);
        text3.enabled = false;
        audioSource.PlayOneShot(sound);
        yield return new WaitForSeconds(.25f);
        SceneManager.LoadScene("Main_01");
    }
}
