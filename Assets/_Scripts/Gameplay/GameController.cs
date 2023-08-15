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
        GameObject cam = Instantiate(cameraPrefab, playerSpawn.position, Quaternion.identity);
        cam.GetComponent<CameraFollow>().target = go.transform;
    }

    public void SetCheckPoint(Vector3 checkpoint) {
        lastCheckPoint = checkpoint;
    }

    private void Update() {
        
    }

    IEnumerator SpawnEnemies() {
        yield return new WaitForSeconds(0);
    }
}
