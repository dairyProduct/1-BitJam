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
    [Tooltip("Name of the scene to load!")]
    [SerializeField] string[] sceneName;

    private int sceneToLoad;
    private Transform player;
    private bool isLoaded = false;
    //[Tooltip("Used for testing - Enable this for Editor loading to see the overall Scene")]
    //public bool loadDuringEditor = false;
    private void Start(){
        player = GameObject.Find("Player(Clone)").transform;
    }
    private void Update()
    {
        /*if(loadDuringEditor && Application.isEditor) 
        {
            EditorSceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive); //additive doesn't work in editor?
            //SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }*/

        if (Vector2.Distance(transform.position, player.position) < loadDistance)
        {
            if (!isLoaded)
            {
                sceneToLoad = Random.Range(0, sceneName.Length) + 2; //picks a random scene from the list of scenes
                SceneManager.LoadSceneAsync(sceneName[sceneToLoad], LoadSceneMode.Additive);
                isLoaded = true;
                //we can reposition everything that gets loaded though a manager in each scene of start. 
                //Players wont see reposition / load because it shouldnt be on screen in that time
            }
        }
        else
        {
            if (isLoaded)
            {
                SceneManager.UnloadSceneAsync(sceneToLoad);
                isLoaded = false;
                
            }
        }
    }



}
