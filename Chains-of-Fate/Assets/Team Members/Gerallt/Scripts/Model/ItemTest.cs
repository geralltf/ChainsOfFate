using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class ItemTest : ItemBase
    {
        public int healthModifier;
        public int arcanaModifier;
        public int defenseModifier;
        public int resolveModifier;
        public int strengthModifier;
        public int wisdomModifier;
        public int speedModifier;
        public float movementSpeedModifier;

        public string GetModifiers(CharacterBase user, string newLineFeed = "\n")
        {
            string statsAffected = string.Empty;

            if (healthModifier != 0)
            {
                statsAffected += " " + user.healthStat.StatName + " " + GetBias(healthModifier) + Mathf.Abs(healthModifier) + newLineFeed;
            }
            if (arcanaModifier != 0)
            {
                statsAffected += " " + user.arcanaStat.StatName + " " + GetBias(arcanaModifier) + Mathf.Abs(arcanaModifier) + newLineFeed;
            }
            if (defenseModifier != 0)
            {
                statsAffected += " " + user.defenseStat.StatName + " " + GetBias(defenseModifier) + Mathf.Abs(defenseModifier) + newLineFeed;
            }
            if (resolveModifier != 0)
            {
                statsAffected += " " + user.resolveStat.StatName + " " + GetBias(resolveModifier) + Mathf.Abs(resolveModifier) + newLineFeed;
            }
            if (strengthModifier != 0)
            {
                statsAffected += " " + user.strengthStat.StatName + " " + GetBias(strengthModifier) + Mathf.Abs(strengthModifier) + newLineFeed;
            }
            if (wisdomModifier != 0)
            {
                statsAffected += " " + user.wisdomStat.StatName + " " + GetBias(wisdomModifier) + Mathf.Abs(wisdomModifier) + newLineFeed;
            }
            if (speedModifier != 0)
            {
                statsAffected += " SPEED " + GetBias(speedModifier) + Mathf.Abs(speedModifier) + newLineFeed;
            }
            if (!Mathf.Approximately(movementSpeedModifier, 0.0f))
            {
                statsAffected += " MOV-SPEED " + GetBias(movementSpeedModifier) + Mathf.Abs(movementSpeedModifier) + newLineFeed;
            }
            
            return statsAffected;
        }
        
        public override void UseItem(CharacterBase user)
        {
            string statsAffected = GetModifiers(user, string.Empty);

            if (healthModifier != 0)
            {
                user.ApplyHealth(healthModifier);
            }
            if (arcanaModifier != 0)
            {
                user.ApplyArcana(arcanaModifier);
            }
            if (defenseModifier != 0)
            {
                user.ApplyDefense(defenseModifier);
            }
            if (resolveModifier != 0)
            {
                user.ApplyResolve(resolveModifier);
            }
            if (strengthModifier != 0)
            {
                user.ApplyStrength(strengthModifier);
            }
            if (wisdomModifier != 0)
            {
                user.ApplyWisdom(wisdomModifier);
            }
            if (speedModifier != 0)
            {
                user.Speed += speedModifier;
            }
            if (!Mathf.Approximately(movementSpeedModifier, 0.0f))
            {
                user.MovementSpeed += movementSpeedModifier;
            }

            // Remove item from inventory:
            for (int i = 0; i < user.availableItems.Count; i++)
            {
                ItemBase itemBase = user.availableItems[i];

                if (ID == itemBase.ID)
                {
                    user.availableItems.RemoveAt(i);
                    break;
                }
            }
            
            Debug.Log("Item USE " + GetName() + " " + GetDescription() + statsAffected);
        }

        private string GetBias(int modifier)
        {
            string dir = "";
            if (modifier > 0)
            {
                dir = "+";
            }
            else if (modifier < 0)
            {
                dir = "-"; 
            }

            return dir;
        }
        
        private string GetBias(float modifier)
        {
            string dir = "";
            if (modifier > 0)
            {
                dir = "+";
            }
            else if (modifier < 0)
            {
                dir = "-"; 
            }

            return dir;
        }
    }
}