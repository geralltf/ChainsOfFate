using UnityEditor;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    [CustomEditor(typeof(Champion))]
    public class ChampionEditor : CharacterBaseEditor
    {
        public override void OnInspectorGUI()
        {
            ShowCharacterButtons();

            DrawDefaultInspector();
        }
    }
}