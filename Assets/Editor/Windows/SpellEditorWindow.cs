using System;
using System.Collections.Generic;
using System.Linq;
using Spells;
using UnityEditor;
using UnityEngine;

namespace Editor.Windows
{
    public class SpellEditorWindow : EditorWindow
    {
        private Spell _spell;
        private List<Vector2> _drawnPath;
        private bool _isDrawing;

        public static void ShowWindow(Spell spell)
        {
            var window = GetWindow<SpellEditorWindow>("Spell Editor");
            window._spell = spell;
            window.Initialize();
        }

        private void Initialize()
        {
            _drawnPath = new List<Vector2>(_spell.path ?? Array.Empty<Vector2>());
            _isDrawing = false;
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Draw the spell in the area below:", EditorStyles.boldLabel);

            var drawArea =
                GUILayoutUtility.GetRect(400, 400, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUI.DrawRect(drawArea, new Color(0.9f, 0.9f, 0.9f));

            var e = Event.current;
            var mousePosition = e.mousePosition;

            if (drawArea.Contains(mousePosition))
            {
                switch (e.type)
                {
                    case EventType.MouseDown when e.button == 0:
                        _isDrawing = true;
                        _drawnPath.Clear();
                        break;
                    case EventType.MouseDrag when e.button == 0 && _isDrawing:
                        var localMousePosition = mousePosition - drawArea.position;
                        var normalizedMousePosition = new Vector2(localMousePosition.x / drawArea.width,
                            localMousePosition.y / drawArea.height);
                        _drawnPath.Add(normalizedMousePosition);
                        Repaint();
                        break;
                    case EventType.MouseUp when e.button == 0:
                        _isDrawing = false;
                        break;
                    default:
                        break;
                }
            }

            DrawPath(drawArea, _drawnPath, Color.black);
            if (_spell.path != null) DrawPath(drawArea, _spell.path.ToList(), Color.red);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Recognize"))
            {
                _drawnPath = SpellRecognizer.ConvertDrawing(_drawnPath);
                var recognizedSpells = SpellRecognizer.RecognizeSpells(new List<Spell> { _spell }, _drawnPath);
                Debug.Log($"Drawn shape matches spell with {recognizedSpells[0].Similarity} similarity.");
            }

            if (GUILayout.Button("Clear"))
            {
                _drawnPath.Clear();
            }

            if (GUILayout.Button("Save Spell"))
            {
                SaveSpell();
            }

            GUILayout.EndHorizontal();
        }

        private void DrawPath(Rect drawArea, List<Vector2> path, Color color)
        {
            if (path.Count <= 1)
                return;

            Handles.BeginGUI();
            Handles.color = color;
            for (var i = 1; i < path.Count; i++)
            {
                Handles.DrawLine(
                    new Vector2(path[i - 1].x * drawArea.width, path[i - 1].y * drawArea.height) +
                    drawArea.position,
                    new Vector2(path[i].x * drawArea.width, path[i].y * drawArea.height) +
                    drawArea.position
                );
            }

            Handles.EndGUI();
        }

        private void SaveSpell()
        {
            var simplifiedPath = SpellRecognizer.SimplifyDrawing(_drawnPath);
            var normalizedDrawnPath = SpellRecognizer.NormalizeDrawing(simplifiedPath).ToArray();

            _spell.path = normalizedDrawnPath;
            EditorUtility.SetDirty(_spell);
            AssetDatabase.SaveAssets();
            Close();
        }
    }
}