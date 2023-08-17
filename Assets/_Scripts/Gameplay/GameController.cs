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
    public GameObject cameraPrefab;

    

    [Header("Checkpoints / Spawn")]
    public Vector3 lastCheckPoint;
    public Transform playerSpawn;

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

    private int difficultyLevel = 0;
    // Start is called before the first frame update
    private void Awake() {
        difficultyLevel = GetComponent<DifficultyManager>().difficultyLevel;
        lastCheckPoint = playerSpawn.position;
        GameObject go = Instantiate(playerPrefab, playerSpawn.position, Quaternion.identity);
        go.GetComponent<PlayerController>().gameController = this;
        //GameObject cam = Instantiate(cameraPrefab, playerSpawn.position, Quaternion.identity);
        //cam.GetComponent<CameraFollow>().target = go.transform;
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
    }
    private IEnumerator ChoseEnemiesFromDifficulty() {
        int totalRoundEnemyCount = difficultyLevel;
        int remainingCount = totalRoundEnemyCount;

        if(difficultyLevel >= maxEnemies){
            totalRoundEnemyCount = maxEnemies;
        }
        for (int i = 0; i < enemiesScaledUp.Length; i++){
            int enemyDifficultyLevel = i;
            yield return new WaitForSeconds(timeBetweenEnemyTypes + (5f/difficultyLevel));
            if(i == enemiesScaledUp.Length-1){
                StartCoroutine(SpawnEnimiesOfType(enemiesScaledUp[i].enimyPrefab, enemyDifficultyLevel, totalRoundEnemyCount, enemiesScaledUp[i].canSpawnInWalls));
                break;
            }

            int currentEnemyAmount = Random.Range(0, remainingCount);
            remainingCount -= currentEnemyAmount;
            
            if(currentEnemyAmount != 0){
                StartCoroutine(SpawnEnimiesOfType(enemiesScaledUp[i].enimyPrefab, enemyDifficultyLevel, currentEnemyAmount, enemiesScaledUp[i].canSpawnInWalls));
            }
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
                return spawnPos;
            }
        }
    }

    public Vector3 GetRandomPointInsideSpawnArea() {
        return new Vector3(Random.Range(spawnZone.bounds.min.x, spawnZone.bounds.max.x), Random.Range(spawnZone.bounds.min.y, spawnZone.bounds.max.y), 0f);
    }
}
