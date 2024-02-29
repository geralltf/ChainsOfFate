using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ChainsOfFate.Gerallt
{
    public class GameManager : SingletonBase<GameManager>
    {
        public float outofboundsBounceForce = 10.0f;
        public bool checkBoundaryCollisions = true;
        public float boundaryMinDistance = 10.0f;
        public float loadRange = 10.0f;
        public float unloadRange = 15.0f;
        public float boundaryRange = 3.0f;
        public bool dontUnloadScenes = false;
        public float loadingAnimationSpeed = 1.0f;
        public Color fadeInColourStart;
        public Color fadeInColourEnd;
        internal bool levelLoadingLock = false;
        
        public CombatUI combatUI;
        public GameObject levelLoadingIndicatorUI;
        public TextMeshProUGUI levelLoadingUIText;
        public float levelLoadingTime = 5.0f;

        private bool shownIndicator = false;
        private bool animatingIndicator = false;
        [CanBeNull] public Material materialLoadingBackground;

        private Champion mainCharacter;
        private PlayerController playerController;

        public CameraMode cameraMode = CameraMode.Isometric;

        public float spawnOrthoZ = -14.86f;
        public float spawnPerspectiveZ = 0;
        public float spawnZ = -14.86f;
        
        
        // HACK: To get player and enemies to line up in correct position when camera mode is in top down
        public Vector3 isometricPlayerOffset = Vector3.zero;
        public Vector3 isometricCameraOffset = Vector3.zero;
        
        public enum CameraMode
        {
            Isometric,
            TopDown
        }

        public Champion GetPlayer()
        {
            return mainCharacter;
        }
        
        public CharacterBase GetMainCharacter()
        {
            return (CharacterBase)mainCharacter;
        }

        public PlayerController GetPlayerController()
        {
            return playerController;
        }
        
        public void ShowCombatUI(List<GameObject> enemiesPrefabs)
        {
            combatUI.gameObject.SetActive(true);
            combatUI.SetCurrentParty(enemiesPrefabs,mainCharacter.GetPartyMembers(),mainCharacter.gameObject);
        }
        
        public void HideCombatUI()
        {
            combatUI.gameObject.SetActive(false);
        }

        public void ShowLevelLoadingIndicator(string sceneName)
        {
            levelLoadingIndicatorUI.SetActive(true);
            
            levelLoadingUIText.text = sceneName + "...";

            if (!animatingIndicator)
            {
                StartCoroutine(ShowIndicator());
            }
        }
        
        public void HideLevelLoadingIndicator()
        {
            if (!shownIndicator)
            {
                StartCoroutine(HideIndicator());
            }
        }

        public static Color RandomColour()
        {
            return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        }
        
        public static Vector4 ToVec4(Color c)
        {
            return new Vector4(c.r, c.g, c.b, c.a);
        }
        
        public IEnumerator ShowIndicator()
        {
            animatingIndicator = true;
            Image image = levelLoadingIndicatorUI.GetComponentInChildren<Image>();
            Material _material = image.material;
            
            
            if (materialLoadingBackground == null)
            {
                Material materialInstance = Resources.Load<Material>("Material/MTL_LoadingIndicator");
                materialLoadingBackground =  Instantiate(materialInstance);
            }

            _material = materialLoadingBackground;
            image.material = materialLoadingBackground;
            
            Color col;// = new Color(fadeInColourStart.r, fadeInColourStart.g, fadeInColourStart.b, fadeInColourStart.a);

            //Color before = _material.GetColor("_Color");
            
            float timeElapsed = 0;
            while (animatingIndicator)
            {
                if (timeElapsed < loadingAnimationSpeed)
                {
                    col = Color.Lerp(fadeInColourStart, fadeInColourEnd, timeElapsed / loadingAnimationSpeed);

                    _material.SetColor("_Color", col);
                    image.material = _material;
                    
                    timeElapsed += Time.deltaTime;
                }
                else
                {
                    animatingIndicator = false;
                }


                yield return new WaitForEndOfFrame();
            }
            
            _material.SetColor("_Color", fadeInColourEnd);
            //image.material.color = before2;
        }
        
        IEnumerator HideIndicator()
        {
            shownIndicator = true;
            yield return new WaitForSeconds(levelLoadingTime);
            levelLoadingIndicatorUI.SetActive(false);
            shownIndicator = false;
        }
        
        public override void Awake()
        {
            base.Awake();
            
            Champion currentPlayer = FindObjectsOfType<Champion>().FirstOrDefault(c=> c.isMainCharacter);
            mainCharacter = currentPlayer;
            playerController = currentPlayer.GetComponent<PlayerController>();
            
            levelLoadingIndicatorUI.SetActive(false);
        }
    }
}