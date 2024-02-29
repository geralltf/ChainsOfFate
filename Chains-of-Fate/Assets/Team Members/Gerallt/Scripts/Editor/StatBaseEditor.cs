using UnityEditor;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public abstract class StatBaseEditor : Editor
    {
        private int? level;
        
        protected void ShowStatButtons()
        {
            if (Application.isPlaying)
            {
                if (target is IStat stat)
                {
                    CharacterBase owner = stat.GetOwner();
                    
                    if (GUILayout.Button("Level Up!"))
                    {
                        if (!level.HasValue)
                        {
                            level = owner.Level;
                        }

                        level++;
                        
                        stat.LevelUp(level.Value, LevelingManager.Instance.maxLevels, true);
                    }

                    if (GUILayout.Button("Simulate All Level Ups!"))
                    {
                        if (!level.HasValue)
                        {
                            level = owner.Level;
                        }
                        while(level < LevelingManager.Instance.maxLevels)
                        {
                            level++;
                            stat.LevelUp(level.Value, LevelingManager.Instance.maxLevels, true);
                        }

                        // Reset stat back to default values:
                        stat.Reset();
                        
                        level = owner.Level;
                    }
                    
                    if (GUILayout.Button("Replenish stat!"))
                    {
                        stat.Replenish();
                    }
                    
                    if (GUILayout.Button("Reset stat!"))
                    {
                        stat.Reset();
                        
                        level = owner.Level;
                    }
                }
            }
        }
    }
}