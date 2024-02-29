using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChainsOfFate.Gerallt;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ChainsOfFate.Gerallt
{
    public class EnemyNPC : CharacterBase, IFleeAction, IAttackAction, IResolveAction, IDefendAction, IInventoryAction
    {
        public float timeUntilMovementResumes = 5.0f;
        
        public EnemyState appliedState = EnemyState.NotSet;
        private bool canFlee = false;
        
        public enum EnemyState : int
        {
            NotSet = 0,
            Defend = 1,
            Flee = 2,
            AttackWithWeapon = 3,
            AttackWithSpell = 4,
            UseItem = 5,
            ResolveEncourage = 6,
            ResolveTaunt = 7,
            Count = 8
        }

        private EnemyState RandomState()
        {
            return (EnemyState)Random.Range(1, (int)EnemyState.Count);
        }
        
        private EnemyState RandomStateExcluding(EnemyState excludingThisState)
        {
            EnemyState newState;
            while (true)
            {
                newState = RandomState();

                if (newState != excludingThisState && newState != EnemyState.Count && newState != EnemyState.NotSet)
                {
                    break;
                }
            }

            return newState;
        }

        public virtual float GetBlockPercentage()
        {
            // Vary agent aptitude at blocking by skill stat in deriving class.
            float blockPercentage = Random.Range(0, 100);
            return blockPercentage;
        }
        
        /// <summary>
        /// Decides a new move to make randomly and applies the move.
        /// </summary>
        public void DecideMove()
        {
            CharacterBase target = GetRandomEnemy();
            CharacterBase agentTarget = GetRandomFriend();
            WeaponBase equippedWeapon = null;
            SpellBase equippedSpell = null;
            ItemBase equippedItem = null;
            
            if (availableWeapons.Count > 0)
            {
                equippedWeapon  = availableWeapons[Random.Range(0, availableWeapons.Count - 1)];
            }
            
            if (availableSpells.Count > 0)
            {
                equippedSpell = availableSpells[Random.Range(0, availableSpells.Count - 1)];
            }
            
            if (availableItems.Count > 0)
            {
                equippedItem = availableItems[Random.Range(0, availableItems.Count - 1)];
            }
            
            EnemyState newState = RandomStateExcluding(EnemyState.NotSet);
            bool tryAgain = false;
            
            canFlee = false;

            // // Process Applied Moves before having turn.
            // float totalDamage = 0;
            // List<AppliedMove> moves = GetMoves();
            // foreach (AppliedMove move in moves)
            // {
            //     if (move is AttackMove)
            //     {
            //         AttackMove attackMove = move as AttackMove;
            //
            //         totalDamage += attackMove.totalDamage;
            //     }
            // }
            //
            // if (totalDamage > 0)
            // {
            //     newState = EnemyState.Defend;
            // }
            // ClearMoves();
            
            do
            {
                tryAgain = false;
                switch (newState)
                {
                    case EnemyState.Defend:
                        // Defend without activating Quick Time Event / block bar.
                        Defend();
                        break;
                    case EnemyState.Flee:
                        canFlee = Flee();
                        break;
                    case EnemyState.AttackWithWeapon:
                        if (equippedWeapon == null)
                        {
                            newState = RandomStateExcluding(EnemyState.AttackWithWeapon);
                            tryAgain = true;
                        }
                        else
                        {
                            Attack(target, equippedWeapon);
                        }
                        break;
                    case EnemyState.AttackWithSpell:
                        if (equippedSpell == null)
                        {
                            newState = RandomStateExcluding(EnemyState.AttackWithSpell);
                            tryAgain = true;
                        }
                        else
                        {
                            Attack(target, equippedSpell);
                        }
                        break;
                    case EnemyState.UseItem:
                        if (equippedItem == null)
                        {
                            newState = RandomStateExcluding(EnemyState.UseItem);
                            tryAgain = true;
                        }
                        else
                        {
                            UseItem(equippedItem);
                        }
                        break;
                    case EnemyState.ResolveEncourage:
                        if (agentTarget == null)
                        {
                            newState = RandomStateExcluding(EnemyState.ResolveEncourage);
                            tryAgain = true;
                        }
                        else
                        {
                            Encourage(agentTarget);
                        }
                        break;
                    case EnemyState.ResolveTaunt:
                        Taunt(target);
                        break;
                    case EnemyState.Count:
                        throw new Exception("Invalid state selected 'Count'");
                    case EnemyState.NotSet:
                        throw new Exception("Invalid state selected 'NotSet'");
                }
            } while (tryAgain);


            appliedState = newState;

            // COMPLETE.
            StartCoroutine(DelayCompleteTurn());
        }
        
        private IEnumerator DelayCompleteTurn()
        {
            CombatGameManager combatGameManager = CombatGameManager.Instance;
            
            yield return new WaitForSeconds(combatGameManager.havingTurnDelaySeconds);

            // COMPLETED TURN.
            
            // Just raise the completed event immediately.
            combatGameManager.RaiseEnemyCompletedTurn(this);

            bool skipNext = (appliedState == EnemyState.Flee) && !canFlee;
            
            // Notify the game manager that this agent has completed it's turn.
            combatGameManager.FinishedTurn(this, false, skipNext);
        }

        /// <summary>
        /// Select the defensive stance.
        /// </summary>
        public void Defend()
        {
            Debug.Log("ENEMY Test defensive stance selected");
            
            currentState = States.Defending;
        }
        
        /// <summary>
        /// Defend against the specified total damage only if defensive stance is applied.
        /// </summary>
        public void Defend(float blockPercentage, float totalDamage)
        {
            Debug.Log("ENEMY Test defend action, block " + blockPercentage + "% totalDamage " + totalDamage);
            
            int damage = (int)(totalDamage * (blockPercentage / 100.0f));
            
            ApplyDamage(damage);
            
            ResetState();
        }
        
        /// <summary>
        /// Checks if the champion can flee by doing a random dice roll.
        /// </summary>
        /// <returns>
        /// True, if allowed to flee.
        /// </returns>
        public bool Flee()
        {
            Debug.Log("ENEMY Test flee action");

            currentState = States.Fleeing;
            
            return Random.value > 0.5f;
        }

        public void Attack(CharacterBase target, WeaponBase weapon)
        {
            Debug.Log("ENEMY Test attack action - weapon " + weapon.GetName());

            float weaponBaseDamage = weapon.BaseDamage;
            float totalDefense = target.Defense;
            
            if (totalDefense == 0)
            {
                totalDefense = 1.0f; // Don't allow divide by zero.
            }

            // Calculate damage.
            // Weapon Base Damage x (STR/DEF) = Total Damage
            // Where STR is the attackers strength score
            // Where DEF is the targets defense score

            int totalDamage = (int)(weaponBaseDamage * ((float)this.Strength / totalDefense));
            
            Debug.Log("target has a defense = " + totalDefense + " total damage " + totalDamage);
            
            // APPLY DAMAGE to target later. Champions always have to respond to damage after their QTE. 
            target.AddDamage(totalDamage, this);

            currentState = States.AttackingWeapon;
        }
        
        public void Attack(CharacterBase target, SpellBase spell)
        {
            Debug.Log("ENEMY Test attack action - spell " + spell.GetName());

            float spellBaseDamage = spell.BaseDamage;
            float totalDefense = target.Defense;
            
            if (totalDefense == 0)
            {
                totalDefense = 1.0f; // Don't allow divide by zero.
            }

            // Calculate damage.
            // Spell Base Damage x (WIS/DEF) = Total Damage
            // Where WIS is the attackers wisdom score
            // Where DEF is the targets defense score

            int totalDamage = (int)(spellBaseDamage * ((float)this.Wisdom / totalDefense));
            
            Debug.Log("target has a defense = " + totalDefense + " total damage " + totalDamage);
            
            // APPLY DAMAGE to target later. Champions always have to respond to damage after their QTE. 
            target.AddDamage(totalDamage, this);

            // Reduce the arcana by the spell's cost.
            ReduceArcana(spell.SpellCost);
            
            currentState = States.AttackingSpell;
        }
        
        public override void AddDamage(int damage, CharacterBase attacker)
        {
            //base.AddDamage(damage);

            if (currentState == States.Defending)
            {
                // Enemies dont have a QTE so there's no need to show block bar
                float blockPercentage = GetBlockPercentage();
            
                Defend(blockPercentage, damage); // Defend applies the damage immediately.
                
                ResetState();
            }
            else
            {
                // APPLY DAMAGE immediately without activating QTE block bar or Defend action since character was not in a defensive stance.
                ApplyDamage(damage);
            }
        }

        public void UseItem(ItemBase item)
        {
            Debug.Log("ENEMY Test use inventory item action " + item);

            currentState = States.UsingItem;
        }
        
        public void Encourage(CharacterBase targetCharacter)
        {
            // Encourage, the target gains 35% (encouragePercent) of the casting Characters WIS: Wisdom stat to the target's Resolve Gauge.
            int gain = (int)((encouragePercent / 100.0f) * Wisdom);

            Debug.Log("ENEMY Test encourage action target " + targetCharacter.CharacterName + " gains " + gain + " wisdom");
            
            // Loose the same amount of wisdom from casting encourage 
            ApplyWisdom(-gain);
            
            // Target gains specified percentage of this character's wisdom
            targetCharacter.ApplyResolve(gain);
            
            currentState = States.Encouraging;
        }

        public void Taunt(CharacterBase targetCharacter)
        {
            // Taunt, the target looses 35% (tauntPercent) of the casting Characters WIS: Wisdom stat to the target's Resolve Gauge.
            int loss = (int)((tauntPercent / 100.0f) * Wisdom);

            Debug.Log("ENEMY Test taunt action target " + targetCharacter.CharacterName + " looses " + loss + " wisdom");
            
            // Loose the same amount of wisdom from casting encourage 
            ApplyWisdom(-loss);
            
            // Target looses specified percentage of this character's wisdom
            targetCharacter.ApplyResolve(-loss);
            
            currentState = States.Taunting;
        }
        
        public CharacterBase GetRandomEnemy()
        {
            var enemies = CombatGameManager.Instance.turnsQueue.GetChampions();
            CharacterBase target = null;

            if (enemies.Any())
            {
                target = enemies[Random.Range(0, enemies.Count -1)];
            }
            return target;
        }
        
        public CharacterBase GetRandomFriend()
        {
            var friends = CombatGameManager.Instance.turnsQueue.GetEnemies()
                .Where(c=> c.ID != this.ID).ToArray();
            CharacterBase target = null;

            if (friends.Any())
            {
                target = friends[Random.Range(0, friends.Length -1)];
            }
            return target;
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