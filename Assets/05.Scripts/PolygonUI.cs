using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(CanvasRenderer))]
public class PolygonUI : MaskableGraphic
{
    // 폴리곤의 꼭지점 리스트
    [SerializeField]
    private List<Vector2> vertices = new List<Vector2>()
    {
        new Vector2(0, 0),
        new Vector2(100, 0),
        new Vector2(100, 100),
        new Vector2(0, 100)
    };

    // 폴리곤 색상
    public Color polygonColor = Color.white;

    // 선 두께 (옵션)
    public float borderWidth = 0f;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (vertices.Count < 3)
            return; // 최소한 3개의 꼭지점이 있어야 다각형을 그릴 수 있음

        // 내부 색상을 위한 정점 추가
        AddPolygonVertices(vh, vertices, polygonColor);

        // 삼각형을 그리기 위한 인덱스 설정
        for (int i = 1; i < vertices.Count - 1; i++)
        {
            vh.AddTriangle(0, i, i + 1);
        }

        // 테두리 그리기 (옵션)
        if (borderWidth > 0f)
        {
            DrawPolygonBorder(vh, vertices, borderWidth);
        }
    }

    // 다각형의 내부 정점 추가
    private void AddPolygonVertices(VertexHelper vh, List<Vector2> vertices, Color color)
    {
        foreach (var vertex in vertices)
        {
            UIVertex uiVertex = UIVertex.simpleVert;
            uiVertex.position = vertex;
            uiVertex.color = color;
            vh.AddVert(uiVertex);
        }
    }

    // 다각형의 테두리 그리기
    private void DrawPolygonBorder(VertexHelper vh, List<Vector2> vertices, float width)
    {
        // 테두리 색상을 정하기 위한 기본 색상
        Color borderColor = Color.black;

        for (int i = 0; i < vertices.Count; i++)
        {
            Vector2 current = vertices[i];
            Vector2 next = vertices[(i + 1) % vertices.Count];

            Vector2 direction = (next - current).normalized;
            Vector2 normal = new Vector2(-direction.y, direction.x) * width;

            UIVertex[] quad = new UIVertex[4];

            quad[0].position = current - normal;
            quad[1].position = current + normal;
            quad[2].position = next + normal;
            quad[3].position = next - normal;

            for (int j = 0; j < 4; j++)
            {
                quad[j].color = borderColor;
                vh.AddVert(quad[j]);
            }

            vh.AddTriangle(vh.currentVertCount - 4, vh.currentVertCount - 3, vh.currentVertCount - 2);
            vh.AddTriangle(vh.currentVertCount - 4, vh.currentVertCount - 2, vh.currentVertCount - 1);
        }
    }

    // 인스펙터에서 꼭지점 리스트를 노출시킴
    public List<Vector2> Vertices
    {
        get => vertices;
        set
        {
            vertices = value;
            SetVerticesDirty();
        }
    }

    // 색상과 테두리 두께 업데이트
    public Color PolygonColor
    {
        get => polygonColor;
        set
        {
            polygonColor = value;
            SetVerticesDirty();
        }
    }

    public float BorderWidth
    {
        get => borderWidth;
        set
        {
            borderWidth = value;
            SetVerticesDirty();
        }
    }
}
