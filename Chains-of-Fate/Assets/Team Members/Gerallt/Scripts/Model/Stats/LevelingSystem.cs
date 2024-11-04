using ChainsOfFate.Gerallt;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelingSystem : ManagerBase<LevelingSystem>
{
    public float Level;
    public int MaxLevel = 100;
    public List<StatInt> Stats = new List<StatInt>();

    public void LevelUp()
    {
        float prev = Level;

        if(Level + 1 <= MaxLevel)
        {
            Level = Level + 1;
        }

        if(Level != prev && Stats != null)
        {
            foreach (StatInt stat in Stats)
            {
                stat.LevelUp(Level, stat.MaxLevel);
            }
        }
    }

    public void SelectLevel(int levelSelector)
    {
        if (levelSelector >= 0 && Stats != null)
        {
            foreach (StatInt stat in Stats)
            {
                stat.SelectLevel(levelSelector);
            }
        }
    }

    public override void Awake()
    {
        base.Awake();
    }
    public override void Start()
    {
        base.Start();
    }
    public override void Update()
    {
        base.Update();
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}