using ChainsOfFate.Gerallt;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatInt : MonoBehaviour, IStat
{
    public int AbsoluteMaximum = 1000;
    public int MaxValue = 100;
    public float StartingValue = 100.0f;
    public float Value = 100.0f;

    public float Level = 0.0f;
    public float StartingLevel = 0.0f;
    public int MaxLevel = 100;

    public AnimationCurve animationLevelingCurve;

    public Action<float> OnLevelChanged;
    public Action<float, float> OnValueChanged;

    public CharacterBase ownerCharacterBase;

    public string StatName { get; set; }

    public void Modify(float modifier)
    {
        if(modifier != 0)
        {
            float prev = Value;

            float change = Value + modifier;

            if ((change != prev) && (change <= MaxValue) && (change >= 0.0f))
            {
                Value = change;

                OnValueChanged?.Invoke(Level, Value);
            }
        }
    }

    public bool LevelUp()
    {
        float prev = Level;
        float newLevel = Level + 1;

        if (newLevel >= 0 && newLevel <= MaxLevel)
        {
            Level = newLevel;
        }

        if (Level != prev)
        {
            SelectLevel(Level);

            OnLevelChanged?.Invoke(Level);
            return true;
        }
        return false;
    }

    public bool LevelUp(float newLevel, float MaxLevels)
    {
        float prev = Level;

        if (newLevel >= 0 && newLevel <= MaxLevels)
        {
            Level = newLevel;
        }

        if (Level != prev)
        {
            SelectLevel(Level);

            OnLevelChanged?.Invoke(Level);
            return true;
        }
        return false;
    }

    public bool LevelUp(float ratio)
    {
        float prev = Level;

        if ((ratio >= 0) && ((ratio * MaxLevel) <= MaxLevel))
        {
            Level = (MaxLevel * ratio);
        }

        if (Level != prev)
        {
            SelectLevel(Level);

            OnLevelChanged?.Invoke(Level);
            return true;
        }
        return false;
    }

    public void SelectLevel(float levelSelector)
    {
        SelectLevel(levelSelector, (float)MaxLevel);
    }

    public void SelectLevel(float levelSelector, float maxLevels)
    {
        if (animationLevelingCurve != null)
        {
            float prev = Value;
            float t = levelSelector / maxLevels;

            Value = (float)(MaxValue) * animationLevelingCurve.Evaluate(t);

            if (Value != prev)
            {
                OnValueChanged?.Invoke(levelSelector, Value);
            }
        }
    }

    public object GetMaximum()
    {
        return MaxValue;
    }

    public object GetAbsoluteMaximum()
    {
        return AbsoluteMaximum;
    }

    public bool LevelUp(int newLevel, int maxLevels, bool debugOutput = false)
    {
        return LevelUp((float)newLevel, (float)maxLevels);
    }

    public bool LevelUp(float ratio, bool debugOutput = false)
    {
        return LevelUp(ratio);
    }

    public void Reset()
    {
        Value = StartingValue;
        Level = StartingLevel;

        SelectLevel(StartingLevel);
    }

    public void Replenish()
    {
        float prev = Value;
        Value = MaxValue;

        if(Value != prev)
        {
            OnValueChanged?.Invoke(Level, Value);
        }
    }

    public CharacterBase GetOwner()
    {
        return ownerCharacterBase;
    }

    public virtual void Awake()
    {

    }

    // Start is called before the first frame update
    public virtual void Start()
    {

    }

    // Update is called once per frame
    public virtual void Update()
    {

    }
    public virtual void FixedUpdate()
    {

    }
}
