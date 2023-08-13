using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Tooltip("Ensure the platform starts at Pos1")]
    [SerializeField] Transform pos1, pos2;
    [Tooltip("platform's moive speed...")]
    [SerializeField] float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        //platform must start at pos1 on scene load
        StartCoroutine(MovePlatform(pos2.position));
    }
    private IEnumerator MovePlatform(Vector3 nextPosition){
        Vector3 oldPosition = transform.position;
        yield return new WaitForSeconds(2f);
        while(transform.position != nextPosition){
            yield return new WaitForSeconds(0.01f);
            transform.position = Vector3.MoveTowards(transform.position, nextPosition, moveSpeed);
            
        }
        StartCoroutine(MovePlatform(oldPosition));
        
    }
}
