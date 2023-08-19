using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Reposition : MonoBehaviour
{
    private Vector3 newPosition;
    public bool isGrid = true;
    // Start is called before the first frame update
    void Start()
    {
        if(isGrid){
            RepositionTileMap();
        }
        else{
            RepositionEntity();
        }
        
    }

    // Update is called once per frame
    void RepositionTileMap(){
        GameObject[] nextPositions = GameObject.FindGameObjectsWithTag("NextSegmentPosition");
        newPosition = Vector3.zero;
        float[] positions = {nextPositions[0].transform.position.y, nextPositions[1].transform.position.y, nextPositions[2].transform.position.y};
        newPosition = new Vector3(0, positions.Min(), 0);
        transform.position = newPosition;
    }

    void RepositionEntity(){
        GameObject[] nextPositions = GameObject.FindGameObjectsWithTag("NextSegmentPosition");
        newPosition = Vector3.zero;
        float[] positions = {nextPositions[0].transform.position.y, nextPositions[1].transform.position.y, nextPositions[2].transform.position.y};
        newPosition = new Vector3(transform.position.x, transform.position.y + positions.Min(), 0);
        transform.position = newPosition;
    }
}
