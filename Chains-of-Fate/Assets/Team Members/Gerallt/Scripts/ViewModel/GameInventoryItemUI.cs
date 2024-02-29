using System.Collections;
using System.Collections.Generic;
using ChainsOfFate.Gerallt;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameInventoryItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public InventoryUI parentView;

    private IDescriptive data;

    public void UpdateView(IDescriptive item, int i)
    {
        data = item;
        
        TextMeshProUGUI buttonText = GetComponentInChildren<TextMeshProUGUI>();
        Image buttonImage = GetComponentInChildren<Image>();
                    
        buttonText.text = item.GetName() + " " + i;
        buttonImage.color = item.GetTint();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        parentView.ShowTooltip(this, data);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        parentView.HideTooltip();
    }
}
