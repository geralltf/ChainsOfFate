using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class BarHitSquare : MonoBehaviour
    {
        public BlockBarUI ParentView;

        /// <summary>
        /// The percentage of damage blocked when shield hits this hit square.
        /// </summary>
        public float defenseBlockPerentage;

        public bool isCounterAttack;
    }
}
