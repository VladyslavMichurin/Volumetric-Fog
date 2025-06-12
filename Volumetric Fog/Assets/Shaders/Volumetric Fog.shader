Shader "_MyShaders/Volumetric Fog"
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

            #include "UnityPBSLighting.cginc"

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

            float4 _FogColor;
            float _FogDensity;

            float _Half_FOV_Tan;
            int _RaymarchingSteps;

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
            float RaymarchingTransmittance(float3 _startPos, float3 _endPos, inout float4 _fogColor)
            {
                float3 viewDir = _endPos - _startPos;
                float rayLenght = length(viewDir);
                float3 rayDir = normalize(viewDir);

                float transmittance = 0;
                float stepSize = rayLenght / _RaymarchingSteps;
                float3 currentPos = _startPos;
                for (int i = 0; i <= _RaymarchingSteps; i++)
                {
                    transmittance += _FogDensity * stepSize;

                    //_fogColor += _LightColor0;

                    currentPos += rayDir * stepSize;
                }

                return saturate(transmittance);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 sceneColor = tex2D(_MainTex, i.uv);
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                depth = Linear01Depth(depth) * _ProjectionParams.z;

                float3 ray = ConstructRay(i);
                float4 viewSpacePos = float4(ray * depth, 1);
                float4 worldSpacePos = mul(unity_CameraToWorld, viewSpacePos);

                float4 fogColor = _FogColor;
                float transmittance = RaymarchingTransmittance(_WorldSpaceCameraPos, worldSpacePos, fogColor);

                return lerp(sceneColor, fogColor, transmittance);
            }
            ENDCG
        }
    }
}
