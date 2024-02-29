using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class WorldInfo : MonoBehaviour
{
    //public float loadRange = 50; // Dont need this anymore. Found dynamically from ground plane collider bounds extents.
    public string leftScene, rightScene, upScene, downScene; // Object type doesn't work in build: references are lost! 
    public bool enableDebugColour = false;
    public Color debugColour;
    
    private Transform player;
    private Vector3 centrePoint;
    private SceneDirection newSceneDirection;
    internal Bounds sceneBounds;
    [SerializeField] private SceneDirection approachingDirection = SceneDirection.Undefined;
    
    private bool leftSceneLoaded, rightSceneLoaded, upSceneLoaded, downSceneLoaded;

    private Scene? thisScene;
    private Scene? theNewScene;
    internal Collider2D thisCollider;

    [SerializeField]
    private List<BuildingBounds> buildingBounds = new ();

    [SerializeField]
    private bool buildingSceneLoaded;
    private string buildingSceneToLoad;

    public enum SceneDirection
    {
        Undefined,
        Left,
        Right,
        Top,
        Bottom
    }
    
    private void Awake()
    {
	    buildingBounds.AddRange(GetComponentsInChildren<BuildingBounds>());
        player = FindObjectOfType<PlayerController>().transform;
        centrePoint = transform.position;

        Tilemap tilemap = GetComponent<Tilemap>();
        
        // Fetch the Collider from the 2D tilemap ground plane
        thisCollider = GetComponent<Collider2D>();

        //UpdateBounds();
        
        Vector3 mapSize = thisCollider.bounds.size;
        centrePoint = transform.position + thisCollider.bounds.center;
        //centrePoint = thisCollider.bounds.center;

        // Get the map bounds for this scene.
        sceneBounds = new Bounds(centrePoint, mapSize);

        if (enableDebugColour)
        {
            GetComponent<TilemapRenderer>().material.color = debugColour;
        }
    }

    private void UpdateBounds()
    {
        Vector3 mapSize = thisCollider.bounds.size;
        //mapSize = tilemap.size;
        //mapSize = tilemap.CellToWorld(tilemap.size);
        
        centrePoint = transform.position + thisCollider.bounds.center;
        //centrePoint = thisCollider.bounds.center;
        
        //Vector3 mapSize = thisCollider.bounds.extents;
        
        // Get the map bounds for this scene.
        sceneBounds = new Bounds(centrePoint, mapSize);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(sceneBounds.center, sceneBounds.size);
    }


    public void ChangeViewMode()
    {
        Grid[] grids = FindObjectsOfType<Grid>(true);
        
        if (ChainsOfFate.Gerallt.GameManager.Instance.cameraMode ==
            ChainsOfFate.Gerallt.GameManager.CameraMode.Isometric)
        {
            foreach (Grid g in grids)
            {
                g.cellLayout = GridLayout.CellLayout.Isometric;
            }
        }
        else
        {
            foreach (Grid g in grids)
            {
                g.cellLayout = GridLayout.CellLayout.Rectangle;
            }
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        ChangeViewMode();
        
        //UpdateBounds();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckPosition();
    }
    

    public static string GetSceneName(Object sceneObj)
    {
        if (sceneObj == null)
        {
            return string.Empty;
        }
        
        //SceneAsset sceneAsset = sceneObj as SceneAsset; // Editor only. SceneAsset Isn't available when project is built.
        ;
        // if (sceneAsset == null)
        // {
        //     throw new ArgumentException(
        //         "Object is meant to be of type 'SceneAsset'. Drag an actual Scene file using the inspector.");
        // }
        
        //return sceneAsset.name;
        
        // Damn! Scenes have to be loaded first before they can be looked up by name!
        // Scene scene = SceneManager.GetSceneByName(sceneObj.name); 
        // if (!scene.IsValid())
        // {
        //     throw new ArgumentException(
        //         "Object is not a valid Scene. Drag an actual Scene file using the inspector and add it to the build scene index in project settings.");
        // }
        
        return sceneObj.name;
    }
    
    public void TryLoadScene(SceneDirection sceneDirection)
    {
        bool sceneLoaded = false;
        string sceneName = string.Empty;
        ChainsOfFate.Gerallt.GameManager gameManager = ChainsOfFate.Gerallt.GameManager.Instance;

        if (gameManager.levelLoadingLock) return; // Can't load new levels because we are in progress of loading another.
        
        switch (sceneDirection)
        {
            case SceneDirection.Left:
                sceneLoaded = leftSceneLoaded;
                sceneName = leftScene;
                break;
            case SceneDirection.Right:
                sceneLoaded = rightSceneLoaded;
                sceneName = rightScene;
                break;
            case SceneDirection.Top:
                sceneLoaded = upSceneLoaded;
                sceneName = upScene;
                break;
            case SceneDirection.Bottom:
                sceneLoaded = downSceneLoaded;
                sceneName = downScene;
                break;
            case SceneDirection.Undefined:
	            sceneLoaded = buildingSceneLoaded;
	            sceneName = buildingSceneToLoad;
	            break;
        }
        
        if (!sceneLoaded)
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                gameManager.levelLoadingLock = true; // Lock out others from loading levels.
                
                switch (sceneDirection)
                {
                    case SceneDirection.Left:
                        leftSceneLoaded = true;
                        break;
                    case SceneDirection.Right:
                        rightSceneLoaded = true;
                        break;
                    case SceneDirection.Top:
                        upSceneLoaded = true;
                        break;
                    case SceneDirection.Bottom:
                        downSceneLoaded = true;
                        break;
                    case SceneDirection.Undefined:
	                    buildingSceneLoaded = true;
	                    break;
                }
                
                newSceneDirection = sceneDirection;
                SceneManager.sceneLoaded += SceneManager_OnSceneLoaded;
                
                gameManager.ShowLevelLoadingIndicator(sceneName);

                StartCoroutine(LoadSceneAsync(sceneName));
            }
        }
    }

    public static IEnumerator LoadSceneAsync(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }

    public bool CheckOutsideBounds(SceneDirection sceneDirection)
    {
        bool outsideBounds = false;
        ChainsOfFate.Gerallt.GameManager gameManager = ChainsOfFate.Gerallt.GameManager.Instance;
        
        // Move the player back into the scene away from the out of bounds zone.
        Vector3 dirBounds = Vector3.zero;
        float dist = 0;
        Vector3 pos = player.position;
        Vector3 boundaryLocation = Vector3.zero;
        string sceneName = string.Empty;
        Vector3 posDelta = Vector3.zero;
        bool boundsTest = false;
        float dt = Time.fixedDeltaTime;
        
        switch (sceneDirection)
        {
            case SceneDirection.Left:
                sceneName = leftScene;
                dirBounds.x += -sceneBounds.extents.x;
                boundaryLocation.x = (sceneBounds.center.x + dirBounds.x);
                dist = (pos.x - boundaryLocation.x);
                posDelta.x = dist * gameManager.outofboundsBounceForce * dt;
                boundsTest = (pos.x < centrePoint.x - (sceneBounds.extents.x - gameManager.boundaryRange));
                break;
            case SceneDirection.Right:
                sceneName = rightScene;
                dirBounds.x += sceneBounds.extents.x;
                boundaryLocation.x = (sceneBounds.center.x + dirBounds.x);
                dist = (pos.x - boundaryLocation.x);
                posDelta.x = dist * gameManager.outofboundsBounceForce * dt;
                boundsTest = (pos.x > centrePoint.x + (sceneBounds.extents.x - gameManager.boundaryRange));
                break;
            case SceneDirection.Top:
                sceneName = upScene;
                dirBounds.y += sceneBounds.extents.y;
                boundaryLocation.y = (sceneBounds.center.y + dirBounds.y);
                dist = (pos.y - boundaryLocation.y);
                posDelta.y = dist * gameManager.outofboundsBounceForce * dt;
                boundsTest = (pos.y > centrePoint.y + (sceneBounds.extents.y - gameManager.boundaryRange));
                break;
            case SceneDirection.Bottom:
                sceneName = downScene;
                dirBounds.y += -sceneBounds.extents.y;
                boundaryLocation.y = (sceneBounds.center.y + dirBounds.y);
                dist = (pos.y - boundaryLocation.y);
                posDelta.y = dist * gameManager.outofboundsBounceForce * dt;
                boundsTest = (pos.y < centrePoint.y - (sceneBounds.extents.y - gameManager.boundaryRange));
                break;
        }

        if (string.IsNullOrEmpty(sceneName) && dist < gameManager.boundaryMinDistance && boundsTest && ChainsOfFate.Gerallt.GameManager.Instance.checkBoundaryCollisions)
        {
            float oldZ = pos.z;
            pos.x += posDelta.x;
            pos.y += posDelta.y;
            pos.z = oldZ;
                
            player.position = pos;

            outsideBounds = true;
        }

        return outsideBounds;
    }
    
    private void SceneManager_OnSceneLoaded(Scene newScene, LoadSceneMode sceneMode)
    {
        SceneManager.sceneLoaded -= SceneManager_OnSceneLoaded;

        // Get the offset from the current map to apply to the new map.
        Vector3 loadOffset = Vector3.zero;
        SceneDirection approaching = SceneDirection.Undefined;
        
        switch (newSceneDirection)
        {
            case SceneDirection.Left:
                loadOffset.x += -sceneBounds.extents.x;
                approaching = SceneDirection.Right;
                break;
            case SceneDirection.Right:
                loadOffset.x += sceneBounds.extents.x;
                approaching = SceneDirection.Left;
                break;
            case SceneDirection.Top:
                loadOffset.y += sceneBounds.extents.y;
                approaching = SceneDirection.Bottom;
                break;
            case SceneDirection.Bottom:
                loadOffset.y += -sceneBounds.extents.y;
                approaching = SceneDirection.Top;
                break;
            case SceneDirection.Undefined:
	            approaching = SceneDirection.Undefined;
	            break;
        }
        loadOffset *= 2.0f; // Double the extents is the full map size.
        
        // Translate the new scene's root objects to new position based on current scene's position.
        GameObject[] rootObjects = newScene.GetRootGameObjects();
        for (int i = 0; i < rootObjects.Length; i++)
        {
            GameObject go = rootObjects[i];

            Tilemap[] tilemaps = go.GetComponentsInChildren<Tilemap>();
            foreach (Tilemap tilemap in tilemaps)
            {
                tilemap.transform.position = transform.position + loadOffset;
            }
            
            Transform rootTransform = go.GetComponent<Transform>();
            WorldInfo worldInfo = go.GetComponentInChildren<WorldInfo>();
            
            if (worldInfo != null)
            {
                worldInfo.sceneBounds.center = worldInfo.thisCollider.bounds.center + transform.position + loadOffset;
                worldInfo.transform.position = transform.position + loadOffset;
                worldInfo.centrePoint = worldInfo.sceneBounds.center;
                worldInfo.thisScene = newScene;
                worldInfo.approachingDirection = approaching;
                //worldInfo.UpdateBounds();
                
                continue;
            }

            if (rootTransform != null)
            {
                rootTransform.position = transform.position + loadOffset;
            }
        }
        
        // Unload old scene since the new scene has loaded.
        if (!thisScene.HasValue)
        {
            thisScene = SceneManager.GetActiveScene();
        }

        theNewScene = newScene;
        
        // Hide the level loading indicator.
        ChainsOfFate.Gerallt.GameManager.Instance.HideLevelLoadingIndicator();
    }

    private void FinishLoading()
    {
        if (theNewScene.HasValue)
        {
            if (!ChainsOfFate.Gerallt.GameManager.Instance.dontUnloadScenes)
            {
                SceneManager.UnloadSceneAsync(thisScene.Value);
            }
            SceneManager.SetActiveScene(theNewScene.Value);

            theNewScene = null;

            // Disengage lock since we are finished loading the level.
            ChainsOfFate.Gerallt.GameManager.Instance.levelLoadingLock = false;
        }
    }

    void CheckLeft(Vector3 pos, ChainsOfFate.Gerallt.GameManager gameManager)
    {
        // Test if finished approaching Left
        if ((pos.x > centrePoint.x - sceneBounds.extents.x + gameManager.unloadRange) && approachingDirection == SceneDirection.Left)
        {
            approachingDirection = SceneDirection.Undefined;
        }
        
        // Left scene
        if (pos.x < centrePoint.x - (sceneBounds.extents.x - gameManager.loadRange) && approachingDirection == SceneDirection.Undefined)
        {
            if(!CheckOutsideBounds(SceneDirection.Left))
            {
                TryLoadScene(SceneDirection.Left);
            }
        }
        if (pos.x < centrePoint.x - sceneBounds.extents.x)
        {
            FinishLoading();
        }
    }
    
    void CheckRight(Vector3 pos, ChainsOfFate.Gerallt.GameManager gameManager)
    {
        // Test if finished approaching Right
        if ((pos.x < centrePoint.x + sceneBounds.extents.x - gameManager.unloadRange) && approachingDirection == SceneDirection.Right)
        {
            approachingDirection = SceneDirection.Undefined;
        }
        
        // Right scene
        if (pos.x > centrePoint.x + (sceneBounds.extents.x - gameManager.loadRange) && approachingDirection == SceneDirection.Undefined)
        {
            if(!CheckOutsideBounds(SceneDirection.Right))
            {
                TryLoadScene(SceneDirection.Right);
            }
        }
        if (pos.x > centrePoint.x + sceneBounds.extents.x)
        {
            FinishLoading();
        }
    }
    
    void CheckTop(Vector3 pos, ChainsOfFate.Gerallt.GameManager gameManager)
    {
        // Test if finished approaching Top
        if ((pos.y < centrePoint.y + sceneBounds.extents.y - gameManager.unloadRange) && approachingDirection == SceneDirection.Top)
        {
            approachingDirection = SceneDirection.Undefined;
        }
        
        // Top scene
        if (pos.y > centrePoint.y + (sceneBounds.extents.y - gameManager.loadRange) && approachingDirection == SceneDirection.Undefined)
        {
            if(!CheckOutsideBounds(SceneDirection.Top))
            {
                TryLoadScene(SceneDirection.Top);
            }
        }
        if (pos.y > centrePoint.y + sceneBounds.extents.y)
        {
            FinishLoading();
        }
    }
    
    void CheckBottom(Vector3 pos, ChainsOfFate.Gerallt.GameManager gameManager)
    {
        // Test if finished approaching Bottom
        if ((pos.y > centrePoint.y - sceneBounds.extents.y + gameManager.unloadRange) && approachingDirection == SceneDirection.Bottom)
        {
            approachingDirection = SceneDirection.Undefined;
        }
        
        // Bottom scene
        if (pos.y < centrePoint.y - (sceneBounds.extents.y - gameManager.loadRange) && approachingDirection == SceneDirection.Undefined)
        {
            if(!CheckOutsideBounds(SceneDirection.Bottom))
            {
                TryLoadScene(SceneDirection.Bottom);
            }
        }
        if (pos.y < centrePoint.y - sceneBounds.extents.y)
        {
            FinishLoading();
        }
    }

    void CheckAdHocSceneChange(Vector3 pos, BuildingBounds building, ChainsOfFate.Gerallt.GameManager gameManager)
    {
	    Bounds buildingBounds = building.buildingBounds;
	    bool checkXPosLoad = pos.x > buildingBounds.center.x - buildingBounds.extents.x / 2f 
	                                                         - 1 && pos.x < buildingBounds.center.x + buildingBounds.extents.x / 2f
						 + 1;
	    bool checkYPosLoad = pos.y > buildingBounds.center.y - buildingBounds.extents.y / 2f 
	                     - 1 && pos.y < buildingBounds.center.y + buildingBounds.extents.y / 2f 
						 + 1;
	    
	    if (checkXPosLoad && checkYPosLoad)
	    {
		    buildingSceneToLoad = building.buildingScene;
		    TryLoadScene(SceneDirection.Undefined);
	    }
	    
	    bool checkXPosFinished = pos.x > buildingBounds.center.x - buildingBounds.extents.x / 2f &&
	                     pos.x < buildingBounds.center.x + buildingBounds.extents.x / 2f;
	    bool checkYPosFinished = pos.y > buildingBounds.center.y - buildingBounds.extents.y / 2f &&
	                     pos.y < buildingBounds.center.y + buildingBounds.extents.y / 2f;
	    if (checkXPosFinished && checkYPosFinished)
	    {
		    FinishLoading();
	    }
    }
    
    void CheckPosition()
    {
        if (player.hasChanged)
        {
            Vector3 pos = player.position;
            ChainsOfFate.Gerallt.GameManager gameManager = ChainsOfFate.Gerallt.GameManager.Instance;

            CheckLeft(pos, gameManager);
            CheckRight(pos, gameManager);
            CheckTop(pos, gameManager);
            CheckBottom(pos, gameManager);
            foreach (BuildingBounds building in buildingBounds)
            {
	            CheckAdHocSceneChange(pos, building, gameManager);
            }
        }
    }
}
