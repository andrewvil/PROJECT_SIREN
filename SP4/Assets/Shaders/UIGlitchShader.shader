Shader "Custom/UIGlitchShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _QuantizeValue("Quantize Value", float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _QuantizeValue;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // i.uv.x = (int)(i.uv.x * _QuantizeValue) / _QuantizeValue;
                // i.uv.y = (int)(i.uv.y * _QuantizeValue) / _QuantizeValue;
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                col.r = (int)(col.r * _QuantizeValue)/_QuantizeValue;
                col.g = (int)(col.g * _QuantizeValue)/_QuantizeValue;
                col.b = (int)(col.b * _QuantizeValue)/_QuantizeValue;



                return col;
            }
            ENDCG
        }
    }
}
