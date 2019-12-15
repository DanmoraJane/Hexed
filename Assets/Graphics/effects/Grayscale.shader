﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Shaders 101/Grayscale"
{
     Properties
     {
         _MainTex ("Texture", 2D) = "white" {}
         _Mask ("Overlay", 2D) = "white" {}
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
             
             sampler2D _MainTex;
             sampler2D _Mask;
 
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
 
             v2f vert (appdata v)
             {
                 v2f o;
                 o.vertex = UnityObjectToClipPos(v.vertex);
                 o.uv = v.uv;
                 return o;
             }
 
             fixed4 frag (v2f i) : SV_Target
             {
                 fixed4 c = tex2D (_MainTex, i.uv);
                 fixed4 gs = (c.r + c.g + c.b) / 3;
                 return lerp (gs, c, tex2D (_Mask, i.uv).r);
             }
             ENDCG
         }
     }
 }