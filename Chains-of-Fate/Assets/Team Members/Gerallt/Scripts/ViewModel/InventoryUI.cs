using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ChainsOfFate.Gerallt
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private GameObject view;
        [SerializeField] private GameObject content;
        [SerializeField] private GameObject itemViewPrefab;
        [SerializeField] private TextMeshProUGUI textCharacterName;
        [SerializeField] private Button buttonFilterWeapons;
        [SerializeField] private Button buttonFilterSpells;
        [SerializeField] private Button buttonFilterItems;
        [SerializeField] private Button buttonFilterAll;
        [SerializeField] private Button buttonQuit;
        [SerializeField] private GameObject tooltipGameObject;
        [SerializeField] private FilterBy filterBy = FilterBy.All;
        [SerializeField] private int xSpaceBetweenItem = 60;
        [SerializeField] private int ySpaceBetweenItems = 60;
        [SerializeField] private int xStart = 0;
        [SerializeField] private int yStart = 0;
        [SerializeField] private int numberOfColumns = 1;
        [SerializeField] private float toggleInSeconds = 0.3f;
        
        private Champion currentPlayer;
        private bool isToggling = false;
        
        public enum FilterBy
        {
            Weapons,
            Spells,
            Items,
            All
        }
        
        public void ItemButton_OnClick(IDescriptive item)
        {
            ItemTest itemTest = item as ItemTest;
            if (itemTest != null)
            {
                itemTest.UseItem(currentPlayer);
                
                // Item has been used and removed, so update inventory view.
                PopulateView();
            }
            else
            {
                Debug.Log("TODO: Inventory Item use " + item.GetName() + " " + item.GetDescription());
            }
        }

        public void PopulateView()
        {
            ClearItems();
            
            tooltipGameObject.SetActive(false);
            
            List<IDescriptive> allItems = currentPlayer.GetInventory();

            switch (filterBy)
            {
                case FilterBy.All:
                    break;
                case FilterBy.Weapons:
                    allItems = allItems.Where(item => item is WeaponBase).ToList();
                    break;
                case FilterBy.Spells:
                    allItems = allItems.Where(item => item is SpellBase).ToList();
                    break;
                case FilterBy.Items:
                    allItems = allItems.Where(item => item is ItemBase).ToList();
                    break;
            }
            
            int i = 0;
            
            //for (int test = 0; test < 100; test++)
            {
                foreach (IDescriptive item in allItems)
                {
                    GameObject itemUIInstance = Instantiate(itemViewPrefab, content.transform);
                    GameInventoryItemUI uiItem = itemUIInstance.GetComponent<GameInventoryItemUI>();
                    Button button = itemUIInstance.GetComponentInChildren<Button>();

                    uiItem.parentView = this;
                    uiItem.UpdateView(item, i);
                    
                    button.onClick.AddListener(() =>
                    {
                        ItemButton_OnClick(item);
                    });

                    i++;
                }
            }

            textCharacterName.text = currentPlayer.CharacterName;
        }

        public void ClearItems()
        {
            for (int i = 0; i < content.transform.childCount; i++)
            {
                Transform child = content.transform.GetChild(i);
                
                Destroy(child.gameObject);
            }
        }

        public void ToggleVisibility()
        {
            if (!isToggling)
            {
                isToggling = true;
                StartCoroutine(ToggleVisibilityCoroutine());
            }
        }

        public void SetVisibility(bool visibility)
        {
            view.SetActive(visibility);
            tooltipGameObject.SetActive(false);
            
            if (visibility)
            {
                InGameUI.Instance.SetVisibility(false);
                
                PopulateView();
            }
        }

        public void ShowTooltip(GameInventoryItemUI itemUI, IDescriptive itemData)
        {
            tooltipGameObject.SetActive(true);

            // Vector3 mouse = Mouse.current.position.ReadValue();
            // Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(mouse);
            // Vector3 mousePosView = Camera.main.ScreenToViewportPoint(mouse);
            //
            // Vector3 local = tooltipGameObject.GetComponent<RectTransform>().InverseTransformPoint(mousePosWorld);
            //
            // RectTransform parentRect = tooltipGameObject.transform.parent.GetComponent<RectTransform>();
            // //parentRect = tooltipGameObject.GetComponent<RectTransform>();
            // RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, mouse, Camera.main, out Vector2 localPoint);

            //tooltipGameObject.GetComponent<RectTransform>().anchoredPosition  = localPoint;
            
            //tooltipGameObject.GetComponent<RectTransform>().position = itemUI.gameObject.GetComponent<RectTransform>().position;

            string msg = string.Empty;
            ItemTest itemTest = itemData as ItemTest;
            if (itemTest != null)
            {
                msg = itemTest.GetDescription() + itemTest.GetModifiers(currentPlayer);
            }
            else
            {
                msg = itemData.GetDescription();
            }

            tooltipGameObject.GetComponentInChildren<TextMeshProUGUI>().text = msg;
        }
        
        public void HideTooltip()
        {
            tooltipGameObject.SetActive(false);
        }
        
        private IEnumerator ToggleVisibilityCoroutine()
        {
            yield return new WaitForSeconds(toggleInSeconds);
            SetVisibility(!view.activeInHierarchy);
            isToggling = false;
        }

        private void Start()
        {
            currentPlayer = GameManager.Instance.GetPlayer();

            SetVisibility(false);
            
            buttonFilterWeapons.onClick.AddListener(() =>
            {
                filterBy = FilterBy.Weapons;
                PopulateView();
            });
            
            buttonFilterSpells.onClick.AddListener(() =>
            {
                filterBy = FilterBy.Spells;
                PopulateView();
            });
            
            buttonFilterItems.onClick.AddListener(() =>
            {
                filterBy = FilterBy.Items;
                PopulateView();
            });
            
            buttonFilterAll.onClick.AddListener(() =>
            {
                filterBy = FilterBy.All;
                PopulateView();
            });
            
            buttonQuit.onClick.AddListener(() =>
            {
                SetVisibility(false);
                ClearItems();
            });
        }

        private void Update()
        {
            if (InputSystem.GetDevice<Keyboard>().iKey.isPressed) // TODO: Use proper new input action binding
            {
                ToggleVisibility();
            }
        }
    }
}