using UnityEditor;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    [CustomEditor(typeof(Grunt))]
    public class GruntEditor : CharacterBaseEditor
    {
        public override void OnInspectorGUI()
        {
            ShowCharacterButtons();

            DrawDefaultInspector();
        }
    }
}