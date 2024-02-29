using UnityEditor;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    [CustomEditor(typeof(WisdomStat))]
    public class WisdomEditor : StatBaseEditor
    {
        public override void OnInspectorGUI()
        {
            ShowStatButtons();

            DrawDefaultInspector();
        }
    }
}