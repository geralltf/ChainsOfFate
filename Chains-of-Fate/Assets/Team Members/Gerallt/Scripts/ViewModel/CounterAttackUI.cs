using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

namespace ChainsOfFate.Gerallt
{
    public class CounterAttackUI : MonoBehaviour
    {
        [SerializeField] private GameObject view;
        [SerializeField] private Button buttonAttack;
        [SerializeField] private TextMeshProUGUI textCounterAttackDamage;
        [SerializeField] private string counterAttackDamageFormat = "{0} counter attack damage";
        
        private Champion attacker;
        private CharacterBase target;
        
        public void Attack_Button_OnClick()
        {
            attacker.CounterAttack(target);
            
            SetVisibility(false);
            
            CombatGameManager.Instance.RaiseCounterAttackCompleteEvent();
        }
        
        public void SetVisibility(bool visibility)
        {
            view.SetActive(visibility);
        }

        private void Start()
        {
            CombatGameManager.Instance.OnCounterAttackEvent += CombatGameManager_OnCounterAttackEvent;
            
            buttonAttack.onClick.AddListener(Attack_Button_OnClick);
            
            SetVisibility(false);
        }

        private void OnDestroy()
        {
            CombatGameManager.Instance.OnCounterAttackEvent -= CombatGameManager_OnCounterAttackEvent;
        }

        private void CombatGameManager_OnCounterAttackEvent(CharacterBase attackingCharacter, CharacterBase targetCharacter)
        {
            Champion champion = attackingCharacter as Champion;

            if (champion == null)
            {
                SetVisibility(false);
            }
            else
            {
                attacker = champion;
                target = targetCharacter;

                textCounterAttackDamage.text = string.Format(counterAttackDamageFormat, champion.counterAttackDamage);
            
                SetVisibility(true);
            }
        }
    }
}