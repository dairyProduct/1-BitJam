using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    [Tooltip("difficultyLevel * timeBetweenDifficulties + 30 = the time for the next difficulty to start")]
    [SerializeField] float timeBetweenDifficulties = 3f;
    
    private GameController gameController;
    private Transform player;
    public int difficultyLevel = 0;
    private bool inTutorial = true;
    // Start is called before the first frame update
    void Start()
    {
        gameController = GetComponent<GameController>();
        player = GameObject.Find("Player(Clone)").transform;
        StartCoroutine(DifficultyDuration());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void RaiseDifficulty(){
        difficultyLevel++;
        gameController.NewDifficulty();
    }

    private IEnumerator DifficultyDuration(){
        if(inTutorial){
            yield return new WaitForSeconds(15);
            inTutorial = false;
            Debug.Log("difficultyIncreased");
            RaiseDifficulty();
        }
        else{
            yield return new WaitForSeconds(7 + (difficultyLevel * timeBetweenDifficulties));
            RaiseDifficulty();
        }
        StartCoroutine(DifficultyDuration());
        
    }
}
