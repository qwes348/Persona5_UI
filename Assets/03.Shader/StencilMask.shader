Shader "Oniboogie/StencilMask"
{
	Properties{
 
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_ID("Mask ID", Int) = 1
	}
		SubShader{
			Tags{ "RenderType" = "Opaque" "Queue" = "Geometry+1" }
			ColorMask 0
			ZWrite off			
			Stencil{
				Ref[_ID]
				Comp always
				Pass replace
			}
		Pass{
			CGINCLUDE
		struct appdata {
			float4 vertex : POSITION;
		};
		struct v2f {
			float4 pos : SV_POSITION;
		};
 
 
		v2f vert(appdata v) {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			return o;
		}
		half4 frag(v2f i) : SV_Target{
 
			return half4(1,1,1,1);
		}
			ENDCG
 
	}
}
}
