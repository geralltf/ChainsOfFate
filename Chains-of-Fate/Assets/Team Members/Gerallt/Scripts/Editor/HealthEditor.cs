using UnityEditor;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    [CustomEditor(typeof(HealthStat))]
    public class HealthEditor : StatBaseEditor
    {
        public override void OnInspectorGUI()
        {
            ShowStatButtons();

            DrawDefaultInspector();
        }
    }
}