Shader "LSQ/Game Skill/Fog Of War/Post Effect"
{
    Properties
    {
        _MainTex ("Source", 2D) = "white"{}
        _Color ("Color", color) = (1,1,1,1)
    }

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            sampler2D _MaskTex;
            sampler2D _CameraDepthTexture;
            float4x4 _ViewToWorld;
            float4 _WorldParams;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float2 GetUVCoord(float3 worldPos) 
            {
                float x = saturate((worldPos.x - _WorldParams.x) / _WorldParams.z);
                float z = saturate((worldPos.z - _WorldParams.y) / _WorldParams.w);
                return float2(x, z);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float depth = Linear01Depth(DecodeFloatRG(tex2D(_CameraDepthTexture, i.uv)));
                float z = depth * _ProjectionParams.z;
                float2 p11_22 = float2(unity_CameraProjection._11, unity_CameraProjection._22);
                float3 viewPos = float3((i.uv * 2 - 1) / p11_22, -1) * z;
                float3 worldPos = mul(_ViewToWorld, float4(viewPos, 1)).xyz; 
                float2 uv = GetUVCoord(worldPos); //return float4(uv, 0, 1);

                fixed4 col = tex2D(_MainTex, i.uv);
                fixed mask = tex2D(_MaskTex, float2(uv.x, uv.y)).r; //return mask;
                return lerp(_Color, col, mask);
            }
            ENDCG
        }
    }
}
