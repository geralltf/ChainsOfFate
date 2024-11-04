using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseStat : StatInt
{
    private string _statName = "Defense";
    public new string StatName { get { return _statName; } set { _statName = value; } }
}