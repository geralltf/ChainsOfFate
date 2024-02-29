using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChainsOfFate.Gerallt
{
    public class CombatUI : MonoBehaviour
    {
        public CombatGameManager combatGameManager;
        public GameObject view;
        public BlockBarUI blockBarUI;
        public SpriteRenderer SceneBackgroundSpriteRenderer;

        public event Action<CombatUI> onSceneDestroyed;

        public event Action<CombatUI, bool> onCloseCombatUI;
        
        public event Action onSceneLoaded;
        public bool isLoaded;
        
        [Header("For Game Testing, add your own party members and enemies here")]
        public bool isTestMode; // If test mode, will use the test party members

        public Transform EnemyTeam;
        
        public List<GameObject> enemies;
        public List<GameObject> partyMembers;
        public GameObject player;

        public void ResetViewState()
        {
            PlayerButtons playerButtons = view.GetComponentInChildren<PlayerButtons>(true);
            playerButtons.ResetViewState();
            playerButtons.enabled = true;
        }

        public void SetRootGridVisibilities(bool visibility)
        {
            Scene active = SceneManager.GetActiveScene();

            GameObject[] roots = active.GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                GameObject child = roots[i];

                Grid comp = child.GetComponent<Grid>();
                if (comp != null)
                {
                    comp.gameObject.SetActive(visibility); 
                }
            }
            
        }
        
        public void RaiseCloseCombatUI(bool hasWon)
        {
            ResetViewState();

            if (!isTestMode)
            {
                SetRootGridVisibilities(true); // HACK: Because Grids draw on top of block bar and scene background, so lets enable them back.
            }

            onCloseCombatUI?.Invoke(this, hasWon);
        }
        
        private void Awake()
        {
            combatGameManager.OnFleeEvent += CombatGameManagerOnOnFleeEvent;
            onSceneLoaded?.Invoke();
        }

        private void CombatGameManagerOnOnFleeEvent(CharacterBase current, bool canFlee, bool unloadCombatUI)
        {
            if (canFlee && unloadCombatUI)
            {
                RaiseCloseCombatUI(hasWon: false);
            }
        }

        private void OnDestroy()
        {
            onSceneDestroyed?.Invoke(this);
        }

        private void Start()
        {
            if (isTestMode)
            {
                SetCurrentParty(enemies, partyMembers, player);
            }
        }
        
        public void SetCurrentParty(List<GameObject> _enemies, List<GameObject> _partyMembers, GameObject currentPlayer)
        {
            Debug.Log("Got list of current enemies, party members, and current player");

            // ResetViewState();
            
            /*if (!isTestMode)
            {
                CameraFollow cameraFollow = FindObjectOfType<CameraFollow>();
                //Camera _camera = cameraFollow.GetComponent<Camera>();
                Vector3 offset = cameraFollow.GetCenterWorldPosition();

                GameObject sceneBg = SceneBackgroundSpriteRenderer.gameObject;
            
                sceneBg.transform.position = offset;
                sceneBg.transform.rotation = cameraFollow.transform.rotation;
                
                SetRootGridVisibilities(false); // HACK: Because Grids draw on top of block bar and scene background, so lets disable them.
            }*/

            PopulateEnemyTeam(_enemies);
            combatGameManager.SetUpQueue(_enemies, _partyMembers, currentPlayer);
        }

        public void PopulateEnemyTeam(List<GameObject> _enemies)
        {
	        for (int i = 0; i < EnemyTeam.childCount; i++)
	        {
		        Destroy(EnemyTeam.GetChild(i).gameObject);
	        }
	        
	        foreach (GameObject t in _enemies)
	        {
		        Instantiate(t, EnemyTeam);
	        }
        }
    }

}
