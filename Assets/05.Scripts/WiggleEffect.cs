using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class WiggleEffect : MonoBehaviour, IMeshModifier
{
    public AnimationCurve wiggleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // 기본적으로 부드러운 곡선
    public float wiggleAmountX = 5f;  // 흔들림의 크기
    public float wiggleAmountY = 5f;
    public float speed = 2f;         // 흔들림 속도
    public float curveDuration = 1f; // 곡선의 주기 (wiggle 애니메이션의 주기)

    private void Update()
    {
        // UI 메쉬를 지속적으로 업데이트하기 위해 재갱신
        GetComponent<Graphic>().SetVerticesDirty();
    }

    public void ModifyMesh(VertexHelper vh)
    {
        if (!this.isActiveAndEnabled) return;

        var vertices = new List<UIVertex>();
        vh.GetUIVertexStream(vertices);

        // 현재 시간을 0과 1 사이의 값으로 정규화
        //float time = (Time.time * speed) % curveDuration / curveDuration;
        float time = Mathf.PingPong(Time.time * speed, 1f);

        for (int i = 0; i < vertices.Count; i++)
        {
            UIVertex vertex = vertices[i];

            // AnimationCurve를 사용하여 흔들림 값을 계산
            float curveValue = wiggleCurve.Evaluate(time); // 0 ~ 1 범위의 곡선 값
            float wiggleOffsetX = curveValue * wiggleAmountX; // 곡선 값에 따른 흔들림 크기
            float wiggleOffsetY = curveValue * wiggleAmountY; // 곡선 값에 따른 흔들림 크기

            // 버텍스의 Y 위치에 wiggle 효과 적용
            vertex.position.y += Mathf.Sin(vertex.position.x * 0.1f + time * Mathf.PI * 2) * wiggleOffsetY;
            vertex.position.x += Mathf.Sin(vertex.position.y * 0.1f + time * Mathf.PI * 2) * wiggleOffsetX;

            vertices[i] = vertex; // 변형된 버텍스를 리스트에 다시 반영
        }

        vh.Clear();
        vh.AddUIVertexTriangleStream(vertices);
    }

    public void ModifyMesh(Mesh mesh)
    {
        // IMeshModifier 인터페이스의 구현 요구사항으로 비워둠
    }
}
