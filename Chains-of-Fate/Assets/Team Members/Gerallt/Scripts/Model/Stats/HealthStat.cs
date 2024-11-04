using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthStat : StatInt
{
    private string _statName = "Health";
    public new string StatName { get { return _statName; } set { _statName = value; } }
}
