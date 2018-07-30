Shader "Distort/CornerPin"
{
	Properties{
			_MainTex("Base (RGB)", 2D) = "white" {}
			_DLx("Down Left x", Range(-2.0,2.0)) = 0
			_DLy("Down Left y", Range(-2.0,2.0)) = 0

			_DRx("Down Right x", Range(-2.0,2.0)) = 0
			_DRy("Down Right y", Range(-2.0,2.0)) = 0

			_TLx("Top Left x", Range(-2.0,2.0)) = 0
			_TLy("Top Left y", Range(-2.0,2.0)) = 0
			
			_TRx("Top Right x", Range(-2.0,2.0)) = 0
			_TRy("Top Right y", Range(-2.0,2.0)) = 0
	}

		SubShader{
		Pass{
		ZTest Always Cull Off ZWrite Off
		Fog{ Mode off }

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest 
		#include "UnityCG.cginc"

		uniform sampler2D _MainTex;

	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
	};

	float _DRx;
	float _DRy;

	float _DLx;
	float _DLy;

	float _TLx;
	float _TLy;

	float _TRx;
	float _TRy;

	v2f vert(appdata_img v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex); 
		o.uv = v.texcoord;

		o.pos.x += _DLx*o.uv.x*o.uv.y;
		o.pos.y += _DLy*o.uv.x*o.uv.y;
		
		o.pos.x += _DRx*(1 - o.uv.x)*o.uv.y;	
		o.pos.y += _DRy*(1 - o.uv.x)*o.uv.y;

		o.pos.x += _TLx*o.uv.x*(1 - o.uv.y);
		o.pos.y += _TLy*o.uv.x*(1 - o.uv.y);

		o.pos.x += _TRx*(1 - o.uv.x)*(1 - o.uv.y);
		o.pos.y += _TRy*(1 - o.uv.x)*(1 - o.uv.y);

		return o;
	}

	float4 frag(v2f i) : SV_Target
	{
		

		return tex2D(_MainTex, i.uv);
	}
		ENDCG
	}
		}
			Fallback off
}