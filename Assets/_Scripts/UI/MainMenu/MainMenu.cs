using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Events;

[RequireComponent(typeof(LeaderBoard))]
public class MainMenu : MonoBehaviour
{
    [SerializeField] Canvas mainMenuCanvas, leaderBoardCanvas, SettingsCanvas, fadeCanvas;
    [SerializeField] TextMeshProUGUI userNameField, nameSizeIssueText, musicPercent, soundPercent;
    [SerializeField] Slider musicSlider, soundSlider;
    [SerializeField] Animator fadeAnimator, pauseTextAnimator;
    [SerializeField] GameObject musicPlayer;
    [SerializeField] AudioSource menuAudioSource;
    [SerializeField] AudioClip[] menuAudio;

    private string pastUserName = "PastUserName";
    private const string musicVolumeKey = "MusicVolume";
    private const string soundVolumeKey = "SoundVolume";
    private float defaultVolume = 50f;




    private void Start(){
        StartCoroutine(PlayFadeIn());
        LoadPlayerMainManuData();

    }
    #region MenuPlayerPrefs
    private void LoadPlayerMainManuData(){
        if (PlayerPrefs.HasKey(musicVolumeKey))
        {
            float musicVolume = PlayerPrefs.GetFloat(musicVolumeKey);
            musicSlider.value = musicVolume;
        }
        else
        {
            musicSlider.value = defaultVolume;
        }

        if (PlayerPrefs.HasKey(soundVolumeKey))
        {
            float soundVolume = PlayerPrefs.GetFloat(soundVolumeKey);
            soundSlider.value = soundVolume;
        }
        else
        {
            soundSlider.value = defaultVolume;
        }
        ChangePercentages();
    }


    private void ApplyVolume()
    {
        // Find all audio sources in the scene and set their volume
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in audioSources)
        {
            if(audioSource.loop == false){
                audioSource.volume = soundSlider.value / 100;
            }
            else{
                audioSource.volume = musicSlider.value / 100;
            }
        }
    }

    public void ChangePercentages(){
        ApplyVolume();
        musicPercent.text = (musicSlider.value).ToString() + "%";
        soundPercent.text = (soundSlider.value).ToString() + "%";
    }
    

    public void SavePlayerData(){   
        

        PlayerPrefs.SetFloat(musicVolumeKey, musicSlider.value);
        PlayerPrefs.SetFloat(soundVolumeKey, soundSlider.value);

        PlayerPrefs.Save();
        

        //we also want to save audio settings and 
    }
    #endregion

    #region MainMenuOnlyButtonLogic
    public void StartGame(){
        if(userNameField.text.Length <= 2 || userNameField.text.Length > 15){
            StopAllCoroutines();
            StartCoroutine(NameFailure(userNameField.text.Length));
            return;
        }
        SavePlayerData();
        PlayerPrefs.SetString(pastUserName, userNameField.text);
        PlayerPrefs.Save();
        //StartCoroutine(AudioFadeOut.FadeOut(musicPlayer.GetComponent<AudioSource>(), 2));
        //StartCoroutine(PlayFadeOut());
        SceneManager.LoadScene(1);
    }

    public void OpenLeaderBoard(){
        leaderBoardCanvas.enabled = true;
        mainMenuCanvas.enabled = false;
    }

    public void CloseLeaderBoard(){
        leaderBoardCanvas.enabled = false;
        mainMenuCanvas.enabled = true;
    }

    public void CloseGame(){
        Application.Quit();
    }
    #endregion

    public void OpenSettings(){
        SettingsCanvas.enabled = true;
        mainMenuCanvas.enabled = false;
    }

    public void CloseSettings(){
        SettingsCanvas.enabled = false;
        mainMenuCanvas.enabled = true;
        SavePlayerData();
    }



    
    public void RestartGame(){
        SceneManager.LoadScene(1);
    }

    public void LoadMainMenu(){
        SceneManager.LoadScene(0);
    }
    /*
    public void PauseGame(){
        GetComponent<Timer>().StopTrackingTime();
        mainMenuCanvas.enabled = true;
        

        pauseTextAnimator.SetTrigger("Paused");
        menuAudioSource.clip = menuAudio[0];
        menuAudioSource.Play();
    }
    

    public void UnpauseGame(){
        GetComponent<Timer>().TrackTime();
        mainMenuCanvas.enabled = false;
        menuAudioSource.clip = menuAudio[1];
        menuAudioSource.Play();
    }
    */

    
    private IEnumerator NameFailure(int nameLength){
        nameSizeIssueText.gameObject.SetActive(true);
        if(nameLength < 3){
            nameSizeIssueText.text = "Name Too Short!";
        }
        else if(nameLength > 13){
            nameSizeIssueText.text ="Name Too Long!";
        }
        yield return new WaitForSeconds(5);
        nameSizeIssueText.gameObject.SetActive(false);

    }

    private IEnumerator PlayFadeIn(){
        fadeCanvas.enabled = true;
        fadeAnimator.SetTrigger("FadeIn");
        
        yield return new WaitForSeconds(7);
        fadeCanvas.enabled = false;
    }
    private IEnumerator PlayFadeOut(){
        fadeCanvas.enabled = true;
        fadeAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(2.1f);
        SceneManager.LoadScene(1);
    }
    /*
    public IEnumerator PlayerWon(){
        Timer timer = GetComponent<Timer>();
        fadeCanvas.enabled = true;
        fadeAnimator.SetTrigger("FadeOut");
        timer.StopTrackingTime();
        timer.DisplayTime();
        timer.SaveTime();
        yield return new WaitForSeconds(2.1f);
        SceneManager.LoadScene(0);
    }
    */

}
