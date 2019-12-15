// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Shaders 101/Rage"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}
        _MaskValue ("Mask Value", Range(0,1)) = 0.1
        _MaskColor ("Mask Color", Color) = (0,0,0,1)
        [Toggle(INVERT_MASK)] _INVERT_MASK ("Mask Invert", Float) = 0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag      
            #include "UnityCG.cginc"
 
            #pragma shader_feature INVERT_MASK
 
      float4 _MainTex_TexelSize;
 
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };
 
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv     : TEXCOORD0;
            };
 
            v2f vert (appdata v)
            {
                        v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
 
            #if UNITY_UV_STARTS_AT_TOP
                if (_MainTex_TexelSize.y < 0)
               	o.uv.y = 1 - o.uv.y;
            #endif
 
                return o;
            }
           
            sampler2D _MainTex;
            sampler2D _MaskTex;
            float _MaskValue;
            float4 _MaskColor;
 
            fixed4 frag (v2f i) : SV_Target
            {

              // fixed4 c = tex2D (_MainTex, i.uv);
               // fixed4 gs = (c.r + c.g + c.b) / 3;
               // return lerp (gs, c, tex2D (_Mask, i.uv).r);


                float4 col = tex2D(_MainTex, i.uv);
                float4 mask = tex2D(_MaskTex, i.uv);
                fixed4 gs = (col.r + col.g + col.b) / 3;
                
 
                // Scale 0..255 to 0..254 range.
                float alpha = mask.a * (1 - 1/255.0);
 
                // If the mask value is greater than the alpha value,
                // we want to draw the mask.
                float weight = step(_MaskValue, alpha);
            #if INVERT_MASK
                weight = 1 - weight;
            #endif
 
                // Blend in mask color depending on the weight
                //col.rgb = lerp(_MaskColor, col.rgb, weight);
 
                // Blend in mask color depending on the weight
                // Additionally also apply a blend between mask and scene
                col.rgb = lerp(col.rgb, lerp(gs, col.rgb, weight), _MaskColor.a);
 
                return col;
            }
            ENDCG
        }
    }
}