using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace ChainsOfFate.Gerallt
{
    public class CombatGameManager : SingletonBase<CombatGameManager>
    {
        /// <summary>
        /// The delay the agent waits before completing their turn.
        /// </summary>
        public float havingTurnDelaySeconds = 2.0f;
        
        public AnimationType animationTypeNormal = AnimationType.Linear;
        public AnimationType animationTypeRoundOver = AnimationType.Circular;
        
        public CharacterBase ActiveCharacter => GetCurrentCharacter();
        public CharacterBase attackTarget = null; // TODO: have the player select a character to target
        public PriorityQueue turnsQueue;
        public PriorityQueue turnsQueueEnemies;
        public PriorityQueue turnsQueueYours;
        public bool shuffleTurns = false;
        public bool makeDeterministic = false;
        public int seed;
        public int round;
        public int maxRoundsPerGame = 5;
        public bool hasWon = false;
        
        private bool proceedToNextTurn = true;
        private bool proceedToNextRound = true;

        public delegate void ActionableDelegate(CharacterBase current, CharacterBase target);

        public delegate void ResolveDelegate(CharacterBase current, CharacterBase target, bool encourage, bool taunt);

        public delegate void FleeDelegate(CharacterBase current, bool canFlee, bool unloadCombatUI);

        public delegate void ManagerInitialisedQueueDelegate(int enemiesAllocated, int partyMembersAllocated);

        public delegate void CounterAttackDelegate(CharacterBase attacker, CharacterBase target);

        public event ManagerInitialisedQueueDelegate OnManagerInitialisedQueueEvent;
        public event Action<CharacterBase> OnChampionHavingNextTurn;
        public event Action<EnemyNPC> OnEnemyHavingTurn;
        public event Action<EnemyNPC> OnEnemyCompletedTurn;

        public event Action<CharacterBase> OnHavingTurn;
        public event Action<CharacterBase, CharacterBase> OnTurnCompleted;
        public event Action<int> OnRoundAdvance;
        public event Action OnWonGameEvent;
        public event Action OnLostGameEvent;
        public event Action OnGameOverEvent;

        public event CharacterBase.StatChangeDelegate OnStatChanged;
        public event ActionableDelegate OnDefendEvent;
        public event ActionableDelegate OnAttackEvent;
        public event ResolveDelegate OnResolveEncourageEvent;
        public event ResolveDelegate OnResolveTauntEvent;
        public event FleeDelegate OnFleeEvent;
        public event CounterAttackDelegate OnCounterAttackEvent;
        public event Action OnCounterAttackCompleteEvent;
        

        private void OnDestroy()
        {
            // Cleanup.
            var queue = turnsQueue.ToList();

            foreach (CharacterBase character in queue)
            {
                character.OnStatChanged -= Character_OnStatChanged;
            }
        }

        /// <summary>
        /// Sets up the turns queue using existing enemies, player, and party members.
        /// If shuffleTurns is enabled, these lists are shuffled randomly to make the game interesting
        ///
        /// Precondition: Enemies and Party Members must have a CharacterBase component in order for the scheduling to work. 
        /// </summary>
        public void SetUpQueue(List<GameObject> currentEnemies, List<GameObject> partyMembers, GameObject currentPlayer)
        {
            // Unsubscribe from old queue.
            var queueOld = turnsQueue.ToList();
            foreach (CharacterBase character in queueOld)
            {
                character.OnStatChanged -= Character_OnStatChanged;
            }

            turnsQueue.Clear();

            // Player always initially goes first.
            CharacterBase playerCharacter = currentPlayer.GetComponent<CharacterBase>();
            turnsQueue.Enqueue(playerCharacter);

            // Evenly distribute turns but enemy goes first.
            int i = 0;
            GameObject go;
            CharacterBase characterBase;
            bool allocatedEnemies = false;
            bool allocatedParty = false;
            bool distributingTurns = true;
            int enemiesAllocated = 0;
            int partyMembersAllocated = 0;

            if (shuffleTurns)
            {
                if (makeDeterministic)
                {
                    Random.InitState(seed); //TODO: This probably should be put in some general GameManager not here
                }

                ShuffleList(ref currentEnemies);
                ShuffleList(ref partyMembers);
            }

            while (distributingTurns)
            {
                if (i < currentEnemies.Count && currentEnemies.Count > 0)
                {
                    go = currentEnemies[i];
                    characterBase = go.GetComponent<CharacterBase>();

                    if (characterBase != null)
                    {
                        enemiesAllocated++;

                        turnsQueue.Enqueue(characterBase);
                    }
                }
                else
                {
                    allocatedEnemies = true;
                }

                if (i < partyMembers.Count && partyMembers.Count > 0)
                {
                    go = partyMembers[i];
                    characterBase = go.GetComponent<CharacterBase>();

                    if (characterBase != null)
                    {
                        partyMembersAllocated++;

                        turnsQueue.Enqueue(characterBase);
                    }
                }
                else
                {
                    allocatedParty = true;
                }

                if (allocatedEnemies && allocatedParty)
                {
                    // Completed allocating turns.
                    distributingTurns = false;
                }

                i++;
            }

            // Subscribe to character stat changes. 
            var queue = turnsQueue.ToList();
            foreach (CharacterBase character in queue)
            {
                character.OnStatChanged += Character_OnStatChanged;
            }

            // Initilise Combat Game Sequence.
            round = 1;
            if (turnsQueue.hadTurns == null)
            {
                turnsQueue.hadTurns = new List<CharacterBase>();
            }
            else
            {
                turnsQueue.hadTurns.Clear();
            }
            
            turnsQueue.UpdateView();

            // Force update of sub queues.
            turnsQueueEnemies.Clear();
            turnsQueueYours.Clear();
            turnsQueueEnemies.Union(turnsQueue.GetEnemies());
            turnsQueueYours.Union(turnsQueue.GetChampions());
            turnsQueueEnemies.UpdateView();
            turnsQueueYours.UpdateView();
            
            OnManagerInitialisedQueueEvent?.Invoke(enemiesAllocated, partyMembersAllocated);
            OnHavingTurn?.Invoke(playerCharacter);
            OnRoundAdvance?.Invoke(round);
        }

        public void FinishedTurn(CharacterBase character, bool skipToNextChallenger = false,
            bool skipToNextChampion = false)
        {
            if (!turnsQueue.hadTurns.Contains(character))
            {
                turnsQueue.hadTurns.Add(character);
            }

            if (CheckGameOver())
            {
                turnsQueue.animationType = animationTypeNormal;
                if (turnsQueue.hadTurns.Count + 1 >= turnsQueue.Count)
                {
                    turnsQueue.animationType = animationTypeRoundOver;
                }
                
                CharacterBase oldTop = turnsQueue.Top();
                CharacterBase oldEnd = turnsQueue.End();

                if (skipToNextChallenger || skipToNextChampion)
                {
                    if (skipToNextChallenger)
                    {
                        Debug.Log("Skipping to next challenger");

                        // Next challenger/enemy takes a turn via skipToNextChallenger
                        turnsQueue.SkipTo(chr => chr is EnemyNPC);
                    }

                    if (skipToNextChampion)
                    {
                        Debug.Log("Skipping to next champion");

                        // Next champion takes a turn via skipToNextChampion
                        turnsQueue.SkipTo(chr => chr is Champion);
                    }
                }
                else
                {
                    // Remove the current character from the top of the queue and adds it back in at the end.
                    turnsQueue.Dequeue();
                }

                // Sort the turns by character attributes.
                turnsQueue.Sort();

                // Run sanity checks on sorted items.
                turnsQueue.SanityChecks(oldTop, oldEnd);

                // Update the UI with the new queue order.
                turnsQueue.UpdateView();
                
                // Force update of sub queues.
                turnsQueueEnemies.Clear();
                turnsQueueYours.Clear();
                turnsQueueEnemies.Union(turnsQueue.GetEnemies());
                turnsQueueYours.Union(turnsQueue.GetChampions());
                turnsQueueEnemies.UpdateView();
                turnsQueueYours.UpdateView();
                
                // New character assigned by the turn queue.
                CharacterBase currentCharacter = GetCurrentCharacter();
                OnTurnCompleted?.Invoke(character, currentCharacter);
                OnHavingTurn?.Invoke(currentCharacter);

                EnemyNPC agent = currentCharacter as EnemyNPC;

                if (agent != null)
                {
                    // Agent.
                    OnEnemyHavingTurn?.Invoke(agent);

                    AgentHaveTurn(agent);

                    // Agent later on calls FinishedTurn() again when it has finished internally.
                }
                else
                {
                    // Champion.
                    OnChampionHavingNextTurn?.Invoke(currentCharacter);

                    Debug.Log("It's your turn!");
                }
            }
        }
        
        public CombatUI GetCombatUI()
        {
            CombatUI combatUI = transform.parent.GetComponent<CombatUI>(); // HACK: can't always guarantee UI is a parent of game manager 
            return combatUI;
        }
        
        public BlockBarUI GetBlockBarUI()
        {
            return GetCombatUI().blockBarUI;
        }

        private bool CheckGameOver()
        {
            proceedToNextRound = false;
            proceedToNextTurn = true;
            
            List<CharacterBase> enemies = turnsQueue.GetEnemies();
            int defeated = 0;
            foreach (CharacterBase enemy in enemies)
            {
                if (enemy.HP == 0)
                {
                    defeated++;
                }
            }

            if (defeated == enemies.Count)
            {
                proceedToNextTurn = false;
                proceedToNextRound = false;
                    
                GameOver();
            }
            else
            {
                if (turnsQueue.hadTurns.Count == turnsQueue.Count)
                {
                    turnsQueue.hadTurns.Clear();

                    if (round + 1 > maxRoundsPerGame)
                    {
                        proceedToNextTurn = false;
                        proceedToNextRound = false;
                    
                        GameOver();
                    }
                    else
                    {
                        proceedToNextRound = true;
                    
                        round++;
                        OnRoundAdvance?.Invoke(round);
                    }
                }
            }

            return proceedToNextTurn; // Proceed with next turn and round.
        }

        private void GameOver()
        {
            List<CharacterBase> enemies = turnsQueue.GetEnemies();
            int defeated = 0;
            foreach (CharacterBase enemy in enemies)
            {
                if (enemy.HP == 0)
                {
                    defeated++;
                }
            }

            Debug.Log("Combat Game Over!");
            
            // The combat game is over. Return back to the last active scene.
            OnGameOverEvent?.Invoke();

            // Notify UI to show different views for different states.
            if (defeated == enemies.Count)
            {
                hasWon = true;
                Debug.Log("~Won defeated: " + defeated.ToString());
                OnWonGameEvent?.Invoke();
            }
            else
            {
                hasWon = false;
                Debug.Log("~Lost but defeated: " + defeated.ToString());
                OnLostGameEvent?.Invoke();
            }

            if (defeated > 0)
            {
                // Level up the main character.
                CharacterBase mainCharacter = GameManager.Instance.GetMainCharacter();
                
                LevelingManager.Instance.LevelUp(mainCharacter, defeated, enemies.Count);
            }
            
            UnloadScene();
        }

        public void UnloadScene()
        {
            CombatUI combatUI = transform.parent.GetComponent<CombatUI>();
            combatUI.RaiseCloseCombatUI(hasWon);
            
            //SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene()); // OLD Approach was when we were having combat in a separate scene, and having to unload that.
        }

        private void AgentHaveTurn(EnemyNPC agent)
        {
            Debug.Log("Agent having a turn");

            agent.DecideMove();
        }

        public CharacterBase GetCurrentCharacter()
        {
            return turnsQueue.Top();
        }

        private void Character_OnStatChanged(CharacterBase character, string propertyName, object newValue)
        {
            OnStatChanged?.Invoke(character, propertyName, newValue);
        }

        public void RaiseEnemyCompletedTurn(EnemyNPC agent)
        {
            OnEnemyCompletedTurn?.Invoke(agent);
        }

        public void RaiseDefendEvent(CharacterBase current, CharacterBase target)
        {
            OnDefendEvent?.Invoke(current, target);
        }

        public void RaiseAttackEvent(CharacterBase current, CharacterBase target)
        {
            OnAttackEvent?.Invoke(current, target);
        }

        public void RaiseResolveEncourageEvent(CharacterBase current, CharacterBase target)
        {
            OnResolveEncourageEvent?.Invoke(current, target, true, false);
        }

        public void RaiseResolveTauntEvent(CharacterBase current, CharacterBase target)
        {
            OnResolveTauntEvent?.Invoke(current, target, false, true);
        }

        public void RaiseFleeEvent(CharacterBase current, bool canFlee, bool unloadCombatUI)
        {
            OnFleeEvent?.Invoke(current, canFlee, unloadCombatUI);
        }

        public void RaiseCounterAttackEvent(CharacterBase attacker, CharacterBase target)
        {
            OnCounterAttackEvent?.Invoke(attacker, target);
        }
        
        public void RaiseCounterAttackCompleteEvent()
        {
            OnCounterAttackCompleteEvent?.Invoke();
        }

        private static void ShuffleList<T>(ref List<T> list)
        {
            int count = list.Count;
            int newIndex;
            T tmp;

            while (count > 1)
            {
                count--;

                newIndex = Random.Range(0, count + 1);

                tmp = list[newIndex];
                list[newIndex] = list[count];
                list[count] = tmp;
            }
        }

        public override void Awake()
        {
            base.Awake();
        }
    }
}