using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace ChainsOfFate.Gerallt
{
    public class PlayerButtonsInventorySet : MonoBehaviour
    {
        public PlayerButtons PlayerButtonsParentView;
        public int XSpaceBetweenItem;
        public int XStart;
        public int YStart;
        public int NumberOfColumn;
        public int YSpaceBetweenItems;
        public GameObject view;
        public GameObject itemViewPrefab;
        public float itemSpacing = 60.0f;
        public float itemOffset = 0.0f;
        
        public void DamagingSpellButton_OnClick()
        {
            Debug.Log("Damaging spell");
        }
        
        public void BackButton_OnClick()
        {
            this.gameObject.SetActive(false);
            PlayerButtonsParentView.view.SetActive(true);
        }
        public Vector3 GetPosition(int i)
        {
            return new Vector3(XStart+(XSpaceBetweenItem*(i%NumberOfColumn)),YStart+(-YSpaceBetweenItems*(i/NumberOfColumn)),0f);
        }
        public void ItemButton_OnClick(ItemBase item)
        {
            CombatGameManager combatGameManager = CombatGameManager.Instance;
            
            CharacterBase currentCharacter = combatGameManager.GetCurrentCharacter();
            IInventoryAction inventoryAction = (IInventoryAction)currentCharacter;

            if (inventoryAction != null)
            {
                CharacterBase target = combatGameManager.attackTarget;
                
                inventoryAction.UseItem(item); 
                //combatGameManager.RaiseAttackEvent(currentCharacter, target);
                
                combatGameManager.FinishedTurn(currentCharacter);
            }
        }
        public void PopulateItems()
        {
            CombatGameManager combatGameManager = CombatGameManager.Instance;

            CharacterBase currentCharacter = combatGameManager.GetCurrentCharacter();

            ClearView();

            if (currentCharacter != null)
            {
                int i = 0;
                foreach (ItemBase item in currentCharacter.availableItems)
                {
                    GameObject itemUIInstance = Instantiate(itemViewPrefab, view.transform);
                    Vector3 pos = itemUIInstance.transform.localPosition;

                    pos.x = (i * itemSpacing) + itemOffset;
                    pos.y = 0;
                    pos.z = 0;
                    pos = GetPosition(i);
                    itemUIInstance.transform.localPosition = pos;

                    itemUIInstance.GetComponentInChildren<TextMeshProUGUI>().text = item.GetName();
                    itemUIInstance.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        ItemButton_OnClick(item);
                    });
                    i++;
                }
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