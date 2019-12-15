Shader "Sprites/Mask3"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1, 1, 1, 1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [PerRendererData] _MaskTargetX ("Mask Target X", Float) = 0
        [PerRendererData] _MaskTargetY ("Mask Target Y", Float) = 0
        [PerRendererData] _RenderDistance ("RenderDistance", Float) = 1
        [PerRendererData] _MaskType ("MaskType", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
                fixed4 worldPosition : WORLDPOS;
			};
			
					
			fixed4 _Color;
            float _MaskTargetX;
            float _MaskTargetY;
            float _RenderDistance;
            float _MaskType;


			v2f vert(appdata_t IN)
			{
				v2f OUT;
                OUT.worldPosition = mul(unity_ObjectToWorld, fixed4(IN.vertex.x, IN.vertex.y, 0, 1));
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
				if (_AlphaSplitEnabled)
					color.a = tex2D (_AlphaTex, uv).r;
#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
                float dist = distance(IN.worldPosition, fixed4(_MaskTargetX, _MaskTargetY, IN.worldPosition.z, 1));
				if (_MaskType > 0 && dist <= _RenderDistance)
				{
				if (_MaskType <= 1) {c.a = 0;}
				else if (_MaskType <= 2)
				{
					float amnt = c.r*0.299 + c.g*0.587 + c.b*0.114;	
					c.rgb = fixed3(amnt, amnt, amnt);
				}
                else if (_MaskType <= 3){
					float amnt = c.r*0.299 + c.g*0.587 + c.b*0.114;	
                    c.rgb = fixed3(amnt, amnt, amnt);
                    c.a = c.a * (1 - dist/_RenderDistance);
               		} 
				}
                

                c.rgb *= c.a;
                return c;
			}
		ENDCG
		}
	}
}