Shader "Unlit/HeightFog"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ EXPONENTIAL EXPONENTIAL_SQRD

            #include "UnityCG.cginc"

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

            sampler2D _MainTex, _CameraDepthTexture;
            float _Half_FOV_Tan;
            float _FogStart, _FogEnd, _FogDensity;
            
            float4 _FogColor;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            float3 ConstructRay(v2f i)
            {
                float3 ray;

                float2 ndc = i.uv * 2 - 1;
                float screenAspect = _ScreenParams.x / _ScreenParams.y;

                float x = _Half_FOV_Tan * ndc.x * screenAspect;
                float y = _Half_FOV_Tan * ndc.y;

                ray = float3(x, y, 1);

                return ray;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                depth = Linear01Depth(depth) * _ProjectionParams.z;

                float3 ray = ConstructRay(i);

                float4 viewSpacePos = float4(ray * depth, 1);
                float4 worldSpacePos = mul(unity_CameraToWorld, viewSpacePos);

                float4 fogFactor = 1;
                #if defined(EXPONENTIAL)

                #elif(EXPONENTIAL_SQRD)

                #else
                    fogFactor = (worldSpacePos.y - _FogStart) / max(0.01, _FogEnd - _FogStart);
                #endif

                float4 output = lerp(_FogColor, col, saturate(fogFactor));

                return output;
            }
            ENDCG
        }
    }
}
