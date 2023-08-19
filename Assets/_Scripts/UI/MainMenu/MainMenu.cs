using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Audio;

[RequireComponent(typeof(LeaderBoard))]
public class MainMenu : MonoBehaviour
{
    [SerializeField] Canvas mainMenuCanvas, leaderBoardCanvas, SettingsCanvas, fadeCanvas;
    [SerializeField] GameObject createUserNamePanel;
    [SerializeField] TextMeshProUGUI userNameField, nameSizeIssueText, musicPercent, soundPercent, userNameTextDisplay;
    [SerializeField] Slider musicSlider, soundSlider;
    [SerializeField] Animator fadeAnimator, pauseTextAnimator;
    [SerializeField] GameObject musicPlayer;
    [SerializeField] AudioSource menuAudioSource;
    [SerializeField] AudioClip[] menuAudio;
    public AudioMixer mixer;

    private const string userNameKey = "UserName";
    private const string musicVolumeKey = "MusicVolume";
    private const string soundVolumeKey = "SoundVolume";
    private float defaultVolume = 50f;




    private void Start(){
        //StartCoroutine(PlayFadeIn());
        LoadPlayerMainManuData();


    }
    #region MenuPlayerPrefs
    private void LoadPlayerMainManuData(){
        if (PlayerPrefs.HasKey(userNameKey))
        {
            string userName = PlayerPrefs.GetString(userNameKey);
            userNameTextDisplay.text = userName;
        }
        else
        {
            createUserNamePanel.SetActive(true);
        }
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

    public void AcceptUserName(){
        PlayerPrefs.SetString(userNameKey, userNameField.text);

        PlayerPrefs.Save();
        userNameTextDisplay.text = PlayerPrefs.GetString(userNameKey);
        createUserNamePanel.SetActive(false);

    }
    private void ApplyVolume()
    {
        // Sound Mixer Magic lolololololol
        mixer.SetFloat("MusicVolume", Mathf.Log10(musicSlider.value) * 20f);
        mixer.SetFloat("SoundVolume", Mathf.Log10(soundSlider.value) * 20f);
    }

    public void ChangePercentages(){
        ApplyVolume();
        musicPercent.text = Mathf.RoundToInt((musicSlider.value * 100f)).ToString() + "%";
        soundPercent.text = Mathf.RoundToInt((soundSlider.value * 100f)).ToString() + "%";
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
        SceneManager.LoadScene("intro");
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
        //fadeAnimator.SetTrigger("FadeIn");
        
        yield return new WaitForSeconds(7);
        fadeCanvas.enabled = false;
    }
    private IEnumerator PlayFadeOut(){
        fadeCanvas.enabled = true;
        //fadeAnimator.SetTrigger("FadeOut");
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
