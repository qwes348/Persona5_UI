#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(PolygonUI))]
public class PolygonUIEditor : Editor
{
    private PolygonUI polygon2D;

    private void OnEnable()
    {
        polygon2D = (PolygonUI)target;
    }

    private void OnSceneGUI()
    {
        // 꼭지점들을 씬에 핸들로 표시하고 드래그할 수 있게 만듦
        for (int i = 0; i < polygon2D.Vertices.Count; i++)
        {
            Vector2 vertex = polygon2D.Vertices[i];
            Vector3 worldPos = polygon2D.transform.TransformPoint(vertex);

            // 드래그 가능한 핸들을 생성하여 꼭지점 위치를 수정 가능하게
            EditorGUI.BeginChangeCheck();
            Vector3 newWorldPos = Handles.PositionHandle(worldPos, Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(polygon2D, "Move Polygon Vertex");
                Vector2 newLocalPos = polygon2D.transform.InverseTransformPoint(newWorldPos);
                polygon2D.Vertices[i] = newLocalPos;

                // 폴리곤을 업데이트
                polygon2D.SetVerticesDirty();
            }

            // 꼭지점 번호를 씬에 표시
            Handles.Label(worldPos, $"Vertex {i}");
        }

        // 새로운 꼭지점 추가 버튼
        if (Handles.Button(polygon2D.transform.position, Quaternion.identity, 0.1f, 0.1f, Handles.DotHandleCap))
        {
            Undo.RecordObject(polygon2D, "Add Polygon Vertex");
            polygon2D.Vertices.Add(Vector2.zero);
            polygon2D.SetVerticesDirty();
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // 인스펙터에 "모든 버텍스 제거" 버튼 추가
        if (GUILayout.Button("모든 버텍스 제거"))
        {
            Undo.RecordObject(polygon2D, "Clear Vertices");
            polygon2D.Vertices.Clear();
            polygon2D.SetVerticesDirty();
        }

        // 인스펙터에 "폴리곤 재설정" 버튼 추가
        if (GUILayout.Button("폴리곤 재설정"))
        {
            Undo.RecordObject(polygon2D, "Reset Polygon");
            polygon2D.Vertices = new List<Vector2>
            {
                new Vector2(0, 0),
                new Vector2(100, 0),
                new Vector2(100, 100),
                new Vector2(0, 100)
            };
            polygon2D.SetVerticesDirty();
        }
    }
}
#endif