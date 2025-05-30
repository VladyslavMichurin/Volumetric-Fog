Shader "_MyShaders/Distance Fog"
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
            
            float _FogStart, _FogEnd, _FogDensity;
            
            float4 _FogColor;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                float viewDist = Linear01Depth(depth) * _ProjectionParams.z;

                float fogFactor = viewDist * _FogDensity;
                #if defined(EXPONENTIAL)
                    fogFactor *= 1.5;
                    fogFactor = exp2(-fogFactor);
                #elif(EXPONENTIAL_SQRD)
                    fogFactor *= 1.25;
                    fogFactor = exp2(-pow(fogFactor, 2));
                #else
                    fogFactor = (_FogEnd - viewDist) / max(0.01, _FogEnd - _FogStart);
                #endif

                float4 output = lerp(_FogColor, col, saturate(fogFactor));

                return output;
            }
            ENDCG
        }
    }
}
