Shader "LSQ/Game Skill/Fog Of War/Blur"
{
    Properties 
	{
		[HideInInspector]_MainTex ("Base (RGB)", 2D) = "" {}
	}

	CGINCLUDE
	
	#include "UnityCG.cginc"
	
	struct appdata
    {
        float4 positionOS : POSITION;
        float2 texcoord : TEXCOORD0;
    };

	struct v2f 
	{
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;

		float4 uv01 : TEXCOORD1;
		float4 uv23 : TEXCOORD2;
		float4 uv45 : TEXCOORD3;
	};
	
	float2 _Offsets;
	
	sampler2D _MainTex;
	
	v2f vert (appdata v) 
	{
		v2f o;

		o.pos = UnityObjectToClipPos(v.positionOS);
		o.uv.xy = v.texcoord.xy;
		o.uv01 =  v.texcoord.xyxy + _Offsets.xyxy * float4(1, 1, -1, -1);
		o.uv23 =  v.texcoord.xyxy + _Offsets.xyxy * float4(1, 1, -1, -1) * 2.0;
		o.uv45 =  v.texcoord.xyxy + _Offsets.xyxy * float4(1, 1, -1, -1) * 3.0;

		return o;
	}
	
	half4 frag (v2f i) : COLOR
	{
		half4 color = float4 (0,0,0,0);

		color += 0.40 * tex2D (_MainTex, i.uv);
		color += 0.15 * tex2D (_MainTex, i.uv01.xy);
		color += 0.15 * tex2D (_MainTex, i.uv01.zw);
		color += 0.10 * tex2D (_MainTex, i.uv23.xy);
		color += 0.10 * tex2D (_MainTex, i.uv23.zw);
		color += 0.05 * tex2D (_MainTex, i.uv45.xy);
		color += 0.05 * tex2D (_MainTex, i.uv45.zw);
		
		return color;
	}

	ENDCG
	
	Subshader 
	{
		Pass 
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }

			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}

	Fallback off
}
