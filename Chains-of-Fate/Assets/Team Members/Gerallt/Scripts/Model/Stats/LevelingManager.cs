using ChainsOfFate.Gerallt;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelingManager : SingletonBase<LevelingManager>
{
    public int maxLevels = 100;

    public List<LevelingSystem> LevelingSystems = new List<LevelingSystem>();

    public System.Action<CharacterBase, int, int, int, List<IStat>> OnLevelChanged;
    
    public void RaiseLevelUp(CharacterBase characterBase, int oldLevel, int newLevel, int maxLevel, List<IStat> statsAffected)
    {
        OnLevelChanged?.Invoke(characterBase, oldLevel, newLevel, maxLevel, statsAffected);
    }

    public void LevelUp(CharacterBase mainCharacter, int defeated, int enemiesCount)
    {

    }

    public void LevelUp()
    {
        if (LevelingSystems != null)
        {
            foreach (LevelingSystem levelingSystem in LevelingSystems)
            {
                levelingSystem.LevelUp();
            }
        }
    }

    public void SelectLevel(int levelSelector)
    {
        if (LevelingSystems != null)
        {
            foreach (LevelingSystem levelingSystem in LevelingSystems)
            {
                levelingSystem.SelectLevel(levelSelector);
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