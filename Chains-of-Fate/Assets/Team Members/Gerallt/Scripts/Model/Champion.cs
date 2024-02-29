using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChainsOfFate.Gerallt;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ChainsOfFate.Gerallt
{
    public class Champion : PlayableCharacter, IFleeAction, IAttackAction, IResolveAction, IDefendAction, IInventoryAction
    {
        /// <summary>
        /// If the Champion is the main character.
        /// </summary>
        public bool isMainCharacter;

        
        /// <summary>
        /// The specified damage that is used when this champion does a counter attack.
        /// </summary>
        public int counterAttackDamage;

        public Transform Party;
        public List<Champion> partyMembers;
        public List<GameObject> combatPartyMembers;

        public List<GameObject> GetPartyMembers()
        {
	        foreach (PartyFollow partyMember in Party.GetComponentsInChildren<PartyFollow>())
	        {
		        combatPartyMembers.Add(partyMember.combatPrefab);
	        }

	        return combatPartyMembers;
        }


        /// <summary>
        /// Select the defensive stance.
        /// </summary>
        public void Defend()
        {
            Debug.Log("Test defensive stance selected");
            
            currentState = States.Defending;
        }
        
        /// <summary>
        /// Defend against the last attack only if defensive stance is applied.
        /// </summary>
        public void Defend(float blockPercentage, float totalDamage)
        {
            Debug.Log("Test defend action, block " + blockPercentage + "% totalDamage " + totalDamage);

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
            Debug.Log("Test flee action");

            currentState = States.Fleeing;
            
            //return true;
            return Random.value > 0.5f;
        }

        public void CounterAttack(CharacterBase target)
        {
            // Apply the champions counter attack damage to the target.
            target.ApplyDamage(counterAttackDamage);
        }
        
        public void Attack(CharacterBase target, WeaponBase weapon)
        {
	        Debug.Log(target);
	        Debug.Log("Test attack action - weapon " + weapon.GetName());
            
            if (target == null) // TODO: Implement target selection
            {
                // HACK: Just pick a random enemy for now
                target = GetRandomEnemy();
            }

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
            
            // APPLY DAMAGE to target later. Enemies always have to respond to damage immediately when its their turn. 
            target.AddDamage(totalDamage, this);
             
            currentState = States.AttackingWeapon;
        }

        public void Attack(CharacterBase target, SpellBase spell)
        {
            Debug.Log("Test attack action - spell " + spell.GetName());
            
            if (target == null) // TODO: Implement target selection
            {
                // HACK: Just pick a random enemy for now
                target = GetRandomEnemy();
            }

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
            
            // APPLY DAMAGE to target later. Enemies always have to respond to damage immediately when its their turn. 
            target.AddDamage(totalDamage, this);

            // Reduce the arcana by the spell's cost.
            ReduceArcana(spell.SpellCost);
            
            currentState = States.AttackingSpell;
        }

        public override void AddDamage(int damage, CharacterBase attacker)
        {
            base.AddDamage(damage, attacker);

            if (currentState == States.Defending)
            {
                BlockBarUI blockBarUI = CombatGameManager.Instance.GetBlockBarUI();

                blockBarUI.totalDamageRecieved = damage;
                blockBarUI.onWonEvent += BlockBarUI_OnWonEvent;
                blockBarUI.onLostEvent += BlockBarUI_OnLostEvent;
                blockBarUI.defendingCharacter = this;
                blockBarUI.attackingCharacter = attacker;
                blockBarUI.isTestMode = CombatGameManager.Instance.GetCombatUI().isTestMode;
                blockBarUI.SetVisibility(true);
            }
            else
            {
                // APPLY DAMAGE immediately without activating QTE block bar or Defend action since character was not in a defensive stance.
                ApplyDamage(damage);
            }
        }

        public void UseItem(ItemBase item)
        {
            Debug.Log("Test use inventory item action " + item);

            currentState = States.UsingItem;
        }
        
        public void Encourage(CharacterBase target)
        {
            if (target == null) // TODO: Implement target selection
            {
                // HACK: Just pick a random party member for now
                target = GetRandomFriend();
            }

            if (target == null)
            {
                Debug.Log("Test encourage action invalid target");
                return;
            }
            
            // Encourage, the target gains 35% (encouragePercent) of the casting Characters WIS: Wisdom stat to the target's Resolve Gauge.
            int gain = (int)((encouragePercent / 100.0f) * Wisdom);

            Debug.Log("Test encourage action target " + target.CharacterName + " gains " + gain + " wisdom");
            
            // Loose the same amount of wisdom from casting encourage 
            ApplyWisdom(-gain);
            
            // Target gains specified percentage of this character's wisdom
            target.ApplyResolve(gain);
            
            currentState = States.Encouraging;
        }

        public void Taunt(CharacterBase target)
        {
            if (target == null) // TODO: Implement target selection
            {
                // HACK: Just pick a random enemy for now
                target = GetRandomEnemy();
            }
            
            // Taunt, the target looses 35% (tauntPercent) of the casting Characters WIS: Wisdom stat to the target's Resolve Gauge.
            int loss = (int) ((tauntPercent / 100.0f) * Wisdom);

            Debug.Log("Test taunt action target " + target.CharacterName + " looses " + loss + " wisdom");

            // Loose the same amount of wisdom from casting encourage 
            ApplyWisdom(-loss);

            // Target looses specified percentage of this character's wisdom
            target.ApplyResolve(-loss);

            currentState = States.Taunting;
        }

        public CharacterBase GetRandomEnemy()
        {
            var enemies = CombatGameManager.Instance.turnsQueue.GetEnemies();
            CharacterBase target = null;

            if (enemies.Any())
            {
                target = enemies[Random.Range(0, enemies.Count -1)];
            }
            return target;
        }
        
        public CharacterBase GetRandomFriend()
        {
            var friends = CombatGameManager.Instance.turnsQueue.GetChampions()
                .Where(c=> c.ID != this.ID).ToArray();

            CharacterBase target = null;

            if (friends.Any())
            {
                target = friends[Random.Range(0, friends.Length -1)];
            }

            return target;
        }
        
        private void BlockBarUI_OnLostEvent()
        {
            Cleanup();
            
            // APPLY DAMAGE
            Defend(0, CombatGameManager.Instance.GetBlockBarUI().totalDamageRecieved);
            
            ResetState();
            
            StartCoroutine(CompleteTurnSequence());
        }

        private void BlockBarUI_OnWonEvent(float blockPercentage, bool doCounterAttack)
        {
            Cleanup();

            BlockBarUI blockBarUI = CombatGameManager.Instance.GetBlockBarUI();
            
            // APPLY DAMAGE
            Defend(blockPercentage, blockBarUI.totalDamageRecieved);
            
            ResetState();

            // COUNTERATTACK
            if (doCounterAttack)
            {
                // Raise Counter Attack event for the specified target, so the target can apply a counter attack.
                CombatGameManager.Instance.RaiseCounterAttackEvent(this, blockBarUI.attackingCharacter);
            }
            
            StartCoroutine(CompleteTurnSequence());
        }

        private void Cleanup()
        {
            BlockBarUI blockBarUI = CombatGameManager.Instance.GetBlockBarUI();

            blockBarUI.onWonEvent -= BlockBarUI_OnWonEvent;
            blockBarUI.onLostEvent -= BlockBarUI_OnLostEvent;
        }
        
        private IEnumerator CompleteTurnSequence()
        {
            CombatGameManager combatGameManager = CombatGameManager.Instance;
            BlockBarUI blockBarUI = CombatGameManager.Instance.GetBlockBarUI();

            blockBarUI.SetVisibility(false);
            yield return new WaitForSeconds(CombatGameManager.Instance.havingTurnDelaySeconds);
            
            combatGameManager.FinishedTurn(this);
            combatGameManager.RaiseDefendEvent(this, null);
        }
    }
}