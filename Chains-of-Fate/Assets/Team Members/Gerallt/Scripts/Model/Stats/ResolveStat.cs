using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolveStat : StatInt
{
    private string _statName = "Resolve";
    public new string StatName { get { return _statName; } set { _statName = value; } }
}