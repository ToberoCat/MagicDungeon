using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Visuals;
using Visuals.MessageSystem;
using Random = UnityEngine.Random;

namespace Spells
{
    public class SpellCaster : MonoBehaviour
    {
        [SerializeField] private List<Spell> spells;

        [Header("Spell Casting")] [SerializeField]
        private float spellCastThreshold = 0.6f;

        [FormerlySerializedAs("spellMissMultiplier")] [SerializeField] private float spellMissBound = 2f;

        [Header("Visuals")] [SerializeField] private Camera mainCamera;
        [SerializeField] private UILineRenderer mostSimilarSpellLineRenderer;
        [SerializeField] private UILineRenderer drawnPathLineRenderer;
        [SerializeField] private RectTransform drawingAreaRectTransform;

        private List<Vector2> _drawnPath;
        private Spell _mostSimilarSpell;
        private bool _isDrawing;

        private void Start()
        {
            _drawnPath = new List<Vector2>();
            _isDrawing = false;
            ResetDrawing();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                MouseDown();
            }
            else if (Input.GetMouseButton(0))
            {
                MouseDrag();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                MouseUp();
            }
        }

        private void MouseDown()
        {
            _isDrawing = true;
            _drawnPath.Clear();
        }

        private void MouseDrag()
        {
            if (!_isDrawing)
                return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(drawingAreaRectTransform,
                Input.mousePosition, mainCamera, out var localPos);

            var normalizedPos = new Vector2(
                Mathf.InverseLerp(drawingAreaRectTransform.rect.xMin, drawingAreaRectTransform.rect.xMax, localPos.x),
                Mathf.InverseLerp(drawingAreaRectTransform.rect.yMin, drawingAreaRectTransform.rect.yMax, localPos.y)
            );

            if (_drawnPath.Count > 0 && Vector2.Distance(_drawnPath.Last(), normalizedPos) < 0.1f)
                return;

            _drawnPath.Add(normalizedPos);
            var drawnPath = SpellRecognizer.NormalizeDrawing(_drawnPath);

            var recognizedSpells = SpellRecognizer.RecognizeSpells(spells, drawnPath);
            _mostSimilarSpell = recognizedSpells[0].Spell;
            DrawToLineRenderer(_mostSimilarSpell.path.ToList(), mostSimilarSpellLineRenderer);
            DrawToLineRenderer(_drawnPath, drawnPathLineRenderer);
        }

        private void MouseUp()
        {
            _isDrawing = false;
            if (_drawnPath.Count > 1)
            {
                var normalizedPath = SpellRecognizer.NormalizeDrawing(_drawnPath);
                var recognizedSpells = SpellRecognizer.RecognizeSpells(spells, normalizedPath);
                CastSpell(recognizedSpells[0]);
            }

            ResetDrawing();
        }

        private void ResetDrawing()
        {
            _drawnPath.Clear();
            DrawToLineRenderer(_drawnPath, drawnPathLineRenderer);
            DrawToLineRenderer(_drawnPath, mostSimilarSpellLineRenderer);
        }

        private void DrawToLineRenderer(List<Vector2> path, UILineRenderer lineRenderer)
        {
            var points = new Vector2[path.Count];
            for (var i = 0; i < path.Count; i++)
            {
                points[i] = path[i] * drawnPathLineRenderer.rectTransform.rect.size;
            }
            lineRenderer.points = points;
            lineRenderer.SetAllDirty();
        }

        private void CastSpell(RecognizedSpell recognizedSpell)
        {
            if (recognizedSpell.Similarity < spellCastThreshold &&
                Random.Range(0f, spellMissBound) >= recognizedSpell.Similarity)
            {
                NotificationManager.Instance.ShowPlayerNotification("Casting failed!");
                return;
            }

            NotificationManager.Instance.ShowPlayerNotification($"Casting {recognizedSpell.Similarity:P0} {recognizedSpell.Spell.spellName}!");
            recognizedSpell.Spell.CastSpell(Player.Instance, recognizedSpell.Similarity);
        }
    }
}