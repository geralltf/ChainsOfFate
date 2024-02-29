using UnityEditor;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    [CustomEditor(typeof(ArcanaStat))]
    public class ArcanaEditor : StatBaseEditor
    {
        public override void OnInspectorGUI()
        {
            ShowStatButtons();

            DrawDefaultInspector();
        }
    }
}