using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dan.Main;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class LeaderBoard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI inputedName, playerCardName, playerCardPastTime, playerCardBestTime;


    [SerializeField] private List<TextMeshProUGUI> names;
    [SerializeField] private List<TextMeshProUGUI> times;
    [SerializeField] private List<TextMeshProUGUI> Positions;

    //playerprefs
    private string pastUserName = "PastUserName";
    private string pastTimeKey = "PastTimeKey";
    private string bestTimeKey = "BestTimeKey";


    public UnityEvent<string, int> submitScoreEvent;
    


    private const string LeaderboardKey = 
        "a17a215d750e7d1c1eab16fc09cc9e931edc82cd6d61986e0e0f007239965a92";

    private void Start(){
        
        GetLeaderBoard();
        LoadPlayerLobbyCardData();
    }

    #region PlayerCardLogic
    private void LoadPlayerLobbyCardData(){
        //if the player played before
        #region SetPlayerCardTimeTexts
        if (PlayerPrefs.HasKey(pastTimeKey))
        {
            float bestTime = PlayerPrefs.GetFloat(bestTimeKey);
            float pastTime = PlayerPrefs.GetFloat(pastTimeKey);

            if(pastTime < bestTime || bestTime == 0){
                bestTime = pastTime;
                PlayerPrefs.SetFloat(bestTimeKey, PlayerPrefs.GetFloat(pastTimeKey));
                PlayerPrefs.Save();
                SubmitScore();
            }
            
            //convert to a proper time 00:00:00
            playerCardPastTime.text = ConvertToReadableTime(pastTime);

            playerCardBestTime.text = ConvertToReadableTime(bestTime); 
        }
        else
        {
            playerCardPastTime.text = "--:--:--"; 
            playerCardBestTime.text = "--:--:--"; 
        }
        #endregion

        #region SetPlayerCardNameText
        if (PlayerPrefs.HasKey(pastTimeKey))
        {
            string pastName = PlayerPrefs.GetString(pastUserName);
            playerCardName.text = pastName; 
        }
        else
        {
            playerCardName.text = ""; 
        }
        #endregion
    }
    #endregion

    #region MainLeaderboardLogic
    public void GetLeaderBoard(){
        LeaderboardCreator.GetLeaderboard(LeaderboardKey, ((msg) => {
            int loopLength = (msg.Length < names.Count) ? msg.Length : names.Count;
            int pos = 1;
            for(int i = loopLength-1; i >= 0; --i){

                Positions[pos-1].text = pos.ToString();
                names[pos-1].text = msg[i].Username;
                times[pos-1].text = ConvertToReadableTime(msg[i].Score);

                pos += 1;
            }
        }));
    }

    //**************CALL THIS AFTER PLAYER BEATS THE GAME(use the submitScore unity event)**************
    public void SetLeaderBoardEntry(string username, int score){
        LeaderboardCreator.UploadNewEntry(LeaderboardKey, username, 
            score, ((msg) => {
            //Do something to sensor names
            GetLeaderBoard();
        }));
    }

    public void SubmitScore(){
        int time = (int)PlayerPrefs.GetFloat(bestTimeKey);
 
        submitScoreEvent.Invoke(PlayerPrefs.GetString(pastUserName), time);
    }
    #endregion

    private string ConvertToReadableTime(float inputTime){
        int minutes = Mathf.FloorToInt(inputTime / 60);
        int seconds = Mathf.FloorToInt(inputTime % 60);
        string outputTime = string.Format("{0:00}:{1:00}", minutes, seconds);
        return outputTime;
    }

}
