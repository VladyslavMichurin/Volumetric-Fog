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

            #pragma target 3.0

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"

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
            sampler2D  _MyShadowMap;     

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
            fixed4 GetCascadeWeights(float _z)
            {
                float4 zNear = float4(_z >= _LightSplitsNear); 
                float4 zFar = float4(_z < _LightSplitsFar); 
                return zNear * zFar; 
			}
            float4 GetShadowCoord(float4 _worldPos, float4 _weights)
            {
			    float3 shadowCoord = float3(0,0,0);
			       
			    shadowCoord += mul(unity_WorldToShadow[0], _worldPos).xyz * _weights[0];
                shadowCoord += mul(unity_WorldToShadow[1], _worldPos).xyz * _weights[1];
                shadowCoord += mul(unity_WorldToShadow[2], _worldPos).xyz * _weights[2];
                shadowCoord += mul(unity_WorldToShadow[3], _worldPos).xyz * _weights[3];
			   
                return float4(shadowCoord,1);            
			} 
            float RaymarchingTransmittance(float3 _startPos, float3 _endPos, inout float4 _fogColor, float _viewPosZ)
            {
                float3 viewDir = _endPos - _startPos;
                float rayLenght = length(viewDir);
                float3 rayDir = normalize(viewDir);

                float4 cascadeWeights = GetCascadeWeights(_viewPosZ);

                float transmittance = 1;
                float stepSize = rayLenght / _RaymarchingSteps;
                float3 currentPos = _startPos; 
                
                for (int i = 0; i <= _RaymarchingSteps; i++)
                {
                    float4 shadowCoord = GetShadowCoord(float4(currentPos, 1), cascadeWeights);
                    float mapDepth = tex2D(_MyShadowMap, shadowCoord.xy).r;
                    float shadowTerm = shadowCoord.z <= mapDepth ? 1.0 : 0.0;

                    _fogColor.rgb += _LightColor0.rgb * shadowTerm * _FogDensity * stepSize;

                    transmittance *= exp(-_FogDensity * stepSize);
                    currentPos += rayDir * stepSize;
                }

                return transmittance;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 sceneColor = tex2D(_MainTex, i.uv);
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                depth = Linear01Depth(depth) * _ProjectionParams.z;

                float3 ray = ConstructRay(i);
                float4 viewSpacePos = float4(ray * depth, 1);
                float4 worldSpacePos = mul(unity_CameraToWorld, viewSpacePos);

                float4 finalFogColor = _FogColor;
                float transmittance = RaymarchingTransmittance(_WorldSpaceCameraPos, worldSpacePos, finalFogColor, viewSpacePos.z);

                return lerp(sceneColor, finalFogColor, 1 - saturate(transmittance));
            }
            ENDCG
        }
    }
}
