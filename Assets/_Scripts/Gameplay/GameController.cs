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
    public GameObject lostSoul;

    public BoxCollider2D spawnZone;
    // Start is called before the first frame update
    private void Awake() {
        lastCheckPoint = playerSpawn.position;
        GameObject go = Instantiate(playerPrefab, playerSpawn.position, Quaternion.identity);
        go.GetComponent<PlayerController>().gameController = this;
        //GameObject cam = Instantiate(cameraPrefab, playerSpawn.position, Quaternion.identity);
        //cam.GetComponent<CameraFollow>().target = go.transform;
    }
    private void Start() {
        //StartCoroutine(SpawnEnemies());
    }

    public void SetCheckPoint(Vector3 checkpoint) {
        lastCheckPoint = checkpoint;
    }

    private void Update() {
        
    }

    IEnumerator SpawnEnemies() {
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(Random.Range(.2f, 1f));
            Instantiate(lostSoul, GetRandomPointInsideSpawnArea(), Quaternion.identity);
        }
        yield return new WaitForSeconds(3f);
        StartCoroutine(SpawnEnemies());
    }

    public Vector3 GetRandomPointInsideSpawnArea() {
        return new Vector3(Random.Range(spawnZone.bounds.min.x, spawnZone.bounds.max.x), Random.Range(spawnZone.bounds.min.y, spawnZone.bounds.max.y), 0f);
    }
}
