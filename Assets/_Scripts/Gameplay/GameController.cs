using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Enimies{
    public GameObject enimyPrefab;
    public bool canSpawnInWalls;
}

public class GameController : MonoBehaviour
{
    [Header("Base")]
    public GameObject playerPrefab;
    public CameraController_Scrolling cameraController;

    [Header("Checkpoints / Spawn")]
    public Vector3 lastCheckPoint;
    public Transform playerSpawn;

    [Header("Speed")]
    public float speedIncreaseAmount = .25f;
    public float maxSpeed = 4f;

    [Header("EnemyConfig")]
    [Tooltip("Max number of enimies allowed on screen at any given time")]
    [SerializeField] int maxEnemies;
    [Tooltip("Arrange the enimies in the order we want the player to be introduced to them(index 0 is first)")]
    public Enimies[] enemiesScaledUp;
    [Tooltip("this value + 5/difficultyLevel = less time between EnemyTypeSpawns over time")]
    public float timeBetweenEnemyTypes = 5f;
    public int maxSingleEnemySpawnRate, minSingleEnemySpawnRate;
    public BoxCollider2D spawnZone;

    public LayerMask groundMask;
    public LayerMask impassableMask;

    private int difficultyLevel = 0;
    private Transform cameraHolder;
    // Start is called before the first frame update
    private void Awake() {
        lastCheckPoint = playerSpawn.position;
        GameObject go = Instantiate(playerPrefab, playerSpawn.position, Quaternion.identity);
        go.GetComponent<PlayerController>().gameController = this;
        cameraHolder = GameObject.Find("CameraHolder").transform;
    }
    private void Start() {
    }

    public void SetCheckPoint(Vector3 checkpoint) {
        lastCheckPoint = checkpoint;
    }

    private void Update() {
        
    }
    public void NewDifficulty(){
        StartCoroutine(ChoseEnemiesFromDifficulty());
        IncreaseSpeed();
    }

    public void IncreaseSpeed() {
        cameraController.speed = Mathf.Clamp(cameraController.speed + speedIncreaseAmount, 2, maxSpeed);
    }
    private IEnumerator ChoseEnemiesFromDifficulty() {
        difficultyLevel = GetComponent<DifficultyManager>().difficultyLevel;
        int totalRoundEnemyCount = difficultyLevel;
        int remainingCount = totalRoundEnemyCount;

        if(difficultyLevel <= 5){
            PreEndlessMode(difficultyLevel);
            yield break;
        }

        

        if(difficultyLevel >= maxEnemies){
            totalRoundEnemyCount = maxEnemies;
        }
        //loop through enemy types(endless mode)
        for (int i = 0; i < enemiesScaledUp.Length; i++){
            int enemyDifficultyLevel = i;
            yield return new WaitForSeconds(timeBetweenEnemyTypes + (5f/difficultyLevel));

            if(i == enemiesScaledUp.Length-1){
                Debug.Log(remainingCount);
                StartCoroutine(SpawnEnimiesOfType(enemiesScaledUp[i].enimyPrefab, enemyDifficultyLevel, remainingCount, enemiesScaledUp[i].canSpawnInWalls));
                break;
            }
            //if there's still bots to spawn, pick how many of the current enemy type should spawn for this round
            if(remainingCount == 0) break;
            int currentEnemyAmount = Random.Range(0, remainingCount);
            

            //spawn the currently selected enemies as long as there was space for it
            if(currentEnemyAmount != 0 ){
                StartCoroutine(SpawnEnimiesOfType(enemiesScaledUp[i].enimyPrefab, enemyDifficultyLevel, currentEnemyAmount, enemiesScaledUp[i].canSpawnInWalls));
            }
            //set the number of bots left to spawn.
            remainingCount -= currentEnemyAmount;
            if(remainingCount < 0){
                remainingCount = 0;
            }
        }
    }

    private void PreEndlessMode(int difficulty){
        if(difficulty == 1){

            StartCoroutine(SpawnEnimiesOfType(enemiesScaledUp[0].enimyPrefab, 1, 2, enemiesScaledUp[0].canSpawnInWalls));
        }
        else if(difficulty == 2){
            StartCoroutine(SpawnEnimiesOfType(enemiesScaledUp[0].enimyPrefab, 1, 5, enemiesScaledUp[0].canSpawnInWalls));
        }
        else if(difficulty == 3){
            StartCoroutine(SpawnEnimiesOfType(enemiesScaledUp[1].enimyPrefab, 1, 1, enemiesScaledUp[1].canSpawnInWalls));
            StartCoroutine(SpawnEnimiesOfType(enemiesScaledUp[0].enimyPrefab, 1, 2, enemiesScaledUp[0].canSpawnInWalls));
        }
        else if(difficulty == 4){
            StartCoroutine(SpawnEnimiesOfType(enemiesScaledUp[0].enimyPrefab, 1, 2, enemiesScaledUp[0].canSpawnInWalls));
            StartCoroutine(SpawnEnimiesOfType(enemiesScaledUp[1].enimyPrefab, 1, 2, enemiesScaledUp[1].canSpawnInWalls));
        }
        else if(difficulty == 5){
            StartCoroutine(SpawnEnimiesOfType(enemiesScaledUp[2].enimyPrefab, 1, 1, enemiesScaledUp[2].canSpawnInWalls));
            StartCoroutine(SpawnEnimiesOfType(enemiesScaledUp[1].enimyPrefab, 1, 2, enemiesScaledUp[1].canSpawnInWalls));
            StartCoroutine(SpawnEnimiesOfType(enemiesScaledUp[0].enimyPrefab, 1, 3, enemiesScaledUp[0].canSpawnInWalls));
        }
        
    }

    private IEnumerator SpawnEnimiesOfType(GameObject enemyType, int enemyDifficulty, int spawnAmount, bool canSpawnInWalls){
        for(int i = 0; i < spawnAmount; i++)
        {
            yield return new WaitForSeconds(Random.Range(minSingleEnemySpawnRate, maxSingleEnemySpawnRate));
            if(canSpawnInWalls){
                Instantiate(enemyType, GetRandomPointInsideSpawnArea(), Quaternion.identity);
            }
            else{
                Instantiate(enemyType, GetValidPos(), Quaternion.identity);
            }
            
        }

        // 1/10 chance an extra bot of the current type spawns cuse why not muahahhaa
        int luck = Random.Range(0, 10);
        if(luck == 0){
            yield return new WaitForSeconds(Random.Range(minSingleEnemySpawnRate, maxSingleEnemySpawnRate));
            if(canSpawnInWalls){
                Instantiate(enemyType, GetRandomPointInsideSpawnArea(), Quaternion.identity);
            }
            else{
                Instantiate(enemyType, GetValidPos(), Quaternion.identity);
            }
        }
    }


    Vector3 GetValidPos() {
        while (true)
        {
            Vector3 spawnPos = GetRandomPointInsideSpawnArea();
            if(!Physics2D.OverlapCircle(spawnPos, 0.5f, groundMask)) {
                if(!Physics2D.OverlapCircle(spawnPos, 0.5f, impassableMask)) {
                    return spawnPos;
                }
            }
        }
    }

    public Vector3 GetRandomPointInsideSpawnArea() {
        return new Vector3(Random.Range(spawnZone.bounds.min.x, spawnZone.bounds.max.x), Random.Range(spawnZone.bounds.min.y, spawnZone.bounds.max.y), 0f);
    }

    public void GameOver(){
        GetComponent<UIController>().SaveScoreData();
        cameraHolder.GetComponent<CameraController_Scrolling>().enabled = false;
    }
}
