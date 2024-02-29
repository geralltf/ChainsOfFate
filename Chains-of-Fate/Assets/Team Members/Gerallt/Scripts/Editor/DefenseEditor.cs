using UnityEditor;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    [CustomEditor(typeof(DefenseStat))]
    public class DefenseEditor : StatBaseEditor
    {
        public override void OnInspectorGUI()
        {
            ShowStatButtons();

            DrawDefaultInspector();
        }
    }
}