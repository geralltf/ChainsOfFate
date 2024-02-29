using UnityEditor;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    [CustomEditor(typeof(StrengthStat))]
    public class StrengthEditor : StatBaseEditor
    {
        public override void OnInspectorGUI()
        {
            ShowStatButtons();

            DrawDefaultInspector();
        }
    }
}