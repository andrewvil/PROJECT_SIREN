Shader "Custom Post-Processing/NightVision"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _LuminosityMidpoint("Luminosity Midpoint", Range(0, 1)) = 0
        _LuminosityIntensity("Luminosity Intensity", Range(0, 50)) = 0
        _NightVisionTint("Night Vision Tint", Color) = (0, 0, 0, 0)
        _NightVisionIntensity("Night Vision Intensity", Range(0, 20)) = 0
    }

    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always

        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
            uniform float _LuminosityMidpoint;
            uniform float _LuminosityIntensity;
            uniform float4 _NightVisionTint;
            uniform float _NightVisionIntensity;
            uniform float _blend;
            uniform int _pixelize;

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            float luminance(float3 c)
            {
                return dot(c, float3(0.2126, 0.7152, 0.0722));
            }

            float4 frag(v2f i) : SV_Target
            {
                if(_pixelize == 1) {
                    i.uv.x = (int)(i.uv.x * 750) / 750.f;
                    i.uv.y = (int)(i.uv.y * 250) / 250.f;
                }
                float3 c = tex2D(_MainTex, i.uv).rgb;
                float lum = saturate(lerp(_LuminosityMidpoint, luminance(c), _LuminosityIntensity));
                c = lum * _NightVisionTint * _NightVisionIntensity;

                if(_pixelize == 1) {
                    c.r = (int)(c.r*10) / 10.f;
                    c.g = (int)(c.g*10) / 10.f;
                    c.b = (int)(c.b*10) / 10.f;
                }

                float3 result = lerp(tex2D(_MainTex, i.uv).rgb,c,_blend);
                return float4(result,1);
            }
            ENDHLSL
        }
    }
}
