using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcanaStat : StatInt
{
    private string _statName = "Arcana";
    public new string StatName { get { return _statName; } set { _statName = value; } }
}
