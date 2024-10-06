Shader "Oniboogie/PolarStar"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        // PolarStar프로퍼티
        _Rotate("Rotate", Float) = 0
        _Frequency("Frequency", Float) = 5.0
        _StarSize("Star Size", Float) = 1.0
        _SineSpeed("Sine Speed", Float) = 5.0
        _Sharpness("Sharpness", Float) = 1.0
        _WhiteColor("White Color", Color) = (1,1,1,1)
        _BlackColor("Black Color", Color) = (0,0,0,1)
        _Pi5Test("Pi5 Test", Float) = 0.628318530718
        _Tiling("Tiling", Vector) = (1,1,1,1)
        _Offset("Offset", Vector) = (0,0,0,0)

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend One OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP


            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float4  mask : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            float _UIMaskSoftnessX;
            float _UIMaskSoftnessY;
            int _UIVertexColorAlwaysGammaSpace;

            float _Rotate;
            float _Frequency;
            float _StarSize;
            float _SineSpeed;
            float _Sharpness;
            fixed4 _WhiteColor;
            fixed4 _BlackColor;

            float _Pi5Test;            
            float2 _Tiling;
            float2 _Offset;

            float PolarStar(float2 p)
            {
                float pi5 = 0.628318530718;

                float m2 = (atan2(p.y, p.x)/_Pi5Test + 10) % 2.0;
                float adjust = -_Sharpness;

                return length(p) * cos((_Pi5Test * adjust) * (m2 - 4.0 * step(1.0, m2) + 1.0)) - 1.0;
            }

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                float4 vPosition = UnityObjectToClipPos(v.vertex);
                OUT.worldPosition = v.vertex;
                OUT.vertex = vPosition;

                float2 pixelSize = vPosition.w;
                pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

                float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                float2 maskUV = (v.vertex.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);
                OUT.texcoord = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
                OUT.mask = float4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));


                if (_UIVertexColorAlwaysGammaSpace)
                {
                    if(!IsGammaSpace())
                    {
                        v.color.rgb = UIGammaToLinear(v.color.rgb);
                    }
                }

                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                //Round up the alpha color coming from the interpolator (to 1.0/256.0 steps)
                //The incoming alpha could have numerical instability, which makes it very sensible to
                //HDR color transparency blend, when it blends with the world's texture.
                const half alphaPrecision = half(0xff);
                const half invAlphaPrecision = half(1.0/alphaPrecision);
                IN.color.a = round(IN.color.a * alphaPrecision)*invAlphaPrecision;

                half4 color = IN.color * (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd);

                #ifdef UNITY_UI_CLIP_RECT
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
                color.a *= m.x * m.y;
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                float2 uv = IN.texcoord - 0.5 + _Offset;
                uv = uv * (2.0 + _Tiling);

                float t = 0.94 + _Rotate;
                float2x2 rotationMatrix = float2x2(float2(cos(t), -sin(t)), float2(sin(t), cos(t)));
                uv = mul(uv.xy, rotationMatrix);
                float d = PolarStar(uv) * 5.0;

                d = sin(d * _Frequency + _Time * _SineSpeed) / 10.0;
                d = smoothstep(0.0, 0.0, d);

                float cl = PolarStar(uv * 5.0 * _StarSize);
                clip(1.0 - cl);
                cl = smoothstep(1.0,1.0,cl);                

                d = d-cl;

                fixed4 whiteCol = fixed4(d,d,d,d) * _WhiteColor;
                fixed4 blackCol = (1.0 - fixed4(d,d,d,d+cl)) * _BlackColor;               
                
                color = color * (whiteCol + blackCol);
                // color = color + blackCol;
                // color = color + whiteCol;
                // color.rgb *= color.a;

                return color;
            }
        ENDCG
        }
    }
}
