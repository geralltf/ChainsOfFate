using System.Collections;
using System.Collections.Generic;
using ChainsOfFate.Gerallt;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class Grunt : EnemyNPC
    {
        /// <summary>
        /// Grunts have a ‘speed type’ that represents how likely it is that they avoid damage.
        /// 40% (Normal)
        /// 25%
        /// 15%
        /// 10%
        /// 7%
        /// 3%
        /// </summary>
        public enum SpeedType
        {
            StNormal = 40, // 40%
            St25Percent = 25, // 25%
            St15Percent = 15, // 15%
            St10Percent = 10, // 10%
            St07Percent = 7, // 7%
            St03Percent = 3,  // 3%
            StRandom0To100 = 0 // Random value between 0 and 100
        }
    
        /// <summary>
        /// Grunts have a ‘speed type’ that represents how likely it is that they avoid damage.
        /// 40% (Normal)
        /// 25%
        /// 15%
        /// 10%
        /// 7%
        /// 3%
        /// </summary>
        public SpeedType speedType = SpeedType.StNormal;
    
        public override float GetBlockPercentage()
        {
            float blockPercentage = 0;
            
            // Vary agent aptitude at blocking by SpeedType skill stat.
            switch (speedType)
            {
                case SpeedType.St03Percent:
                    blockPercentage = 3.0f;
                    break;
                case SpeedType.St07Percent:
                    blockPercentage = 7.0f;
                    break;
                case SpeedType.St10Percent:
                    blockPercentage = 10.0f;
                    break;
                case SpeedType.St15Percent:
                    blockPercentage = 15.0f;
                    break;
                case SpeedType.St25Percent:
                    blockPercentage = 25.0f;
                    break;
                case SpeedType.StNormal:
                    blockPercentage = 40.0f;
                    break;
                case SpeedType.StRandom0To100:
                    blockPercentage = Random.Range(0, 100);
                    break;
            }

            return blockPercentage;
        }
    }

}