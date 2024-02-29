using System;
using System.Collections;
using System.Collections.Generic;
using ChainsOfFate.Gerallt;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestTeleporter : MonoBehaviour
{
    
    private bool loadCombatScene = false; // Old way to load combat UI was to actually load the scene.

    public List<GameObject> enemyCombatPrefabs;
    
    private PlayerController playerController;
    private bool collisionsDisabled = false;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (collisionsDisabled) return;
        
        PlayerController _playerController = other.gameObject.GetComponent<PlayerController>();

        if (_playerController != null)
        {
            playerController = _playerController;

            collisionsDisabled = true;
            
            // Disable movement of enemy and player.
            GetComponent<EnemyMove>().enabled = false;
            GetComponent<TestTeleporter>().enabled = false;

            Rigidbody2D enemyRigidbody = GetComponent<Rigidbody2D>();
            enemyRigidbody.velocity = Vector2.zero;
            enemyRigidbody.angularVelocity = 0;
            enemyRigidbody.isKinematic = true;
                
            Rigidbody2D playerRigidbody = playerController.GetComponent<Rigidbody2D>();
            playerRigidbody.velocity = Vector2.zero;
            playerRigidbody.angularVelocity = 0;
            playerRigidbody.isKinematic = true;
            
            
            // New approach to loading combat scene without using Scene Manager.
            ChainsOfFate.Gerallt.GameManager.Instance.ShowCombatUI(enemyCombatPrefabs);

	        CombatUI combatUI = ChainsOfFate.Gerallt.GameManager.Instance.combatUI;
            
            // List<GameObject> enemies = new List<GameObject>();
            // enemies.Add(this.gameObject);

	        // Add current party members from current player to this list
            // List<GameObject> partyMembers = new List<GameObject>();
            // Champion player = ChainsOfFate.Gerallt.GameManager.Instance.GetPlayer();
            // foreach (Champion partyMember in player.partyMembers)
            // {
            //     partyMembers.Add(partyMember.gameObject);
            // }
            
	        
	        playerController.controls.Player.Disable();
	        
	        combatUI.isTestMode = false; 
	        CombatGameManager.Instance.GetBlockBarUI().isTestMode = false;
	        combatUI.onCloseCombatUI += CombatUI_OnCloseCombatUI; 
	        //combatUI.onSceneDestroyed += CombatUI_OnSceneDestroyed;
            //combatUI.SetCurrentParty(enemies, partyMembers, playerController.gameObject);
            
        }
    }

    private void CombatUI_OnCloseCombatUI(CombatUI combatUI, bool hasWon)
    {
        combatUI.onCloseCombatUI -= CombatUI_OnCloseCombatUI;
        
        ChainsOfFate.Gerallt.GameManager.Instance.HideCombatUI();
        
        // Enable movement of enemy and player.
        playerController.controls.Player.Enable();

        Rigidbody2D enemyRigidbody = GetComponent<Rigidbody2D>();
        enemyRigidbody.velocity = Vector2.zero;
        enemyRigidbody.angularVelocity = 0;
        enemyRigidbody.isKinematic = false;

        Rigidbody2D playerRigidbody = playerController.GetComponent<Rigidbody2D>();
        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.angularVelocity = 0;
        playerRigidbody.isKinematic = false;

        if (hasWon)
        {
            // Have won the combat with this enemy. So despawn the enemy.
            Destroy(gameObject);
        }

        StartCoroutine(ResumeFunctionAndMovement());
    }

    private void SceneManager_GeneralTeleport_OnSceneLoaded(Scene newScene, LoadSceneMode sceneMode)
    {
        SceneManager.sceneLoaded -= SceneManager_GeneralTeleport_OnSceneLoaded;
        
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        SceneManager.SetActiveScene(newScene);
    }
    
    private void SceneManagerOnsceneLoaded(Scene newScene, LoadSceneMode sceneMode)
    {
        List<GameObject> enemies = new List<GameObject>();
        enemies.Add(this.gameObject);

        List<GameObject> partyMembers = new List<GameObject>();
        // TODO: add current party members from current player to this list
        
        GameObject[] rootObjects = newScene.GetRootGameObjects();
        
        foreach (GameObject rootObj in rootObjects)
        {
            CombatUI combatUI = rootObj.GetComponent<CombatUI>();
            if (combatUI != null)
            {
                SceneManager.SetActiveScene(newScene);
                
                playerController.controls.Player.Disable();
                
                combatUI.isTestMode = false;
                combatUI.onSceneDestroyed += CombatUI_OnSceneDestroyed;
                combatUI.SetCurrentParty(enemies, partyMembers, playerController.gameObject);
                break;
            }
        }
        
        SceneManager.sceneLoaded-= SceneManagerOnsceneLoaded;
    }

    private void CombatUI_OnSceneDestroyed(CombatUI combatUI)
    {
        combatUI.onSceneDestroyed -= CombatUI_OnSceneDestroyed;
        
        // Enable movement of enemy and player.
        playerController.controls.Player.Enable();
        
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().angularVelocity = 0;
        GetComponent<Rigidbody2D>().isKinematic = false;
        playerController.GetComponent<Rigidbody2D>().isKinematic = false;
        
        StartCoroutine(ResumeFunctionAndMovement());
    }

    IEnumerator ResumeFunctionAndMovement()
    {
        yield return new WaitForSeconds(GetComponent<EnemyNPC>().timeUntilMovementResumes);
        
        TestTeleporter testTeleporter = GetComponent<TestTeleporter>();
        if (testTeleporter != null)
        {
            testTeleporter.enabled = true;
        }
        
        EnemyMove enemyMove = GetComponent<EnemyMove>();
        if (enemyMove != null)
        {
            enemyMove.enabled = true;
        }

        collisionsDisabled = false;
    }
}
