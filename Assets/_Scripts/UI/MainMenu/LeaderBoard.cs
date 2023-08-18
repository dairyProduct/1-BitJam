using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dan.Main;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class LeaderBoard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI inputedName, playerCardName, playerCardBestScore;


    [SerializeField] private List<TextMeshProUGUI> names;
    [SerializeField] private List<TextMeshProUGUI> scores;
    [SerializeField] private List<TextMeshProUGUI> Positions;

    //playerprefs
    private const string userNameKey = "UserName";
    private const string bestScoreKey = "BestScoreValue";
    //private string bestTimeKey = "BestTimeKey";


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
        if (PlayerPrefs.HasKey(bestScoreKey))
        {
            playerCardBestScore.text = PlayerPrefs.GetInt(bestScoreKey).ToString(); 
        }
        else
        {
            playerCardBestScore.text = "0"; 
        }
        #endregion

        #region SetPlayerCardNameText
        if (PlayerPrefs.HasKey(bestScoreKey))
        {
            string pastName = PlayerPrefs.GetString(userNameKey);
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
            for(int i = 0; i < loopLength; i++){

                Positions[i].text = (i+1).ToString();
                names[i].text = msg[i].Username;
                scores[i].text = msg[i].Score.ToString();

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
        if(!PlayerPrefs.HasKey(bestScoreKey)) return;
        int score = PlayerPrefs.GetInt(bestScoreKey);
        string playerName = PlayerPrefs.GetString(userNameKey);
        submitScoreEvent.Invoke(playerName, score);
    }
    #endregion


}
