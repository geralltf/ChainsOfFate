using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrengthStat : StatInt
{
    private string _statName = "Strength";
    public new string StatName { get { return _statName; } set { _statName = value; } }
}