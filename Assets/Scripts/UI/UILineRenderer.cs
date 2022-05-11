using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineRenderer : MaskableGraphic
{
    public List<Vector2> points;
    private Vector2[] normals;
    float width;
    float height;
    float unitWidth;
    float unitHeight;

    public LineCap lineStart;
    public LineCap lineEnd;

    public float thickness = 10;

    public enum LineCap
    {
        None, 
        Arrow,
        Triangle,
        Box
    }

    protected override void Start()
    {
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

        if(points == null || points.Count < 2)
        {
            return;
        }

        CalculateNormals();
        
        int index = 0;
        for(int i = 0; i < points.Count-1; ++i)
        {
            UIVertex v = UIVertex.simpleVert;
            v.color = color;

            v.position = points[i] + normals[i] * thickness;  // index 0
            vh.AddVert(v);

            v.position = points[i] - normals[i] * thickness; // index 1
            vh.AddVert(v);

            v.position = points[i + 1] + normals[i + 1] * thickness; // index 2
            vh.AddVert(v);

            v.position = points[i + 1] - normals[i + 1] * thickness; // index 3
            vh.AddVert(v);

            vh.AddTriangle(index + 1,index + 0,index + 2);
            vh.AddTriangle(index + 1,index + 2,index + 3);

            index = index + 4;
        }

        DrawEndCaps(vh);
    }
    
    private void CalculateNormals()
    {
        normals = new Vector2[points.Count];
        Vector2[] segmentNormals = new Vector2[points.Count - 1];
        for (int i = 0; i < segmentNormals.Length; ++i)
        {
            float dx = points[i + 1].x - points[i].x;
            float dy = points[i + 1].y - points[i].y;
            segmentNormals[i] = new Vector2(-dy, dx).normalized;
        }

        normals[0] = segmentNormals[0];
        normals[points.Count - 1] = segmentNormals[segmentNormals.Length - 1];

        for(int i = 1; i < points.Count-1; ++i)
        {
            normals[i] = (segmentNormals[i - 1] + segmentNormals[i]).normalized;
        }
    }

    private void DrawEndCaps(VertexHelper vh)
    {
        UIVertex v = UIVertex.simpleVert;
        v.color = color;
        int lastIndex = vh.currentVertCount;
        Vector2 lineDirection = (points[0] - points[1]).normalized;
        Vector2 normal = normals[0];
        Vector2 point = points[0];
        switch (lineStart)
        {
            case LineCap.Arrow:
                v.position = point;
                vh.AddVert(v);

                v.position = new Vector3(point.x + normal.x * thickness * 3 - lineDirection.x * thickness * 1.5f, point.y + normal.y * thickness * 3 - lineDirection.y * thickness * 1.5f);
                vh.AddVert(v);

                v.position = new Vector3(point.x - normal.x * thickness * 3 - lineDirection.x * thickness * 1.5f, point.y - normal.y * thickness * 3 - lineDirection.y * thickness * 1.5f);
                vh.AddVert(v);

                v.position = new Vector3(point.x + lineDirection.x * thickness * 3, point.y + lineDirection.y * thickness * 3);
                vh.AddVert(v);

                vh.AddTriangle(lastIndex + 1, lastIndex + 0, lastIndex + 3);
                vh.AddTriangle(lastIndex + 0, lastIndex + 2, lastIndex + 3);
                break;
            case LineCap.Triangle:
                v.position = new Vector3(point.x + normal.x * thickness * 2, point.y + normal.y * thickness * 2);
                vh.AddVert(v);

                v.position = new Vector3(point.x - normal.x * thickness * 2, point.y - normal.y * thickness * 2);
                vh.AddVert(v);

                v.position = new Vector3(point.x + lineDirection.x * thickness * 3, point.y + lineDirection.y * thickness * 3);
                vh.AddVert(v);

                vh.AddTriangle(lastIndex + 0, lastIndex + 1, lastIndex + 2);
                break;
            case LineCap.Box:
                v.position = new Vector3(point.x + normal.x * thickness * 2 + lineDirection.x * thickness * 2, point.y + normal.y * thickness * 2 + lineDirection.y * thickness * 2);
                vh.AddVert(v);

                v.position = new Vector3(point.x - normal.x * thickness * 2 + lineDirection.x * thickness * 2, point.y - normal.y * thickness * 2 + lineDirection.y * thickness * 2);
                vh.AddVert(v);

                v.position = new Vector3(point.x - normal.x * thickness * 2 - lineDirection.x * thickness * 2, point.y - normal.y * thickness * 2 - lineDirection.y * thickness * 2);
                vh.AddVert(v);

                v.position = new Vector3(point.x + normal.x * thickness * 2 - lineDirection.x * thickness * 2, point.y + normal.y * thickness * 2 - lineDirection.y * thickness * 2);
                vh.AddVert(v);

                vh.AddTriangle(lastIndex + 1, lastIndex + 0, lastIndex + 2);
                vh.AddTriangle(lastIndex + 3, lastIndex + 2, lastIndex + 0);
                break;
        }

        lastIndex = vh.currentVertCount;
        lineDirection = (points[points.Count-1] - points[points.Count-2]).normalized;
        normal = normals[points.Count-1];
        point = points[points.Count-1];

        switch (lineEnd)
        {
            case LineCap.Arrow:
                v.position = point;
                vh.AddVert(v);

                v.position = new Vector3(point.x + normal.x * thickness * 3 - lineDirection.x * thickness * 1.5f, point.y + normal.y * thickness * 3 - lineDirection.y * thickness * 1.5f);
                vh.AddVert(v);

                v.position = new Vector3(point.x - normal.x * thickness * 3 - lineDirection.x * thickness * 1.5f, point.y - normal.y * thickness * 3 - lineDirection.y * thickness * 1.5f);
                vh.AddVert(v);

                v.position = new Vector3(point.x + lineDirection.x * thickness * 3, point.y + lineDirection.y * thickness * 3);
                vh.AddVert(v);

                vh.AddTriangle(lastIndex + 0, lastIndex + 1, lastIndex + 3);
                vh.AddTriangle(lastIndex + 2, lastIndex + 0, lastIndex + 3);
                break;
            case LineCap.Triangle:
                v.position = new Vector3(point.x + normal.x * thickness * 2, point.y + normal.y * thickness * 2);
                vh.AddVert(v);

                v.position = new Vector3(point.x - normal.x * thickness * 2, point.y - normal.y * thickness * 2);
                vh.AddVert(v);

                v.position = new Vector3(point.x + lineDirection.x * thickness * 3, point.y + lineDirection.y * thickness * 3);
                vh.AddVert(v);

                vh.AddTriangle(lastIndex + 1, lastIndex + 0, lastIndex + 2);
                break;
            case LineCap.Box:
                v.position = new Vector3(point.x + normal.x * thickness * 2 + lineDirection.x * thickness * 2, point.y + normal.y * thickness * 2 + lineDirection.y * thickness * 2);
                vh.AddVert(v);

                v.position = new Vector3(point.x - normal.x * thickness * 2 + lineDirection.x * thickness * 2, point.y - normal.y * thickness * 2 + lineDirection.y * thickness * 2);
                vh.AddVert(v);

                v.position = new Vector3(point.x - normal.x * thickness * 2 - lineDirection.x * thickness * 2, point.y - normal.y * thickness * 2 - lineDirection.y * thickness * 2);
                vh.AddVert(v);

                v.position = new Vector3(point.x + normal.x * thickness * 2 - lineDirection.x * thickness * 2, point.y + normal.y * thickness * 2 - lineDirection.y * thickness * 2);
                vh.AddVert(v);

                vh.AddTriangle(lastIndex + 0, lastIndex + 1, lastIndex + 2);
                vh.AddTriangle(lastIndex + 0, lastIndex + 2, lastIndex + 3);
                break;
        }
    }
}
