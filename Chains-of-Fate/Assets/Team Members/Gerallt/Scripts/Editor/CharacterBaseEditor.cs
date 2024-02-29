using UnityEditor;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public abstract class CharacterBaseEditor : Editor
    {
        private int? savedLevelTmp;
        
        protected void ShowCharacterButtons()
        {
            if (Application.isPlaying)
            {
                CharacterBase character = target as CharacterBase;

                if (character != null)
                {
                    if (GUILayout.Button("Level Up!"))
                    {
                        if (!savedLevelTmp.HasValue)
                        {
                            savedLevelTmp = character.Level;
                        }
                        
                        character.LevelUp(character.Level + 1, LevelingManager.Instance.maxLevels, true, false);
                    }

                    if (character is Champion)
                    {
                        if (GUILayout.Button("Level Up Event!"))
                        {
                            character.LevelUp(character.Level + 1, LevelingManager.Instance.maxLevels, true, true);
                        }
                    }

                    if (GUILayout.Button("Simulate All Level Ups!"))
                    {
                        //for (int level = character.Level + 1; ; level++)
                        int savedLevel = character.Level;
                        int level = character.Level;
                        bool @continue = true;
                        while(level < LevelingManager.Instance.maxLevels && @continue)
                        {
                            @continue = character.LevelUp(level + 1, LevelingManager.Instance.maxLevels, true, false);

                            @continue = true; 
                            level = character.Level;
                        }

                        // Reset level and stats back to default values:
                        character.Level = savedLevel;
                        character.ResetStats();
                    }
                    
                    if (GUILayout.Button("Replenish stats!"))
                    {
                        character.ReplenishStats();
                    }
                    
                    if (GUILayout.Button("Reset stats!"))
                    {
                        character.ResetStats();
                        
                        if (savedLevelTmp.HasValue)
                        {
                            character.Level = savedLevelTmp.Value;
                            savedLevelTmp = null;
                        }
                    }
                    
                    if (GUILayout.Button("Kill"))
                    {
                        character.HP = 0;
                    }
                }
            }
        }
    }
}