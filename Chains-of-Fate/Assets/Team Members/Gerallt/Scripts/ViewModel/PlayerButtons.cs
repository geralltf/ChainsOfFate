using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChainsOfFate.Gerallt
{
    public class PlayerButtons : MonoBehaviour
    {
        public PlayableCharacter playableCharacter;
        public CombatGameManager combatGameManager;
        public GameObject view;
        public CombatUI parentView;
        
        public PlayerButtonsAttackSet AttackButtonSet;
        public PlayerButtonsResolveSet ResolveButtonsSet;
        public PlayerButtonsInventorySet InventoryButtonsSet;
        public PlayerButtonsDefensiveSet DefensiveButtonsSet;

        public PlayerButtonsSelectEnemy SelectEnemyButtonsSet;
        
        public void AttackButton_OnClick()
        {
            view.SetActive(false);
            SelectEnemyButtonsSet.gameObject.SetActive(true);
        }

        public void EnemySelected()
        {
	        SelectEnemyButtonsSet.gameObject.SetActive(false);
            AttackButtonSet.gameObject.SetActive(true);
            
            AttackButtonSet.PopulateAttacks();
        }

        public void InventoryButton_OnClick()
        {
            view.SetActive(false);
            InventoryButtonsSet.gameObject.SetActive(true);
            InventoryButtonsSet.PopulateItems();
        }
        
        public void DefendButton_OnClick()
        {
            view.SetActive(false);
            DefensiveButtonsSet.gameObject.SetActive(true);
        }
        
        public void ResolveButton_OnClick()
        {
            view.SetActive(false);
            ResolveButtonsSet.gameObject.SetActive(true);
        }
        
        public void FleeButton_OnClick()
        {
            Debug.Log("Flee");
            
            CharacterBase currentCharacter = combatGameManager.GetCurrentCharacter();
            IFleeAction fleeAction = (IFleeAction)currentCharacter;
            Champion champion = currentCharacter as Champion;
            
            bool canFlee;
            
            if (fleeAction != null)
            {
                canFlee = fleeAction.Flee();
                
                // If can't flee, the next challenger/enemy in the queue takes a turn via skipToNextChallenger.
                if (!canFlee)
                {
                    combatGameManager.FinishedTurn(currentCharacter, !canFlee); 
                }
            }
            else
            {
                canFlee = true;
            }
            
            if (canFlee && (champion != null && champion.isMainCharacter))
            {
                // Actually quit the combat scene.
                
                //combatGameManager.UnloadScene(); // Old approach.
                combatGameManager.RaiseFleeEvent(currentCharacter, canFlee, true);
            }
            else
            {
                combatGameManager.RaiseFleeEvent(currentCharacter, canFlee, false);
            }
        }

        public void ResetViewState()
        {
            view.SetActive(true);
            
            AttackButtonSet.gameObject.SetActive(false);
            ResolveButtonsSet.gameObject.SetActive(false);
            InventoryButtonsSet.gameObject.SetActive(false);
            DefensiveButtonsSet.gameObject.SetActive(false);
        }
        
        private void OnEnable()
        {
            ResetViewState();
        }

        private void OnDisable()
        {

        }

        private void Awake()
        {
            combatGameManager.OnEnemyHavingTurn += CombatGameManager_OnEnemyHavingTurn;
            combatGameManager.OnEnemyCompletedTurn += CombatGameManager_OnEnemyCompletedTurn;
            combatGameManager.OnChampionHavingNextTurn += CombatGameManager_OnChampionHavingNextTurn;
        }

        private void OnDestroy()
        {
            combatGameManager.OnEnemyHavingTurn -= CombatGameManager_OnEnemyHavingTurn;
            combatGameManager.OnEnemyCompletedTurn -= CombatGameManager_OnEnemyCompletedTurn;
            combatGameManager.OnChampionHavingNextTurn -= CombatGameManager_OnChampionHavingNextTurn;
        }

        private void CombatGameManager_OnEnemyHavingTurn(EnemyNPC currentAgent)
        {
            view.SetActive(false);
            
            // Also hide children views:
            AttackButtonSet.gameObject.SetActive(false);
            ResolveButtonsSet.gameObject.SetActive(false);
            InventoryButtonsSet.gameObject.SetActive(false);
            DefensiveButtonsSet.gameObject.SetActive(false);
        }
        
        private void CombatGameManager_OnEnemyCompletedTurn(EnemyNPC currentAgent)
        {
            view.SetActive(true);
        }
        
        private void CombatGameManager_OnChampionHavingNextTurn(CharacterBase current)
        {
            view.SetActive(true);
            
            // Reset children views:
            AttackButtonSet.gameObject.SetActive(false);
            ResolveButtonsSet.gameObject.SetActive(false);
            InventoryButtonsSet.gameObject.SetActive(false);
            DefensiveButtonsSet.gameObject.SetActive(false);
        }
        
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}