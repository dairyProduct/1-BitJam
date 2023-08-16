using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

//[ExecuteInEditMode]
public class LoadAndUnloadAdditive : MonoBehaviour
{
    
    [Tooltip("Segments will load '1,2,3,1,2,3,1... and cycle as such. This way there's something on screen no matter how fast the screen scrolls")]
    [SerializeField] float loadDistance = 20f;
    [SerializeField] float unloadDistance = 0f;
    [Tooltip("Name of the scene to load!")]
    [SerializeField] string[] sceneName;

    private Transform camera;
    private int sceneToLoad;
    //[Tooltip("Used for testing - Enable this for Editor loading to see the overall Scene")]
    //public bool loadDuringEditor = false;
    private void Start(){
        camera = GameObject.Find("CameraHolder").transform;
    }
    private void Update()
    {
        /*if(loadDuringEditor && Application.isEditor) 
        {
            EditorSceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive); //additive doesn't work in editor?
            //SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }*/

        if (transform.position.y - camera.position.y <= loadDistance)
        {
            UnloadSegment();
            LoadSegement();
        }
        
        Debug.Log(gameObject.name + (transform.position.y - camera.position.y).ToString());
    }

    private void UnloadSegment(){
        //Debug.Log("unloaded: "+ gameObject.name);
        try{
            SceneManager.UnloadSceneAsync(sceneName[sceneToLoad]);
        }
        catch{
            return;
        }
        
    }

    private void LoadSegement(){
        transform.position = new Vector3(0f, transform.position.y+75, 0f);
        sceneToLoad = Random.Range(0, sceneName.Length-1) + 1; //picks a random scene from the list of scenes
        try{
            SceneManager.LoadSceneAsync(sceneName[sceneToLoad], LoadSceneMode.Additive);
        }
        catch{
            return;
        }
        
    }



}
