using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


namespace Visuals
{
    /// <summary>
    ///  Renders a line using the UI canvas system.
    /// </summary>
    /// Thank you CGPala for the original UILineRender
    /// https://gist.github.com/CGPala/d1ace7dddbfbe78cd2de2bb9e40f6393
    [RequireComponent(typeof(CanvasRenderer))]
    public class UILineRenderer : Graphic
    {
        [SerializeField] private Texture mTexture;
        [SerializeField] private Rect mUVRect = new(0f, 0f, 1f, 1f);

        public float lineThickness = 2;
        public bool useMargins;
        public Vector2 margin;
        public Vector2[] points;
        public bool relativeSize;

        public override Texture mainTexture => mTexture == null ? s_WhiteTexture : mTexture;

        /// <summary>
        /// Texture to be used.
        /// </summary>
        public Texture Texture
        {
            get => mTexture;
            set
            {
                if (mTexture == value)
                    return;

                mTexture = value;
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// UV rectangle used by the texture.
        /// </summary>
        public Rect UVRect
        {
            get => mUVRect;
            set
            {
                if (mUVRect == value)
                    return;
                mUVRect = value;
                SetVerticesDirty();
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            // requires sets of quads
            if (points == null || points.Length < 2)
                points = new[] { new Vector2(0, 0), new Vector2(0, 0) };
            const int capSize = 24;
            var sizeX = rectTransform.rect.width;
            var sizeY = rectTransform.rect.height;
            var offsetX = -rectTransform.pivot.x * rectTransform.rect.width;
            var offsetY = -rectTransform.pivot.y * rectTransform.rect.height;

            // don't want to scale based on the size of the rect, so this is switchable now
            if (!relativeSize)
            {
                sizeX = 1;
                sizeY = 1;
            }

            // build a new set of points taking into account the cap sizes.
            // would be cool to support corners too, but that might be a bit tough :)
            var pointList = new List<Vector2> { points[0] };
            var capPoint = points[0] + (points[1] - points[0]).normalized * capSize;
            pointList.Add(capPoint);

            // should bail before the last point to add another cap point
            for (var i = 1; i < points.Length - 1; i++)
            {
                pointList.Add(points[i]);
            }

            capPoint = points[^1] -
                       (points[^1] - points[^2]).normalized * capSize;
            pointList.Add(capPoint);
            pointList.Add(points[^1]);

            var tempPoints = pointList.ToArray();
            if (useMargins)
            {
                sizeX -= margin.x;
                sizeY -= margin.y;
                offsetX += margin.x / 2f;
                offsetY += margin.y / 2f;
            }

            vh.Clear();

            var prevV1 = Vector2.zero;
            var prevV2 = Vector2.zero;

            for (var i = 1; i < tempPoints.Length; i++)
            {
                var prev = tempPoints[i - 1];
                var cur = tempPoints[i];
                prev = new Vector2(prev.x * sizeX + offsetX, prev.y * sizeY + offsetY);
                cur = new Vector2(cur.x * sizeX + offsetX, cur.y * sizeY + offsetY);

                var angle = Mathf.Atan2(cur.y - prev.y, cur.x - prev.x) * 180f / Mathf.PI;

                var v1 = prev + new Vector2(0, -lineThickness / 2);
                var v2 = prev + new Vector2(0, +lineThickness / 2);
                var v3 = cur + new Vector2(0, +lineThickness / 2);
                var v4 = cur + new Vector2(0, -lineThickness / 2);

                v1 = RotatePointAroundPivot(v1, prev, new Vector3(0, 0, angle));
                v2 = RotatePointAroundPivot(v2, prev, new Vector3(0, 0, angle));
                v3 = RotatePointAroundPivot(v3, cur, new Vector3(0, 0, angle));
                v4 = RotatePointAroundPivot(v4, cur, new Vector3(0, 0, angle));

                var uvTopLeft = Vector2.zero;
                var uvBottomLeft = new Vector2(0, 1);

                var uvTopCenter = new Vector2(0.5f, 0);
                var uvBottomCenter = new Vector2(0.5f, 1);

                var uvTopRight = new Vector2(1, 0);
                var uvBottomRight = new Vector2(1, 1);

                var uvs = new[] { uvTopCenter, uvBottomCenter, uvBottomCenter, uvTopCenter };

                if (i > 1)
                    SetVh(vh, new[] { prevV1, prevV2, v1, v2 }, uvs);

                if (i == 1)
                    uvs = new[] { uvTopLeft, uvBottomLeft, uvBottomCenter, uvTopCenter };
                else if (i == tempPoints.Length - 1)
                    uvs = new[] { uvTopCenter, uvBottomCenter, uvBottomRight, uvTopRight };

                SetVh(vh, new[] { v1, v2, v3, v4 }, uvs);

                prevV1 = v3;
                prevV2 = v4;
            }

            return;
        }

        private void SetVh(VertexHelper vh, Vector2[] vertices, Vector2[] uvs)
        {
            var lastFour = new List<UIVertex>(4);
            for (var i = 0; i < vertices.Length; i++)
            {
                var vert = UIVertex.simpleVert;
                vert.color = color;
                vert.position = vertices[i];
                vert.uv0 = uvs[i];
                vh.AddVert(vert);
                lastFour.Add(vert);
                if (lastFour.Count >= 4)
                    vh.AddUIVertexQuad(lastFour.ToArray());
            }
        }

        private static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
        {
            var dir = point - pivot; // get point direction relative to pivot
            dir = Quaternion.Euler(angles) * dir; // rotate it
            point = dir + pivot; // calculate rotated point
            return point; // return it
        }
    }
}