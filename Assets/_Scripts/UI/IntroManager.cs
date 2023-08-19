using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class IntroManager : MonoBehaviour
{
    public TMP_Text text1;
    public TMP_Text text2;
    public TMP_Text text3;

    // Start is called before the first frame update
    void Start()
    {
        text1.enabled = true;
        text2.enabled = false;
        text3.enabled = false;
        StartCoroutine(Intro());
    }

    IEnumerator Intro() {
        yield return new WaitForSeconds(1);
        text2.enabled = true;
        yield return new WaitForSeconds(1);
        text3.enabled = false;
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Main_01");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
