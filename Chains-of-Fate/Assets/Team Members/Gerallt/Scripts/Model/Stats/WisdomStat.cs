using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WisdomStat : StatInt
{
    private string _statName = "Wisdom";
    public new string StatName { get { return _statName; } set { _statName = value; } }
}