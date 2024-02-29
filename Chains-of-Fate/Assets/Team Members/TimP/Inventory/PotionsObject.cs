using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Potion Object", menuName = "Inventory/Items/Potion")]
public class PotionsObject : ItemObject
{
    public int restoreHealthValue;
    private void Awake()
    {
        type = ItemType.Potion;
    }
}
