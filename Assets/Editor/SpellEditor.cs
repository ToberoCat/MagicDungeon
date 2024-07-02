using Editor.Windows;
using Spells;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Spell), true)]
    public class SpellEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var spell = (Spell)target;

            if (GUILayout.Button("Open Spell in Editor"))
            {
                SpellEditorWindow.ShowWindow(spell);
            }
        }
    }
}