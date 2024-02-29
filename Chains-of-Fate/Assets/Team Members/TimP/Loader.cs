using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    public string mainScene = "Main";
    public string managersScene = "Managers Scene";
    public bool loadMainScene = false;
    public bool unloadLoaderScene = true;
    
    private Scene thisScene;
    
    private void Awake()
    {
        thisScene = SceneManager.GetActiveScene();
        
        DontDestroyOnLoad(FindObjectOfType<PlayerController>().gameObject);

        Camera[] cams = FindObjectsOfType<Camera>(true);
        foreach (Camera cam in cams)
        {
            DontDestroyOnLoad(cam);
        }

        // Have to enable the mouse cursor for game builds!
        Cursor.visible = true;

        ChainsOfFate.Gerallt.GameManager gameManager = ChainsOfFate.Gerallt.GameManager.Instance;

        if (gameManager == null)
        {
            if (!SceneManager.GetSceneByName(managersScene).isLoaded)
            {
                StartCoroutine(WorldInfo.LoadSceneAsync(managersScene));
                //SceneManager.LoadSceneAsync(managersScene, LoadSceneMode.Additive);
            }
        }

        if (loadMainScene)
        {
            SceneManager.sceneLoaded += SceneManager_OnSceneLoaded;
            
            StartCoroutine(WorldInfo.LoadSceneAsync(mainScene));
            //SceneManager.LoadSceneAsync(mainScene, LoadSceneMode.Additive);
        }
    }

    private void SceneManager_OnSceneLoaded(Scene newScene, LoadSceneMode sceneMode)
    {
        if (newScene.name == mainScene)
        {
            SceneManager.sceneLoaded -= SceneManager_OnSceneLoaded;
            
            if (unloadLoaderScene)
            {
                SceneManager.UnloadSceneAsync(thisScene);
            }
            
            SceneManager.SetActiveScene(newScene);
        }
    }
}
