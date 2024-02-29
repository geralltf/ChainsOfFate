using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChainsOfFate.Gerallt
{
    public class PlayerButtonsAttackSet : MonoBehaviour
    {
        public GameObject view;
        public GameObject Scrollbar;
        public GameObject weaponViewPrefab;
        public float itemSpacing = 60.0f;
        public float itemOffset = 0.0f;
        
        public GameObject PlayerButtonsParentView;

        public void WeaponButton_OnClick(WeaponBase weapon)
        {
            CombatGameManager combatGameManager = CombatGameManager.Instance;
            
            CharacterBase currentCharacter = combatGameManager.GetCurrentCharacter();
            IAttackAction attackAction = (IAttackAction)currentCharacter;

            if (attackAction != null)
            {
                CharacterBase target = combatGameManager.attackTarget;
                
                attackAction.Attack(target, weapon); 
                combatGameManager.RaiseAttackEvent(currentCharacter, target);
                
                combatGameManager.FinishedTurn(currentCharacter);
            }
        }

        public void SpellButton_OnClick(SpellBase spell)
        {
            CombatGameManager combatGameManager = CombatGameManager.Instance;
            
            CharacterBase currentCharacter = combatGameManager.GetCurrentCharacter();
            IAttackAction attackAction = (IAttackAction)currentCharacter;

            if (attackAction != null)
            {
                CharacterBase target = combatGameManager.attackTarget;
                
                attackAction.Attack(target, spell); 
                combatGameManager.RaiseAttackEvent(currentCharacter, target);
                
                combatGameManager.FinishedTurn(currentCharacter);
            }
        }
        
        public void BackButton_OnClick()
        {
            this.gameObject.SetActive(false);
            PlayerButtonsParentView.SetActive(true);
        }

        public void PopulateAttacks()
        {
            CombatGameManager combatGameManager = CombatGameManager.Instance;

            CharacterBase currentCharacter = combatGameManager.GetCurrentCharacter();

            ClearView();

            if (currentCharacter != null)
            {
                int i = 0;
                foreach (WeaponBase weapon in currentCharacter.availableWeapons)
                {
                    GameObject weaponUIInstance = Instantiate(weaponViewPrefab, view.transform);
                    Vector3 pos = weaponUIInstance.transform.localPosition;

                    pos.x = (i%3 * itemSpacing) + itemOffset;
                    pos.y = -i/3* itemSpacing * 0.5f - itemOffset*0.5f;
                    pos.z = 0;
                
                    weaponUIInstance.transform.localPosition = pos;

                    weaponUIInstance.GetComponentInChildren<TextMeshProUGUI>().text = weapon.GetName();
                    weaponUIInstance.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        WeaponButton_OnClick(weapon);
                    });
                    i++;
                }
                
                foreach (SpellBase spell in currentCharacter.availableSpells)
                {
                    GameObject spellUIInstance = Instantiate(weaponViewPrefab, view.transform);
                    Vector3 pos = spellUIInstance.transform.localPosition;

                    pos.x = (i%3 * itemSpacing) + itemOffset;
                    pos.y = -i/3* itemSpacing * 0.5f - itemOffset*0.5f;
                    pos.z = 0;
                
                    spellUIInstance.transform.localPosition = pos;

                    spellUIInstance.GetComponentInChildren<TextMeshProUGUI>().text = spell.GetName();
                    spellUIInstance.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        SpellButton_OnClick(spell);
                    });
                    i++;
                }

                //Scrollbar.GetComponent<Scrollbar>().size = 2f/(1+Mathf.Max((i - 1) / 3, 1)); not working?
            }
        }

        public void ClearView()
        {
            for (int i = 0; i < view.transform.childCount; i++)
            {
                Transform child = view.transform.GetChild(i);
                
                DestroyImmediate(child.gameObject);
            }
        }
        
        public void OnEnable()
        {

        }

        public void OnDisable()
        {
            
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