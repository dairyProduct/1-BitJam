using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class UIController : MonoBehaviour
{


    [SerializeField] Canvas pauseCanvas, settingsCanvas;
    [SerializeField] TextMeshProUGUI musicPercent, soundPercent, bestScoreText, currentScoreText, scoreMultiplierText;
    [SerializeField] Slider musicSlider, soundSlider;
    [SerializeField] TMP_Dropdown controlsDropdown;
    [SerializeField] Animator fadeAnimator, pauseTextAnimator;
    [SerializeField] AudioSource pauseGameAudioSource;
    [SerializeField] AudioClip pauseGame, unpauseGame;

    public AudioClip hoverSound;
    public AudioClip pressSound;
    public AudioMixer mixer;

    private const string controlsKey = "Controls";
    private const string musicVolumeKey = "MusicVolume";
    private const string soundVolumeKey = "SoundVolume";
    private const string bestScoreKey = "BestScoreValue";
    private float defaultVolume = 50f;
    private int defaultScore, scoreEarned = 0;
    private Transform cam;
    private bool gamePaused, inSettings;
    PlayerController player;
    void Awake(){
        LoadPlayerInGameData();
        PlayFadeIn();
    }
    void Start()
    {
        cam = GameObject.Find("CameraHolder").transform;
        player = FindObjectOfType<PlayerController>();
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(!gamePaused){
                PauseGame();
            }
            else if(gamePaused && !inSettings){
                UnpauseGame();
            }
            else if(gamePaused && inSettings){
                
                CloseSettings();
            }
        }
        UpdateScoreUI(0);
        
    }


    
    #region PauseGame
    


    #region MenuPlayerPrefs
    private void LoadPlayerInGameData(){
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

        if (PlayerPrefs.HasKey(bestScoreKey))
        {
            int bestScore = PlayerPrefs.GetInt(bestScoreKey);
            bestScoreText.text = bestScore.ToString();
        }
        else
        {
            bestScoreText.text = defaultScore.ToString();
        }
        if(PlayerPrefs.HasKey(controlsKey)) {
            controlsDropdown.value = PlayerPrefs.GetInt(controlsKey);
        } else {
            controlsDropdown.value = 0;
        }
        ChangePercentages();
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

    public void SetControls() {
        if(player == null) return;
        if(controlsDropdown.value == 0) {
            player.SetControls(true);
        } else {
            player.SetControls(false);
        }
    }
    

    public void SaveSettingsData(){   
        PlayerPrefs.SetFloat(musicVolumeKey, musicSlider.value);
        PlayerPrefs.SetFloat(soundVolumeKey, soundSlider.value);
        PlayerPrefs.SetInt(controlsKey, controlsDropdown.value);
        PlayerPrefs.Save(); 
    }

    public void SaveScoreData(){
        //Update best score if it is first score or if it's better than the best score
        if (!PlayerPrefs.HasKey(bestScoreKey)){
            PlayerPrefs.SetInt(bestScoreKey, int.Parse(currentScoreText.text));
        }
        else if(int.Parse(bestScoreText.text) < int.Parse(currentScoreText.text)){
            PlayerPrefs.SetInt(bestScoreKey, int.Parse(currentScoreText.text));
        }
        PlayerPrefs.Save();
        
    }
    #endregion


    public void OpenSettings(){
        inSettings = true;
        settingsCanvas.enabled = true;
        pauseCanvas.enabled = false;
    }

    public void CloseSettings(){
        inSettings = false;
        
        settingsCanvas.enabled = false;
        pauseCanvas.enabled = true;
        SaveSettingsData();
        
    }
    
    public void RestartGame(){
        SceneManager.LoadScene(1);
    }

    public void LoadMainMenu(){
        SceneManager.LoadScene(0);
    }

    public void PauseGame(){
        gamePaused = true;
        Time.timeScale = 0;
        pauseCanvas.enabled = true;
        //pauseTextAnimator.SetTrigger("Paused");
        //pauseGameAudioSource.clip = unpauseGame;
        //pauseGameAudioSource.Play();
    }

    public void UnpauseGame(){
        gamePaused = false;
        Time.timeScale = 1;
        pauseCanvas.enabled = false;
        //pauseGameAudioSource.clip = pauseGame;
        //pauseGameAudioSource.Play();
    }

    public void PlayerHoverSound() {
        pauseGameAudioSource.PlayOneShot(hoverSound);
    }
    public void PlayPressSound() {
        pauseGameAudioSource.PlayOneShot(pressSound);
    }

    private void PlayFadeIn(){
        fadeAnimator.SetTrigger("GameStarted");
        
    }

    public void PlayFadeOut(){
        bestScoreText.transform.parent.GetComponent<Animator>().enabled = false;
        currentScoreText.transform.parent.GetComponent<Animator>().enabled = false;
        fadeAnimator.SetTrigger("GameEnded");
    }
    public void UpdateScoreMultiplier(int scoreMultiplier){
        scoreMultiplierText.text = scoreMultiplier.ToString();
    }
    public void UpdateScoreUI(int score){
        scoreEarned += score * int.Parse(scoreMultiplierText.text);
        currentScoreText.text = ((int)cam.transform.position.y + scoreEarned).ToString();
    }

    #endregion


    
}
