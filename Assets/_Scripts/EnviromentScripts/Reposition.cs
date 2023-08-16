using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reposition : MonoBehaviour
{
    private Vector3 newPosition;
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] nextPositions = GameObject.FindGameObjectsWithTag("NextSegmentPosition");
        newPosition = transform.position;
        foreach(GameObject nextposition in nextPositions){
            Vector3 currentPosition = nextposition.transform.position;
            if(currentPosition.y > newPosition.y){
                newPosition = currentPosition;
            }
        }
    }

    // Update is called once per frame
    void RepositionTileMap(){
        transform.position = newPosition;
    }
}
