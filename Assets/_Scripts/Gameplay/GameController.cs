using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Base")]
    public GameObject playerPrefab;
    public GameObject cameraPrefab;

    

    [Header("Checkpoints / Spawn")]
    public Vector3 lastCheckPoint;
    public Transform playerSpawn;

    [Header("EnemyPrefabs")]
    public GameObject hand;
    public GameObject eye;
    public GameObject lostSoul;

    public BoxCollider2D spawnZone;
    public LayerMask groundMask;
    // Start is called before the first frame update
    private void Awake() {
        lastCheckPoint = playerSpawn.position;
        GameObject go = Instantiate(playerPrefab, playerSpawn.position, Quaternion.identity);
        go.GetComponent<PlayerController>().gameController = this;
        //GameObject cam = Instantiate(cameraPrefab, playerSpawn.position, Quaternion.identity);
        //cam.GetComponent<CameraFollow>().target = go.transform;
    }
    private void Start() {
        StartCoroutine(SpawnEnemies());
    }

    public void SetCheckPoint(Vector3 checkpoint) {
        lastCheckPoint = checkpoint;
    }

    private void Update() {
        
    }

    IEnumerator SpawnEnemies() {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(Random.Range(.2f, 1f));
            Instantiate(lostSoul, GetRandomPointInsideSpawnArea(), Quaternion.identity);
        }
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(Random.Range(.2f, 1f));
            Instantiate(eye, GetValidPos(), Quaternion.identity);
        }
        yield return new WaitForSeconds(7f);
        StartCoroutine(SpawnEnemies());
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
