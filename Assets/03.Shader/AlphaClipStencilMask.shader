Shader "Oniboogie/StencilMaskTransitionWithID"
{
    Properties
    {
        _TransitionTex ("Transition Texture", 2D) = "white" {}
        _AlphaCutoff ("Alpha Cutoff", Range(0,1)) = 0.5
        _StencilRef ("Stencil Reference", Float) = 1 // 스텐실 ID 설정
        _StencilComp ("Stencil Comparison", Float) = 8
        _StencilPass ("Stencil Pass", Int) = 0 // 스텐실 패스 설정 (Replace, Keep 등)
    }

    SubShader
    {
        Tags { "Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Stencil
        {
            Ref [_StencilRef]        // 스텐실 ID 참조값
            // Comp [_StencilComp]      // 스텐실 비교 연산
            // Pass [_StencilPass]      // 스텐실 패스 설정
            Comp always
            Pass replace
        }

        Pass
        {
            Name "StencilMaskTransition"
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask 0 // 화면에 아무것도 그리지 않음

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _TransitionTex;
            float _AlphaCutoff;
            float4 _MainTex_ST;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // Transition Texture의 알파 값을 사용하여 마스크 처리
                half4 col = tex2D(_TransitionTex, i.uv);
                if (col.r < _AlphaCutoff) // 검은 부분을 기준으로 클리핑
                    discard;

                return col;
            }
            ENDCG
        }
    }

    FallBack "Transparent"
}
