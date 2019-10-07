Shader "Stencil/Mask"
{
	SubShader
	{
		Tags { "RenderType" = "Opaque" "Queue" = "Geometry-100"}
		ColorMask 0
		ZWrite Off

		Stencil
		{
			Ref 1
			Comp Always
			Pass Replace
		}

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			half4 frag(v2f i) : COLOR
			{
				return fixed4(0,0,0,1);
			}
		ENDCG
		}
	}
}